using Config;
using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAnalyseModule : ModuleBase
{
    public int LevelNum;                       //当前关卡
    public int RetryCount;                     //当前次数 
    public int ReviveTimes;                    //使用复活次数  

    protected override void OnInit()
    {
        base.OnInit();

        EventManager.Register(EventKey.SwitchUserGroup, OnSwitchUserGroup);
        EventManager.Register(EventKey.GameStart, OnGameStart);
        EventManager.Register(EventKey.PageBeginOpen, OnPageBeginOpen);
        EventManager.Register(EventKey.PageClosed, OnPageClosed);
        Reset();

        OnSwitchUserGroup(null);
    }

    protected override void OnUninit()
    {
        base.OnUninit();

        EventManager.Unregister(EventKey.SwitchUserGroup, OnSwitchUserGroup);
        EventManager.Unregister(EventKey.GameStart, OnGameStart);
        EventManager.Unregister(EventKey.PageBeginOpen, OnPageBeginOpen);
    }

    void Reset()
    { 
        ReviveTimes = 0; 
    }

    void OnGameStart(EventData pEventData)
    {
        var tEventData = pEventData as GameStart;

        Reset();

    }

    void OnSwitchUserGroup(EventData pEventData)
    {
        if (AppInfoManager.Instance.UserGroup == UserGroup.GroupA)
        {
           
        }
        else
        { 

        }
    }

    void OnPageBeginOpen(EventData pEventData)
    {
        var tEventData = pEventData as PageOperation;
        if (tEventData.pageID == PageID.MiniMapPage)
        {
            GameVariable.CurSceneType = GameSceneType.MiniGame;
        }
        else if (tEventData.pageID == PageID.HomePage)
        {
            GameVariable.CurSceneType = GameSceneType.WordGame;
        }
    }

    void OnPageClosed(EventData pEventData)
    {
        var tEventData = pEventData as PageOperation;

    }
}
