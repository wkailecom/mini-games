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

    public int IssueNum => mData.IssueNum;
    public int CurLevel => mData.CurrentLevel;
    public int RecLevel => mData.RecordLevel;
    public bool IsCompleted => mData.IsComplete;
    public MiniGameInfoData InfoData => mData;
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public MiniGameType GameType => (MiniGameType)mScheduleConfig.gameType;
    public int MaxLevel => mMapConfigs.Count;

    List<MiniMapConfig> mMapConfigs;
    MiniScheduleConfig mScheduleConfig;
    MiniGameInfoData mData;
    protected override void OnInit()
    {
        Deserialize();

        EventManager.Register(EventKey.PurchaseSuccess, OnPurchaseSuccess);
        EventManager.Register(EventKey.VideoADRewarded, OnVideoADRewarded);
        EventManager.Register(EventKey.MiniGameOver, OnMiniGameOver);

        RefreshData();
        MiniGameManager.Instance.Init();
    }

    void RefreshData()
    {
        mScheduleConfig = ConfigData.miniScheduleConfig.GetByPrimary(IssueNum, false);
        StartTime = DateTime.Parse(mScheduleConfig.startTime);
        EndTime = DateTime.Parse(mScheduleConfig.endTime);
        mMapConfigs = ConfigData.miniMapConfig.GetByIndexes(mScheduleConfig.mapIndex);
    }

    void OnPurchaseSuccess(EventData pEventData)
    {
        var tEventData = pEventData as PurchaseSuccess;
        var tConfig = tEventData.productConfig;
        if (tConfig.shopId == 2 || tConfig.shopId == 101 || tConfig.shopId == 102)
        {
            var tPropDatas = GameMethod.ParseProps(tConfig.propsID, tConfig.propsCount);
            PageManager.Instance.OpenPage(PageID.RewardPage, new RewardPageParam(tPropDatas, PropSource.Shop));
        }
    }

    void OnVideoADRewarded(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.RewardVideo)
        {
            var ttPropID = GetPropID(tEventData.showReason);
            if (ttPropID != PropID.Invalid)
            {
                var tPageParam = new RewardPageParam(ttPropID, 1, PropSource.Rrewarded);
                PageManager.Instance.OpenPage(PageID.RewardPage, tPageParam);
            }
        }
    }

    void OnMiniGameOver(EventData pEventData)
    {
        var tEventData = pEventData as MiniGameOver;

        if (tEventData.isSuccess)
        {
            mData.CurrentLevel++;
        }

    }



    public void SyncLevel()
    {
        mData.RecordLevel = mData.CurrentLevel;

        Serialize();
    }

    public int GetNewIssue()
    {
        DateTime tNow = DateTime.Now;
        foreach (var tConfig in ConfigData.miniScheduleConfig.DataList)
        {
            //if (tConfig.ID == mData.IssueNum) continue;//新活动和记录活动相同跳过
            // 先解析结束时间并比较
            if (DateTime.TryParse(tConfig.endTime, out DateTime tEndTime) && tNow < tEndTime)
            {
                if (DateTime.TryParse(tConfig.startTime, out DateTime tStartTime) && tNow >= tStartTime)
                {
                    return tConfig.ID;
                }
            }
        }

        return -1;
    }

    public void StartNewIssue(int pIssueNum)
    {
        mData.IssueNum = pIssueNum;
        mData.IsComplete = false;
        mData.CurrentLevel = 1;
        mData.RecordLevel = 1;

        RefreshData();
        Serialize();
    }

    public void RetryGame()
    {
        mData.RetryCount += 1;
        Serialize();
    }

    public bool IsComplete()
    {
        return CurLevel > MaxLevel;
    }

    public bool IsUnderway()
    {
        if (StartTime <= DateTime.Now && DateTime.Now < EndTime)
        {
            return true;
        }
        return false;
    }


    public MiniMapConfig GetLevelConfig(int pLevel)
    {
        return mMapConfigs.FirstOrDefault(config => config.level == pLevel);
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

    public int GetLevelID(int pLevel)
    {
        var tConfig = GetLevelConfig(pLevel);
        var strArray = tConfig.Chessboard.Split(';');

        return strArray[0].ToInt();
    }

    public int[] GetLevelIDs(int pLevel, int pLevelCount)
    {
        var tConfig = GetLevelConfig(pLevel);
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


    #region 资源处理

    HashSet<ADShowReason> MiniGameAD = new HashSet<ADShowReason>
    {
        ADShowReason.Video_GetScrewExtraSlot,
        ADShowReason.Video_GetScrewHammer,
        ADShowReason.Video_GetScrewExtraBox,
        ADShowReason.Video_GetJam3DReplace,
        ADShowReason.Video_GetJam3DRevert,
        ADShowReason.Video_GetJam3DShuffle,
        ADShowReason.Video_GetTileRecall,
        ADShowReason.Video_GetTileMagnet,
        ADShowReason.Video_GetTileShuffle
    };

    public bool IsMiniGameAD(ADShowReason pReason) => MiniGameAD.Contains(pReason);

    public PropID GetPropID(ADShowReason pReason)
    {
        return pReason switch
        {
            ADShowReason.Video_GetScrewExtraSlot => PropID.ScrewExtraSlot,
            ADShowReason.Video_GetScrewHammer => PropID.ScrewHammer,
            ADShowReason.Video_GetScrewExtraBox => PropID.ScrewExtraBox,
            ADShowReason.Video_GetJam3DReplace => PropID.Jam3DReplace,
            ADShowReason.Video_GetJam3DRevert => PropID.Jam3DRevert,
            ADShowReason.Video_GetJam3DShuffle => PropID.Jam3DShuffle,
            ADShowReason.Video_GetTileRecall => PropID.TileRecall,
            ADShowReason.Video_GetTileMagnet => PropID.TileMagnet,
            ADShowReason.Video_GetTileShuffle => PropID.TileShuffle,
            _ => PropID.Invalid,
        };
    }

    #endregion

    #region 序列化
    void Serialize()
    {
        DataTool.Serialize(RECORD_KEY, mData);
    }

    void Deserialize()
    {
        mData = DataTool.Deserialize<MiniGameInfoData>(RECORD_KEY);
    }

    #endregion

    #region Editor



    #endregion
}
