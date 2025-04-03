using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsModule : ModuleBase
{
    const string STATISTICS_LAST_TIME_KEY = "Statistics_LastTimeKey";
    const string STATISTICS_TOTAL_KEY = "Statistics_TotalKey";
    const string STATISTICS_TOTAL_DAY_KEY = "Statistics_TotalSameDayKey";
    const string STATISTICS_TYPE_KEY = "Statistics_GameTypeKey";
    const string STATISTICS_TYPE_DAY_KEY = "Statistics_GameTypeSameDayKey";

    Dictionary<string, int> mTotalData;
    Dictionary<string, int> mTotalSameDayData;
    Dictionary<GameModeType, Dictionary<string, int>> mTypeData;
    Dictionary<GameModeType, Dictionary<string, int>> mTypeSameDayData;

    DateTime mLastRecordTime;

    #region 统计数据

    public int GetValue(StatsID pKey, StatsGroup pGroup)
    {
        return GetValue(pKey, pGroup, GameManager.Instance.CurrentGameModeType);
    }

    public void SetValue(StatsID pKey, int pValue, StatsGroup pGroup)
    {
        SetValue(pKey, pValue, pGroup, GameManager.Instance.CurrentGameModeType);
    }

    public int AddValue(StatsID pKey, StatsGroup pGroup, int pValue = 1)
    {
        return AddValue(pKey, pGroup, GameManager.Instance.CurrentGameModeType);
    }

    public int GetValue(StatsID pKey, StatsGroup pGroup, GameModeType pGameType)
    {
        return pGroup switch
        {
            StatsGroup.Total => GetValue(pKey.ToString(), false),
            StatsGroup.TotalDay => GetValue(pKey.ToString(), true),
            StatsGroup.Type => GetValue(pGameType, pKey.ToString(), false),
            StatsGroup.TypeDay => GetValue(pGameType, pKey.ToString(), true),
            _ => GetValue(pKey.ToString(), true),
        };
    }

    public void SetValue(StatsID pKey, int pValue, StatsGroup pGroup, GameModeType pGameType)
    {
        switch (pGroup)
        {
            case StatsGroup.Total:
                SetValue(pKey.ToString(), pValue, false);
                break;
            case StatsGroup.TotalDay:
                SetValue(pKey.ToString(), pValue, true);
                break;
            case StatsGroup.Type:
                SetValue(pGameType, pKey.ToString(), pValue, false);
                break;
            case StatsGroup.TypeDay:
                SetValue(pGameType, pKey.ToString(), pValue, true);
                break;
        }
    }

    public int AddValue(StatsID pKey, StatsGroup pGroup, GameModeType pGameType, int pValue = 1)
    {
        return pGroup switch
        {
            StatsGroup.Total => AddValue(pKey.ToString(), false, pValue),
            StatsGroup.TotalDay => AddValue(pKey.ToString(), true, pValue),
            StatsGroup.Type => AddValue(pGameType, pKey.ToString(), false, pValue),
            StatsGroup.TypeDay => AddValue(pGameType, pKey.ToString(), true, pValue),
            _ => 0
        };
    }

    public int GetValue(GameModeType pGameType, string pKey, bool pIsDay)
    {
        var tGameData = pIsDay ? mTypeSameDayData : mTypeData;
        if (tGameData.ContainsKey(pGameType))
        {
            return tGameData[pGameType].GetValue(pKey);
        }
        return 0;
    }

    public int GetValue(string pKey, bool pIsDay)
    {
        var tGameData = pIsDay ? mTotalSameDayData : mTotalData;
        return tGameData.GetValue(pKey);
    }

    public void SetValue(GameModeType pGameType, string pKey, int pValue, bool pIsDay)
    {
        var tGameData = pIsDay ? mTypeSameDayData : mTypeData;
        if (tGameData.ContainsKey(pGameType))
        {
            tGameData[pGameType].SetValue(pKey, pValue);
        }
        else
        {
            tGameData.Add(pGameType, new Dictionary<string, int>() { { pKey, pValue } });
        }
    }

    public void SetValue(string pKey, int pValue, bool pIsDay)
    {
        var tGameData = pIsDay ? mTotalSameDayData : mTotalData;
        tGameData.SetValue(pKey, pValue);
    }

    public int AddValue(GameModeType pGameType, string pKey, bool pIsDay, int pValue = 1)
    {
        var tGameData = pIsDay ? mTypeSameDayData : mTypeData;
        if (tGameData.ContainsKey(pGameType))
        {
            return tGameData[pGameType].AddValue(pKey, pValue);
        }
        else
        {
            tGameData.Add(pGameType, new Dictionary<string, int>() { { pKey, pValue } });
            return pValue;
        }
    }

    public int AddValue(string pKey, bool pEveryday, int pValue = 1)
    {
        var tGameData = pEveryday ? mTotalSameDayData : mTotalData;
        return tGameData.AddValue(pKey, pValue);
    }

    #endregion

    protected override void OnInit()
    {
        base.OnInit();
        Deserialize();

        EventManager.Register(EventKey.ApplicationFocus, OnApplicationFocus);
        EventManager.Register(EventKey.StartNewDay, OnStartNewDay);

        EventManager.Register(EventKey.GameStart, OnGameStart);
        EventManager.Register(EventKey.GameOver, OnGameOver);
        EventManager.Register(EventKey.PropCountChange, OnPropCountChange);
    }

    protected override void OnUninit()
    {
        base.OnUninit();
        Serialize();

        EventManager.Unregister(EventKey.ApplicationFocus, OnApplicationFocus);
        EventManager.Unregister(EventKey.StartNewDay, OnStartNewDay);

        EventManager.Unregister(EventKey.GameStart, OnGameStart);
        EventManager.Unregister(EventKey.GameOver, OnGameOver);
        EventManager.Unregister(EventKey.PropCountChange, OnPropCountChange);
    }


    public void AddPlayTime(int pTime)
    {
        mTotalData.AddValue(StatsID.PlayTime.ToString(), pTime);
        mTotalSameDayData.AddValue(StatsID.PlayTime.ToString(), pTime);
    }

    void OnApplicationFocus(EventData pEventData)
    {
        var tEventData = pEventData as ApplicationFocus;
        if (!tEventData.focus)
        {
            Serialize();
        }
    }

    void OnStartNewDay(EventData pEventData)
    {
        CheckNewDay();
        Serialize();
    }

    void CheckNewDay()
    {
        if (mLastRecordTime.Date != DateTime.Now.Date)
        {
            mLastRecordTime = DateTime.Now;
            AddValue(StatsID.LoginDays.ToString(), false);

            //新一天清空
            mTotalSameDayData.Clear();
            foreach (var item in mTypeSameDayData.Values)
            {
                item.Clear();
            }
        }
    }

    void OnGameStart(EventData pEventData)
    {
        var tEventData = pEventData as GameStart;
        var tGameType = tEventData.gameModeType;
        if (!tEventData.isNewGame || tGameType == GameModeType.Guide) return;

        AddValue(StatsID.PlayTimes, StatsGroup.Total);
        AddValue(StatsID.PlayTimes, StatsGroup.TotalDay);
        AddValue(StatsID.PlayTimes, StatsGroup.Type, tGameType);
        AddValue(StatsID.PlayTimes, StatsGroup.TypeDay, tGameType);
    }

    void OnGameOver(EventData pEventData)
    {
        var tEventData = pEventData as GameOver;
        var tGameType = tEventData.gameModeType;
        if (!tEventData.isSuccess || tGameType == GameModeType.Guide) return;

        AddValue(StatsID.OverTimes, StatsGroup.Total);
        AddValue(StatsID.OverTimes, StatsGroup.TotalDay);
        AddValue(StatsID.OverTimes, StatsGroup.Type, tGameType);
        AddValue(StatsID.OverTimes, StatsGroup.TypeDay, tGameType);

        Serialize();
    }

    void OnPropCountChange(EventData pEventData)
    {
        var tEventData = pEventData as PropCountChange;
        if (tEventData.changedCount < 0)
        {
 
        }
    }


    #region 序列化 & 反序列化
    void Serialize()
    {
        DataTool.SetString(STATISTICS_LAST_TIME_KEY, mLastRecordTime.Ticks.ToString());
        DataTool.Serialize(STATISTICS_TOTAL_KEY, mTotalData);
        DataTool.Serialize(STATISTICS_TOTAL_DAY_KEY, mTotalSameDayData);
        DataTool.Serialize(STATISTICS_TYPE_KEY, mTypeData);
        DataTool.Serialize(STATISTICS_TYPE_DAY_KEY, mTypeSameDayData);
    }

    void Deserialize()
    {
        mLastRecordTime = new DateTime(DataTool.GetString(STATISTICS_LAST_TIME_KEY, DateTime.Now.Ticks.ToString()).ToLong());
        mTotalData = DataTool.Deserialize<Dictionary<string, int>>(STATISTICS_TOTAL_KEY);
        mTypeData = DataTool.Deserialize<Dictionary<GameModeType, Dictionary<string, int>>>(STATISTICS_TYPE_KEY);

        if (mLastRecordTime.Date != DateTime.Now.Date)
        {
            mTotalSameDayData = new Dictionary<string, int>();
            mTypeSameDayData = new Dictionary<GameModeType, Dictionary<string, int>>();
            mLastRecordTime = DateTime.Now;
            AddValue(StatsID.LoginDays, StatsGroup.Total);
        }
        else
        {
            mTotalSameDayData = DataTool.Deserialize<Dictionary<string, int>>(STATISTICS_TOTAL_DAY_KEY);
            mTypeSameDayData = DataTool.Deserialize<Dictionary<GameModeType, Dictionary<string, int>>>(STATISTICS_TYPE_DAY_KEY);
        }
    }

    #endregion
}

public enum StatsGroup
{
    Total,
    TotalDay,
    Type,
    TypeDay
}

public enum StatsID
{
    LoginDays,         //登录天数
    ADVideo,           //激励广告数
    ADInterstitial,    //插屏广告数

    PlayTimes,         //游戏累计局数
    OverTimes,         //游戏结算局数 
    ReviveTimes,       //游戏复活次数 
    PlayTime,          //游戏用时时间
    BestPlayTime,      //最佳用时时间
    WinStreak,         //最大连胜数


    ADHealthTimes,     //观看激励获取体力次数 

}