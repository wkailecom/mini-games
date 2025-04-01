using Config;
using Game;
using Game.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

public class GameBaseData
{
    public long UniqueID { get; private set; }       //唯一标识，取开始时间
    public int TakeTime { get; private set; }        //用时(秒)

    public void Init()
    {
        EventManager.Register(EventKey.GameStart, OnGameStart);
        EventManager.Register(EventKey.GameOver, OnGameOver, true);
        EventManager.Register(EventKey.PageClosed, OnPageClosed);
        EventManager.Register(EventKey.ApplicationFocus, OnApplicationFocus); 
    }


    void OnGameStart(EventData pEventData)
    {
        var tEventData = pEventData as GameStart;
        if (tEventData.isNewGame)
        {
            UniqueID = DateTime.Now.Ticks;
            StartPlayTime(true, false);
        }
        else
        {
            StartPlayTime(false, true);
        }
    }

    void OnGameOver(EventData pEventData)
    {
        var tEventData = pEventData as GameOver;

        StopPlayTime();
    }

    void OnPageClosed(EventData pEventData)
    {
        var tEventData = pEventData as PageOperation;
        if (tEventData.pageID == PageID.GamePage)
        {
            StopPlayTime();
        }
    }

    void OnApplicationFocus(EventData pEventData)
    {
        var tEventData = pEventData as ApplicationFocus;
        if (!tEventData.focus)
        {
            StopPlayTime();
        }
        else
        {
            bool tIsStart = GameManager.Instance.GameStart && PageManager.Instance.GetGamePage().IsOpen;
            StartPlayTime(false, tIsStart);
        }
    }

    #region 用时统计

    readonly Stopwatch sw = new();
    void StartPlayTime(bool pIsInit = false, bool pIsStart = true)
    {
        if (pIsInit)
        {
            TakeTime = 0;
            sw.Restart();
        }

        if (pIsStart)
        {
            sw.Restart();
        }
    }
    void StopPlayTime()
    {
        sw.Stop();

        TakeTime += (int)sw.Elapsed.TotalSeconds;
        ModuleManager.Statistics.AddPlayTime((int)sw.Elapsed.TotalSeconds);
        sw.Reset();
    }

    #endregion


    public void Reset()
    {
        TakeTime = 0;
    }

    #region 序列化 & 反序列化

    private const string SEPARATOR = ",";

    public string Serialize()
    {
        StringBuilder tStrBuilder = new StringBuilder();
        tStrBuilder.Append(UniqueID).Append(SEPARATOR);
        tStrBuilder.Append(TakeTime).Append(SEPARATOR);

        return tStrBuilder.ToString();
    }

    public void Deserialize(string pSerializedString)
    {
        Reset();
        if (string.IsNullOrEmpty(pSerializedString))
        {
            return;
        }

        var tSpStr = pSerializedString.Split(SEPARATOR);
        //if (tSpStr.Length < 11)
        //{
        //    LogManager.LogError($"GameBaseData.Deserialize: Invalid string {pSerializedString}");
        //    return;
        //}

        if (tSpStr.Length > 0) UniqueID = tSpStr[0].ToLong();
        if (tSpStr.Length > 1) TakeTime = tSpStr[1].ToInt();
    }
    #endregion


}
