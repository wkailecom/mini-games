using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Config;
using Game;
using Game.UISystem;
using System.Linq;

public class MiniGameModule : ModuleBase
{
    const string RECORD_KEY = "RecordMiniGameInfo";

    List<MiniMapConfig> mMapConfigs;
    //MiniGameInfoData mData;

    Dictionary<int, MiniGameData> mGameData;
    protected override void OnInit()
    {
        Deserialize();

        EventManager.Register(EventKey.PurchaseSuccess, OnPurchaseSuccess);
        EventManager.Register(EventKey.VideoADRewarded, OnVideoADRewarded);
        EventManager.Register(EventKey.MiniGameStart, OnMiniGameStart);
        EventManager.Register(EventKey.MiniGameOver, OnMiniGameOver);

        RefreshData();
        MiniGameManager.Instance.Init();
    }

    void RefreshData()
    {

    }

    #region 事件方法

    void OnPurchaseSuccess(EventData pEventData)
    {
        var tEventData = pEventData as PurchaseSuccess;
        var tConfig = tEventData.productConfig;

        var tPropDatas = GameMethod.ParseProps(tConfig.propsID, tConfig.propsCount);
        var tPageParam = new RewardPageParam(tPropDatas, PropSource.Shop);
        PageManager.Instance.OpenPage(PageID.RewardPage, tPageParam);
    }

    void OnVideoADRewarded(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.RewardVideo)
        {
            var tPropID = DataConvert.GetADPropID(tEventData.showReason);
            if (tPropID != PropID.Invalid)
            {
                var tPageParam = new RewardPageParam(tPropID, 1, PropSource.Rrewarded);
                PageManager.Instance.OpenPage(PageID.RewardPage, tPageParam);
            }
        }
    }

    void OnMiniGameStart(EventData pEventData)
    {
        var tEventData = pEventData as MiniGameStart;
        if (tEventData.isNewGame)
        {
            int pTypeId = (int)tEventData.modeType;

            SetIsNewGame(pTypeId, false);
            AddRetryCount(pTypeId);
            Serialize(pTypeId);
        }
    }

    void OnMiniGameOver(EventData pEventData)
    {
        var tEventData = pEventData as MiniGameOver;
        if (tEventData.isSuccess)
        {
            int pTypeId = (int)tEventData.modeType;

            AddCurLevel(pTypeId);
            SetRetryCount(pTypeId, 0);
            SetIsNewGame(pTypeId, true);
            Serialize(pTypeId);
        }
    }

    #endregion

    #region 记录数据信息

    public MiniGameData GetGameData(int pTypeId)
    {
        var tData = mGameData.GetValue(pTypeId);
        if (tData == null)
        {
            tData = new MiniGameData();
            mGameData.Add(pTypeId, tData);
        }
        return tData;
    }

    //当前关卡
    public int GetCurLevel(int pTypeId)
    {
        return GetGameData(pTypeId).CurLevel;
    }
    public void SetCurLevel(int pTypeId, int pValue)
    {
        GetGameData(pTypeId).CurLevel = pValue;
    }
    public void AddCurLevel(int pTypeId, int pValue = 1)
    {
        GetGameData(pTypeId).CurLevel += pValue;
    }

    public int GetRetryCount(int pTypeId)
    {
        return GetGameData(pTypeId).RetryCount;
    }
    public void SetRetryCount(int pTypeId, int pValue)
    {
        GetGameData(pTypeId).RetryCount = pValue;
    }
    public void AddRetryCount(int pTypeId, int pValue = 1)
    {
        GetGameData(pTypeId).RetryCount += pValue;
    }

    public bool IsNewGame(int pTypeId)
    {
        return GetGameData(pTypeId).IsNewGame;
    }
    public void SetIsNewGame(int pTypeId, bool pValue)
    {
        GetGameData(pTypeId).IsNewGame = pValue;
    }


    #endregion

    #region 其他信息 

    public MiniTypeConfig GetTypeConfig(int pTypeId)
    {
        return ConfigData.miniTypeConfig.GetByPrimary(pTypeId);
    }

    public List<MiniMapConfig> GetTypeMapConfig(int pTypeId)
    {
        return ConfigData.miniMapConfig.GetByIndexes(pTypeId, 1);
    }

    public MiniMapConfig GetLevelConfig(MiniGameType pTypeId, int pLevel)
    {
        mMapConfigs = GetTypeMapConfig((int)pTypeId);

        return mMapConfigs.FirstOrDefault(config => config.level == pLevel);
    }

    MiniMapConfig GetLevelConfigByIndex(MiniGameType pTypeId, int pIndex)
    {
        mMapConfigs = GetTypeMapConfig((int)pTypeId);

        if (pIndex < 0 || pIndex >= mMapConfigs.Count)
        {
            LogManager.LogError($"GetLevelConfig {pIndex + 1} 配置存在");
            return null;
        }
        return mMapConfigs[pIndex];
    }

    public MiniMapConfig GetCurLevelConfig()
    {
        return GetLevelConfig(MiniGameManager.Instance.GameType, MiniGameManager.Instance.Level);
    }

    int GetLoopLevel(int pTypeId, int pLevel)
    {
        int tMaxLevel = GetMaxLevel(pTypeId);
        return GameMethod.GetLoopIndex(pLevel, tMaxLevel) + 1;
    }

    public int GetLevelID(MiniGameType pTypeId, int pLevel)
    {
        pLevel = GetLoopLevel((int)pTypeId, pLevel);
        var tConfig = GetLevelConfig(pTypeId, pLevel);
        var strArray = tConfig.Chessboard.Split(';');
        return strArray[0].ToInt();
    }

    public int[] GetLevelIDs(MiniGameType pTypeId, int pLevel, int pLevelCount)
    {
        pLevel = GetLoopLevel((int)pTypeId, pLevel);
        var tConfig = GetLevelConfig(pTypeId, pLevel);
        var strArray = tConfig.Chessboard.Split(';');
        var tSeed = pLevel + pLevelCount + DateTime.Now.Month;
        System.Random random = new System.Random(tSeed);
        var strList = new List<string>(strArray);
        List<int> tResult = Enumerable.Repeat(-1, pLevelCount).ToList();
        for (int i = strList.Count - 1; i >= 0; i--)
        {
            var tmpChessboard = strList[i].Split('&');
            if (tmpChessboard.Length > 1)
            {
                int index = int.Parse(tmpChessboard[1]);
                tResult[index - 1] = int.Parse(tmpChessboard[0]);
                strList.RemoveAt(i);
            }
        }

        for (int i = 0; i < strList.Count; i++)
        {
            int index = random.Next(i, strList.Count);
            string temp = strList[i];
            strList[i] = strList[index];
            strList[index] = temp;
        }

        int k = 0;
        for (int i = 0; i < tResult.Count; i++)
        {
            if (tResult[i] == -1)
            {
                tResult[i] = int.Parse(strList[k]);
                k++;
            }
        }

        //StringBuilder str = new StringBuilder();
        //for (int i = 0; i < tResult.Count; i++)
        //{
        //    str.Append(tResult[i] + ",");
        //}
        //PlayingChessboardID = str.ToString();
        //Debug.Log($"=== current tSeed:{tSeed}, current chessboards: {str}");
        return tResult.ToArray();
    }

    public List<PropData> GetLevelReward(string pRewardStr)
    {
        var tResult = new List<PropData>();
        if (string.IsNullOrEmpty(pRewardStr)) return tResult;

        var tProp = pRewardStr.Split(';');
        foreach (var item in tProp)
        {
            var tP = item.Split(',');
            tResult.Add(new PropData((PropID)tP[0].ToInt(), tP[1].ToInt()));
        }
        return tResult;
    }

    public int GetMaxLevel(int pTypeId)
    {
        return GetTypeMapConfig(pTypeId).Count;
    }

    #endregion



    #region 资源处理



    #endregion

    #region 序列化

    string GetSerializeKey(int pTypeId)
    {
        return $"{RECORD_KEY}_{pTypeId}";
    }

    MiniGameData GetSerializeData(int pTypeId)
    {
        return mGameData.GetValue(pTypeId);
    }

    void Serialize(int pTypeId)
    {
        DataTool.Serialize(GetSerializeKey(pTypeId), GetSerializeData(pTypeId));
    }

    void Serialize()
    {
        //DataTool.Serialize(RECORD_KEY, mData);

        foreach (var tData in mGameData)
        {
            Serialize(tData.Key);
        }
    }

    void Deserialize()
    {
        //mData = DataTool.Deserialize<MiniGameInfoData>(RECORD_KEY);

        mGameData = new Dictionary<int, MiniGameData>();
        foreach (var tConfig in ConfigData.miniTypeConfig.DataList)
        {
            mGameData.Add(tConfig.ID, DataTool.Deserialize<MiniGameData>(GetSerializeKey(tConfig.ID)));
        }
    }

    #endregion

    #region Editor



    #endregion
}
