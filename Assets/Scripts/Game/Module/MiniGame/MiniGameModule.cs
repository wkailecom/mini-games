using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Config;
using Game;
using Game.UISystem;
using System.Linq;
using Game.UI;
using Game.MiniGame;

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
        EventManager.Register(EventKey.MiniGameRevive, OnMiniGameRevive);
        EventManager.Register(EventKey.MiniLevelOver, OnMiniLevelOver);

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
            if (tPropID != PropID.Invalid && tPropID != PropID.Coin)
            {
                var tPageParam = new RewardPageParam(tPropID, 1, PropSource.Rrewarded);
                PageManager.Instance.OpenPage(PageID.RewardPage, tPageParam);
            }
        }
    }

    void OnMiniGameStart(EventData pEventData)
    {
        var tEventData = pEventData as MiniGameStart;
        int pTypeId = (int)tEventData.modeType;
        if (tEventData.isNewGame)
        {
            SetIsNewGame(pTypeId, false);
            SetReset(pTypeId);
            Serialize(pTypeId);
        }
        else
        {
            AddRetryCount(pTypeId);
            SetReviveCount(pTypeId, 0);
            Serialize(pTypeId);
        }
    }

    void OnMiniLevelOver(EventData pEventData)
    {
        var tEventData = pEventData as MiniLevelOver;
        int pTypeId = (int)tEventData.modeType;

        if (tEventData.isSuccess)
        { 
            AddCurLevel(pTypeId);
            SetIsNewGame(pTypeId, true);
            Serialize(pTypeId);
        }

        ModuleManager.MiniTower.TowerUpdate(tEventData.isSuccess);
        if (tEventData.isSuccess)
        {
            var tActivityValid = ModuleManager.MiniTower.ActivityValid();
            if (tActivityValid && ModuleManager.MiniTower.HasReward(ModuleManager.MiniTower.CurFloor))
            {
                var tPageParam = new TowerMapPageParam(TowerMapPageParam.OpenFrom.LevelSuccess, () =>
                {
                    PageManager.Instance.OpenPage(PageID.MiniSucceedPage, new MiniSucceedPageParam());
                }); 
                PageManager.Instance.OpenPage(PageID.TowerMapPage, tPageParam);
            }
            else
            {
                PageManager.Instance.OpenPage(PageID.MiniSucceedPage, new MiniSucceedPageParam());
            }
        }
        else
        {
            var tPageParam = new TowerMapPageParam(TowerMapPageParam.OpenFrom.ExitLevel, () =>
            {
                PageManager.Instance.OpenPage(PageID.HomePage);
            });

            //var tPageParam1 = new TowerMapPageParam(TowerMapPageParam.OpenFrom.LevelFail, () =>
            //{
            //    var tCurConfig = ModuleManager.MiniGame.GetTypeConfig(pTypeId);
            //    PageManager.Instance.OpenPage(PageID.MiniEnterPage, new MiniEnterPageParam(tCurConfig));
            //});
            PageManager.Instance.OpenPage(PageID.TowerMapPage, tPageParam);
        }
    }

    void OnMiniGameOver(EventData pEventData)
    {
        //var tEventData = pEventData as MiniGameOver;
        //if (tEventData.isSuccess)
        //{
        //    OnGameSuccess?.Invoke(tEventData.modeType);
        //}
        //else
        //{
        //    OnGameFailed?.Invoke(tEventData.modeType);
        //}


        /*
         * 退出游戏 ——> 关卡未完成
         * 
         * 游戏成功
         * 关卡完成 ——> 是否活动 ——> 爬塔-成功页/成功页
         * 
         * 游戏失败
         * 获取死亡页参数，复活道具，是否有效，复活逻辑
         * 道具复活 ——> 执行复活逻辑()
         * 金币重试 ——> 执行重试游戏(不消耗体力)
         * 放弃选择 ——> 关卡未完成
         * 
         * 关卡未完成 ——> 是否活动 ——> 爬塔-首页/首页 
         * 
         */
    }

    void OnMiniGameRevive(EventData pEventData)
    {
        var tEventData = pEventData as MiniGameRevive;
        int pTypeId = (int)tEventData.modeType;
        AddReviveCount(pTypeId);
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

    public int GetReviveCount(int pTypeId)
    {
        return GetGameData(pTypeId).ReviveCount;
    }
    public void SetReviveCount(int pTypeId, int pValue)
    {
        GetGameData(pTypeId).ReviveCount = pValue;
    }
    public void AddReviveCount(int pTypeId, int pValue = 1)
    {
        GetGameData(pTypeId).ReviveCount += pValue;
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
    public void SetReset(int pTypeId)
    {
        GetGameData(pTypeId).Reset();
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

        return mMapConfigs?.FirstOrDefault(config => config.level == pLevel);
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
        var strArray = tConfig.Chessboard.Split(',');
        System.Random random = new System.Random(pLevel);
        int result = random.Next(0, strArray.Length);
        return strArray[result].ToInt();
    }

    public int[] GetLevelIDs(MiniGameType pTypeId, int pLevel, int pLevelCount)
    {
        pLevel = GetLoopLevel((int)pTypeId, pLevel);
        var tConfig = GetLevelConfig(pTypeId, pLevel);
        var strArray = tConfig.Chessboard.Split(',');
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

    public int GetTripleLevelTimes(MiniMapConfig pMapConfig, int tLevelID)
    {
        var time = 50;
        foreach (var item in ConfigData.tripleLevelSeedConfig.DataList)
        {
            if (item.id == tLevelID)
            {
                time = pMapConfig.LimitTime switch
                {
                    1 => item.diff_1,
                    2 => item.diff_2,
                    3 => item.diff_3,
                    4 => item.diff_4,
                    5 => item.diff_5,
                    _ => 50,
                };
                return time;
            }
        }

        return time;
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
