using Config;
using Game;
using Game.UI;
using Game.UISystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GuideModule : ModuleBase
{
    const string GUIDE_COINFIG_PATH = "GuideConfig";

    protected override void OnInit()
    {
        base.OnInit();

        EventManager.Register(EventKey.PageOpened, OnPageOpened);
        EventManager.Register(EventKey.GameStart, OnGameStart);

    }

    protected override void OnUninit()
    {
        base.OnUninit();

        EventManager.Unregister(EventKey.PageOpened, OnPageOpened);
        EventManager.Unregister(EventKey.GameStart, OnGameStart);
    }

    void OnPageOpened(EventData pEventData)
    {
        var tEventData = pEventData as PageOperation;
        if (tEventData.pageID == PageID.HomePage)
        {
            
        }
    }

    void OnGameStart(EventData pEventData)
    {
        var tEventData = pEventData as GameStart;
        if (tEventData.gameModeType == GameModeType.Endless)
        {
            
        }
    }

}