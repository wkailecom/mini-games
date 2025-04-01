using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.UISystem;
using Config;
using System.Linq;
//using Unity.Mathematics;
using System.IO;
using System.Diagnostics;

public class TileGameModule : ModuleBase
{
    const string TILE_ROOT_PATH = "LevelInfos/Tile/";
    const int ACTIVITY_PERIOD_DAYS = 7;
    public int CurrIssueNum => mTileGameInfo.IssueNum;
    public int CurrentLevel => mTileGameInfo.CurrentLevel;
    public int CurrentCheckLevel => mTileGameInfo.CurCheckLevel;
    public int RetryCount => mTileGameInfo.RetryCount;
    public long PlayTime => mTileGameInfo.PlayTime;
    //public int MaxMapCount => TileMapConfigs.Count;
    //public int MaxLevelCount => TileMapConfigs.Count;
    //public bool IsCompleteAllLevel => CurrentLevel > MaxLevelCount;
    //public int CacheCurrentLevel => IsCompleteAllLevel ? MaxLevelCount : CurrentLevel;

    #region TileGame_Map_Variable
    public int BeginLevelId { get; private set; }
    public int EndLevelId { get; private set; }
    public int MaxReachLevel => mTileGameInfo.MaxReachLevel;
    public int CurCheckLevel => mTileGameInfo.CurCheckLevel;
    public List<int> OpenedGifts => mTileGameInfo.OpenedGifts;

    #endregion

    #region 引导相关
    const string GUIDE_FINISHED_KEY = "TileGameGuideFinished";

    /// <summary>
    /// 进到步骤 0未引导 1完成入口 2完成开始游戏 3完成点击块 4完成点击道具
    /// </summary>
    public int IsGuideStep
    {
        get => PlayerPrefs.GetInt(GUIDE_FINISHED_KEY, 0);
        set => PlayerPrefs.SetInt(GUIDE_FINISHED_KEY, value);
    }

    public bool IsNoviceGuide => IsGuideStep == 0;
    public bool IsGuideEntrance => IsGuideStep == 0;
    public bool IsGuideStart => mTileGameInfo.IssueNum == 1 && mTileGameInfo.CurrentLevel == 1 && IsGuideStep < 2;
    public bool IsGuideClick => mTileGameInfo.IssueNum == 1 && mTileGameInfo.CurrentLevel == 1 && IsGuideStep < 3;
    public bool IsGuideProp => mTileGameInfo.IssueNum == 1 && mTileGameInfo.CurrentLevel == 2 && IsGuideStep < 4;

    #endregion

    #region 临时统计

    const string STATISTICS_DATA_KEY = "tileStatisticsData_Key";
    const string FAIL_SELECTION_KEY = "tileFailureSelection_Key";
    Dictionary<int, DataTimes> StatisticsTimes;//<关卡，（失败数，重试数>
    public bool IsFailIng
    {
        get => PlayerPrefs.GetInt(FAIL_SELECTION_KEY, 0) == 1;
        set => PlayerPrefs.SetInt(FAIL_SELECTION_KEY, value ? 1 : 0);
    }
    class DataTimes
    {
        public int lostTimes;
        public int replayTimes;

        DataTimes() { }
        public DataTimes(int pLostTimes, int pReplayTimes)
        {
            lostTimes = pLostTimes;
            replayTimes = pReplayTimes;
        }
    }

    #endregion

    TileInfo mTileInfo;
    TileGameInfo mTileGameInfo;

    //public IReadOnlyList<TileMapConfig> TileMapConfigs => ConfigData.tileMapConfig.DataList;
    //public IReadOnlyList<TileLevelConfig> TileLevelConfigs => ConfigData.tileLevelConfig.DataList;
    //public Dictionary<int, TileLevelData> allLevelData { get; private set; }
    //public Dictionary<int, int4> allLimitData { get; private set; }

    protected override void OnInit()
    {
        //EventManager.Register(EventDataType.ApplicationFocus, OnApplicationFocus);
        //EventManager.Register(EventDataType.PageOpened, OnPageOpened);
        //EventManager.Register(EventDataType.PageClosed, OnPageClosed);
        //EventManager.Register(EventDataType.PurchaseSuccess, OnPurchaseSuccess);
        //EventManager.Register(EventDataType.StartNewDay, Refresh);
        //EventManager.Register(EventDataType.TileStartGame, OnTileStartGame);
        //EventManager.Register(EventDataType.TileGameOver, OnTileGameOver);

        mTileInfo = new TileInfo();
        mTileGameInfo = new TileGameInfo();
        StatisticsTimes = new Dictionary<int, DataTimes>();
        Deserialize();

        Refresh();
    }

    public void Refresh(EventData pEventData = null)
    {
        if (IsUnderway())
        {
            RefreshTileData();
            if (IsFailIng)//杀端重进检查扣除体力
            {
               // ModuleManager.Prop.ExpendProp(PropID.TileHeart);
                IsFailIng = false;
            }
        }
        if (IsHarvestHeart())
        {
            HarvestHeartRecover();
        }
    }

    void OnApplicationFocus(EventData pEventData)
    {
        var tEventData = pEventData as ApplicationFocus;
        if (!tEventData.focus)
        {
            StopPlayTime();
            Serialize();
        }
        else
        {
           // bool tIsStart = ModuleManager.GameAnalyse.InThePageID == 102;
           // StartPlayTime(false, tIsStart);
        }
    }

    void OnPageOpened(EventData pEventData)
    {
        //var tEventData = pEventData as PageOperation;
        //if (tEventData.pageID == PageID.TileGamePage)
        //{
        //    StartPlayTime(false, true);
        //}
    }

    void OnPageClosed(EventData pEventData)
    {
        var tEventData = pEventData as PageOperation;
        //if (tEventData.pageID == PageID.TileGamePage)
        //{
        //    StopPlayTime();
        //}
    }

    void OnPurchaseSuccess(EventData pEventData)
    {
        //var tEventData = pEventData as PurchaseSuccess;
        //var tConfig = tEventData.productConfig;
        //if (tConfig.category != 10) return;

        //List<PropData> tPropDatas = new List<PropData>();
        //for (int i = 0; i < tConfig.propsID.Length; i++)
        //{
        //    tPropDatas.Add(new PropData((PropID)tConfig.propsID[i], tConfig.propsCount[i]));
        //}

        //if (tConfig.showUsage == 4) return;
        //var tPageParam = new RewardPageParam(tPropDatas, PropSource.Shop);
        //PageManager.Instance.OpenPage(PageID.RewardPage, tPageParam);
    }

    void OnTileStartGame(EventData pEventData)
    {
        //var tEventData = pEventData as TileStartGame;
        //if (tEventData.isNewGame)
        //{
        //    var tLevel = tEventData.gameParams.ToInt();
        //    if (tLevel > 0 && tLevel != CurrentLevel)
        //    {
        //        mTileGameInfo.CurrentLevel = tLevel;
        //    }
        //    StartPlayTime(true, false);
        //    BQManager.ReportTileLevelEntry(mTileGameInfo.RetryCount > 0);
        //}
        //else
        //{
        //    StartPlayTime(false, true);
        //}
    }

    void OnTileGameOver(EventData pEventData)
    {
        //StopPlayTime();
        //var tEventData = pEventData as TileGameOver;
        //var tCurrentLevell = CurrentLevel;
        //var tMaxReachLevel = MaxReachLevel;
        //if (!tEventData.isSuccess)
        //{
        //    LevelFail();
        //    MiniRevivePopupParam tSlotParam = new MiniRevivePopupParam()
        //    {
        //        level = tEventData.level,
        //    };
        //    tSlotParam.clickAction = (isReplay) =>
        //    {
        //        BQManager.ReportTileLevelPass(tCurrentLevell, tEventData.isSuccess, isReplay);
        //    };
        //    PageManager.Instance.OpenPage(PageID.MiniRevivePopup, tSlotParam);
        //}
        //else
        //{
        //    BQManager.ReportTileLevelPass(tCurrentLevell, tEventData.isSuccess, false);
        //    LevelFinish();
        //    MiniGameOverPageParam tParam = new MiniGameOverPageParam
        //    {
        //        isSuccess = tEventData.isSuccess,
        //        level = tEventData.level,
        //        isRewards = tCurrentLevell == tMaxReachLevel,
        //    };
        //    PageManager.Instance.OpenPage(PageID.MiniGameOverPage, tParam);
        //}
        //Serialize();//保持一下防止数据丢失
    }

    #region 临时统计

    Stopwatch sw = new Stopwatch();
    void StartPlayTime(bool pIsInit = false, bool pIsStart = true)
    {
        if (pIsInit)
        {
            mTileGameInfo.PlayTime = 0;
            sw.Reset();
        }

        if (pIsStart)
        {
            sw.Restart();
        }
    }

    void StopPlayTime()
    {
        sw.Stop();

        mTileGameInfo.PlayTime += (int)sw.Elapsed.TotalSeconds;

        sw.Reset();
    }

    public int GetStatisticsTimes(int pLevel, bool pIsReplay)
    {
        if (StatisticsTimes.ContainsKey(pLevel))
        {
            return pIsReplay ? StatisticsTimes[pLevel].replayTimes : StatisticsTimes[pLevel].lostTimes;
        }
        else
        {
            return 0;
        }
    }

    void AddStatisticsTimes(int pLevel, bool pIsReplay)
    {
        if (StatisticsTimes.ContainsKey(pLevel))
        {
            if (pIsReplay)
            {
                StatisticsTimes[pLevel].replayTimes++;
            }
            else
            {
                StatisticsTimes[pLevel].lostTimes++;
            }
        }
        else
        {
            StatisticsTimes.Add(pLevel, new DataTimes(0, 0));
        }
    }

    #endregion

    #region Tile Level 数据
    void InitAllLevelConfig()
    {
        //int tFloor;
        //int tIndex;
        //foreach (var tConfig in ConfigData.tileLevelConfig.DataList)
        //{
        //    if (!allLevelData.TryGetValue(tConfig.level, out TileLevelData tLevelData))
        //    {
        //        tLevelData = new TileLevelData();
        //        tLevelData.level = tConfig.level;
        //        tLevelData.floorsData = new List<FloorData>();
        //        allLevelData.Add(tConfig.level, tLevelData);
        //    }

        //    tFloor = tConfig.floor - 1;
        //    FloorData tFloorData = new FloorData(tFloor);

        //    int4 tLimit;
        //    if (!allLimitData.TryGetValue(tConfig.level, out tLimit))
        //    {
        //        tLimit = new int4(int.MaxValue, int.MinValue, int.MinValue, int.MaxValue);//上，右，下，左
        //        allLimitData.Add(tConfig.level, tLimit);
        //    }

        //    //每层按从上到下的顺序，把数据添加到floorDatas
        //    var cell1Datas = GetCellData(tConfig.cells1);
        //    for (int j = 0; j < cell1Datas.Count; j++)
        //    {
        //        tIndex = TileCellData.GetIndex(0, j);
        //        var tCellData = new TileCellData(tFloor, tIndex, cell1Datas[j]);
        //        if (cell1Datas[j] > 0)
        //            GetLimit(ref tLimit, tIndex);
        //        tFloorData.floorData.Add(tCellData);
        //    }

        //    var cell2Datas = GetCellData(tConfig.cells2);
        //    for (int j = 0; j < cell2Datas.Count; j++)
        //    {
        //        tIndex = TileCellData.GetIndex(1, j);
        //        var tCellData = new TileCellData(tFloor, tIndex, cell2Datas[j]);
        //        if (cell2Datas[j] > 0)
        //            GetLimit(ref tLimit, tIndex);
        //        tFloorData.floorData.Add(tCellData);
        //    }

        //    var cell3Datas = GetCellData(tConfig.cells3);
        //    for (int j = 0; j < cell3Datas.Count; j++)
        //    {
        //        tIndex = TileCellData.GetIndex(2, j);
        //        var tCellData = new TileCellData(tFloor, tIndex, cell3Datas[j]);
        //        if (cell3Datas[j] > 0)
        //            GetLimit(ref tLimit, tIndex);
        //        tFloorData.floorData.Add(tCellData);
        //    }

        //    var cell4Datas = GetCellData(tConfig.cells4);
        //    for (int j = 0; j < cell4Datas.Count; j++)
        //    {
        //        tIndex = TileCellData.GetIndex(3, j);
        //        var tCellData = new TileCellData(tFloor, tIndex, cell4Datas[j]);
        //        if (cell4Datas[j] > 0)
        //            GetLimit(ref tLimit, tIndex);
        //        tFloorData.floorData.Add(tCellData);
        //    }

        //    var cell5Datas = GetCellData(tConfig.cells5);
        //    for (int j = 0; j < cell5Datas.Count; j++)
        //    {
        //        tIndex = TileCellData.GetIndex(4, j);
        //        var tCellData = new TileCellData(tFloor, tIndex, cell5Datas[j]);
        //        if (cell5Datas[j] > 0)
        //            GetLimit(ref tLimit, tIndex);
        //        tFloorData.floorData.Add(tCellData);
        //    }

        //    tFloorData.floorData.Sort((f1, f2) => f1.Index.CompareTo(f2.Index));
        //    tLevelData.floorsData.Add(tFloorData);

        //    allLimitData[tConfig.level] = tLimit;
        //}
    }

    //private void GetLimit(ref int4 tLimit, int tIndex)
    //{
    //    var x = tIndex % 5;
    //    var y = tIndex / 5;

    //    tLimit.x = Mathf.Min(tLimit.x, y);
    //    tLimit.z = Mathf.Max(tLimit.z, y);

    //    tLimit.y = Mathf.Max(tLimit.y, x);
    //    tLimit.w = Mathf.Min(tLimit.w, x);
    //}

    ///// <summary>
    ///// 获取限制范围
    ///// </summary>
    ///// <returns>上，右，下，左</returns>
    //public int4 GetCurrentLimitData()
    //{
    //    return allLimitData[CurrentLevel];
    //}

    //public List<FloorData> GetLevelData(int pLeveIndex)
    //{
    //    //if (!allLevelData.ContainsKey(pLeveIndex))
    //    //{
    //    //    LogManager.LogError($"TileGameModule.GetLevelData:{pLeveIndex}关卡数错误，不存在此关");
    //    //    return new List<FloorData>();
    //    //}

    //    //return allLevelData[pLeveIndex].floorsData;
    //}

    //拆分cell数据
    List<int> GetCellData(string cellDataStr)
    {
        List<int> cellsList = new List<int>();
        string tmpValue = cellDataStr.Replace("{", "");
        tmpValue = tmpValue.Replace("}", "");
        string[] sp = tmpValue.Split(',');
        for (int i = 0; i < sp.Length; i++)
        {
            int index = int.Parse(sp[i]);
            if (index > 0)
            {
                index -= 1000;
            }
            cellsList.Add(index);
        }
        return cellsList;
    }

    public List<List<int>> GetOriginalDataIndex(int levelIndex)
    {
       // var tAllLevelData = allLevelData[levelIndex];
        List<List<int>> result = new List<List<int>>();
        //for (int i = 0; i < tAllLevelData.floorsData.Count; i++)
        //{
        //    List<int> t = new List<int>();
        //    for (int j = 0; j < tAllLevelData.floorsData[i].floorData.Count; j++)
        //    {
        //        if (tAllLevelData.floorsData[i].floorData[j].IconIndex > 0)
        //        {
        //            t.Add(j);
        //        }
        //    }
        //    result.Add(t);
        //}
        return result;
    }

    #endregion

    #region Tile Map 数据
    private void InitMapConfig()
    {
        //BeginLevelId = TileMapConfigs[0].ID;
        //EndLevelId = TileMapConfigs[0].ID;
        //foreach (var config in TileMapConfigs)
        //{
        //    if (config.ID < BeginLevelId)
        //    {
        //        BeginLevelId = config.ID;
        //    }
        //    if (config.ID > EndLevelId)
        //    {
        //        EndLevelId = config.ID;
        //    }
        //}
    }

    //public TileMapConfig GetLevelConfig(int pLevelId)
    //{
    //    foreach (var config in TileMapConfigs)
    //    {
    //        if (config.ID == pLevelId)
    //        {
    //            return config;
    //        }
    //    }
    //    return TileMapConfigs[0];
    //}

    public List<PropData> GetLevelReward(string pRewardStr)
    {
        if (string.IsNullOrEmpty(pRewardStr)) return null;

        var preReward = pRewardStr.Split(';');
        List<PropData> result = new List<PropData>();
        for (int i = 0; i < preReward.Length; i++)
        {
            if (string.IsNullOrEmpty(preReward[i]))
                continue;
            var value = preReward[i].Split(',');
            int tID = int.Parse(value[0]);
            int tCount = int.Parse(value[1]);
            if (Enum.IsDefined(typeof(PropID), tID))
            {
                PropData tData = new PropData((PropID)tID, tCount);
                result.Add(tData);
            }
        }
        return result;
    }

    public void LevelFinish()
    {
        CheckLevelFinish();
        mTileGameInfo.CurrentLevel++;
        mTileGameInfo.RetryCount = 0;
        if (CurrentLevel > MaxReachLevel)
        {
            mTileGameInfo.MaxReachLevel = CurrentLevel;
        }
    }

    public void LevelFail()
    {
        AddStatisticsTimes(CurrentLevel, false);
        mTileGameInfo.CurrentLevel = CurCheckLevel;
    }

    private void CheckLevelFinish()
    {
        //if (GetLevelConfig(CurrentLevel).Checkpoint == 1)
        //{
        //    mTileGameInfo.CurCheckLevel = CurrentLevel;
        //}
    }

    public void AddOpenedGift(int pIndex)
    {
        mTileGameInfo.OpenedGifts.Add(pIndex);
    }

    #endregion

    public void RefreshTileData()
    {
        ////临时循环4期
        //var tIssueNum = (mTileGameInfo.IssueNum + 3) % 4 + 1;
        //ConfigData.tileMapConfig.LoadData(GetRootPath(tIssueNum));
        //ConfigData.tileLevelConfig.LoadData(GetRootPath(tIssueNum));
        //// mTileMapConfigs = ConfigData.tileMapConfig.DataList;
        //// mTileLevelConfigs = ConfigData.tileLevelConfig.DataList;
        //allLevelData = new Dictionary<int, TileLevelData>();
        //allLimitData = new Dictionary<int, int4>();
        //InitMapConfig();
        //InitAllLevelConfig();
    }

    public void DefaultProp()
    {
        ////临时初始生命体力
        //if (!ModuleManager.Prop.HasProp(PropID.Gold))
        //{
        //    ModuleManager.Prop.AddProp(PropID.Gold, CommonDefine.tileExchangeStaminaGoldCount, PropSource.Unknown);
        //}
        ////临时初始生命体力
        //if (!ModuleManager.Prop.HasProp(PropID.TileHeart))
        //{
        //    ModuleManager.Prop.AddProp(PropID.TileHeart, CommonDefine.tileFullStaminaCount, PropSource.Unknown);
        //}
    }

    public void BeginNewIssue()
    {
        //IsFailIng = false;
        //StatisticsTimes = new Dictionary<int, DataTimes>();
        //mTileInfo.IssueNum = mTileGameInfo.IssueNum + 1;
        //mTileGameInfo = new TileGameInfo
        //{
        //    IssueNum = mTileInfo.IssueNum,
        //    BeginTime = TimeTool.Now.Ticks,
        //    EndTime = TimeTool.Now.AddDays(ACTIVITY_PERIOD_DAYS).Ticks
        //};

        //RefreshTileData();
        //if (mTileInfo.IssueNum == 1)
        //{
        //    DefaultProp();
        //}
    }

    public bool CheckValid()
    {
        if (IsUnderway())
        {
            //当期进行中
            return true;
        }
        else
        {
            //TODO:检查配置是否存在
            var tIsExist = true;
            return tIsExist;
        }
    }

    public bool IsUnderway()
    {
        //var tCurTime = TimeTool.Now.Ticks;
        ////是否进行中
        //if (tCurTime >= mTileGameInfo.BeginTime && tCurTime < mTileGameInfo.EndTime)
        //{
        //    return true;
        //}
        //else
        //{
            return false;
        //}
    }

    public DateTime GetEndTime()
    {
        return new DateTime(mTileGameInfo.EndTime);
    }

    public void TileRetryGame()
    {
        AddStatisticsTimes(CurrentLevel, true);
        mTileGameInfo.RetryCount += 1;
    }

    //获取重玩关卡金币
    public int GetLevelReplayCoin()
    {
        //if (CurrentLevel <= TileMapConfigs.Count)
        //{
        //    return TileMapConfigs[CurrentLevel - 1].ReplayCoin;
        //}
        return 0;
    }

    public bool IsHarvestHeart()
    {
        //if (mTileInfo.HeartRecoverTime != 0 && TimeTool.Now.Ticks >= mTileInfo.HeartRecoverTime)
        //{
        //    return true;
        //}
         return false;
    }

    public void HarvestHeartRecover()
    {
        //int tCount = (int)(new DateTime(TimeTool.Now.Ticks) - new DateTime(mTileInfo.HeartRecoverTime)).TotalMinutes / CommonDefine.tileRecoverTimeInterval;
        //var tCurHeartCount = ModuleManager.Prop.GetPropCount(PropID.TileHeart);
        //int tAddCount;
        //if (tCount + 1 + tCurHeartCount > CommonDefine.tileFullStaminaCount)
        //{
        //    tAddCount = CommonDefine.tileFullStaminaCount - tCurHeartCount;
        //}
        //else
        //{
        //    tAddCount = tCount + 1;
        //}
        //mTileInfo.HeartRecoverTime = 0;
        //ModuleManager.Prop.AddProp(PropID.TileHeart, tAddCount, PropSource.TileHeartRecover);
    }

    public void ClearHeartRecoverTime()
    {
        mTileInfo.HeartRecoverTime = 0;
    }

    public void SetHeartRecoverTime()
    {
        //if (TimeTool.Now.Ticks >= mTileInfo.HeartRecoverTime)
        //{
        //    mTileInfo.HeartRecoverTime = TimeTool.Now.AddMinutes(CommonDefine.tileRecoverTimeInterval).Ticks;
        //}
    }
    public DateTime GetHeartRecoverTime()
    {
        return new DateTime(mTileInfo.HeartRecoverTime);
    }

    //bool CheckTileGameOpen()
    //{
    //    bool tIsOpen = false;
    //    var tIssueNum = 1;
    //    DateTime curTime = TimeTool.Now;
    //    DateTime startTime = TimeTool.defaultTime, endTime = TimeTool.defaultTime;
    //    foreach (var tConfig in ConfigData.tileOpenConfig.DataList)
    //    {
    //        startTime = Convert.ToDateTime(tConfig.openTime);
    //        endTime = Convert.ToDateTime(tConfig.endTime);
    //        if (curTime >= startTime && curTime < endTime)
    //        {
    //            tIssueNum = tConfig.ID;
    //            tIsOpen = true;
    //            break;
    //        }
    //    }

    //    if (tIsOpen)
    //    {
    //        mTileGameInfo.IssueNum = tIssueNum;
    //        mTileGameInfo.BeginTime = startTime.Ticks;
    //        mTileGameInfo.EndTime = endTime.Ticks;
    //    }

    //    return tIsOpen;
    //}

    #region 资源处理

    string GetRootPath(int pIssueNum)
    {
        return $"{TILE_ROOT_PATH}{pIssueNum}";
    }

    string GetAssetsPath(int pIssueNum, string pAssetsName)
    {
        return $"{TILE_ROOT_PATH}{pIssueNum}/{pAssetsName}";
    }

    //string GetTileLevelConfigPath(int pIssueNum) => GetAssetsPath(pIssueNum, "TileLevelConfig");
    // string GetTileMapConfigPath(int pIssueNum) => GetAssetsPath(pIssueNum, "TileMapConfig");

    #endregion

    #region 序列化
    void Serialize()
    {
        //DataTool.Serialize(DataKey.ACTIVIT_TILE_INFO, mTileInfo);
        //DataTool.Serialize(DataKey.ACTIVIT_TILE_GAME_INFO, mTileGameInfo);
        //DataTool.Serialize(STATISTICS_DATA_KEY, StatisticsTimes);
    }

    void Deserialize()
    {
        //mTileInfo = DataTool.Deserialize<TileInfo>(DataKey.ACTIVIT_TILE_INFO);
        //mTileGameInfo = DataTool.Deserialize<TileGameInfo>(DataKey.ACTIVIT_TILE_GAME_INFO);
        //StatisticsTimes = DataTool.Deserialize<Dictionary<int, DataTimes>>(STATISTICS_DATA_KEY);
    }


    #endregion

    #region Editor
    public void ChangeCurrentLevel(int plevel)
    {
        //if (plevel < 1 || plevel > TileMapConfigs.Count)
        //{
        //    LogManager.LogError("关卡id 越界");
        //    return;
        //}
        //mTileGameInfo.CurrentLevel = plevel;
        //mTileGameInfo.MaxReachLevel = plevel;
        //mTileGameInfo.RetryCount = 0;
        //for (int i = plevel - 2; i >= 0; i--)
        //{
        //    if (TileMapConfigs[i].Checkpoint == 1)
        //    {
        //        mTileGameInfo.CurCheckLevel = i + 1;
        //        break;
        //    }
        //}
        PlayerPrefs.SetString("ArchiveKey_TileGame", string.Empty);
    }

    public void ChangeCurrentIssue(int pIssue)
    {
        //mTileInfo.IssueNum = pIssue;
        //mTileGameInfo = new TileGameInfo
        //{
        //    IssueNum = mTileInfo.IssueNum,
        //    BeginTime = TimeTool.Now.Ticks,
        //    EndTime = TimeTool.Now.AddDays(ACTIVITY_PERIOD_DAYS).Ticks
        //};
        //PlayerPrefs.SetString("ArchiveKey_TileGame", string.Empty);

        //RefreshTileData();
        //DefaultProp();
    }
    #endregion
}
