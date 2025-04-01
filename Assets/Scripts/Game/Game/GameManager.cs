using Config;
using Game.UI;
using Game.UISystem;
using LLFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameModeType CurrentGameModeType => mGameMode?.ModeType ?? GameModeType.Endless;
    public GameMode CurrentGameMode => mGameMode;
    public bool GameStart { get; private set; }
    public GameBaseData BaseData { get; private set; }

    GameMode mGameMode;
    GameModeType mCacheGameModeType;
    bool mCacheIsNewGame;
    int mCacheLevelID;
    public void Init()
    {
        BaseData = new GameBaseData();
        BaseData.Init();

        EventManager.Register(EventKey.ApplicationFocus, OnApplicationFocus);
        EventManager.Register(EventKey.PageClosed, OnPageClosed);
    }

    void OnApplicationFocus(EventData pEventData)
    {
        var tEventData = pEventData as ApplicationFocus;
        if (GameStart && !tEventData.focus)
        {
            mGameMode?.Archive();
        }
    }

    void OnPageClosed(EventData pEventData)
    {
        var tEventData = pEventData as PageOperation;
        if (GameStart && tEventData.pageID == PageID.GamePage)
        {
            mGameMode?.Archive();
        }
    }

    void OnSymbolFill(EventData pEventData)
    {
        CheckGamStatus();
    }

    void CheckGamStatus()
    {
        if (TryFinishGame())
        {
            return;
        }
    }

    bool TryFinishGame()
    {
        if (mGameMode.IsGameSuccess())
        {
            FinishGame(true);
            return true;
        }

        if (mGameMode.IsGameFailed())
        {
            FinishGame(false);
            return true;
        }

        return false;
    }

    public void CheckGamDeath()
    {
        
    }

    public void SetGameState(bool pIsStart = false)
    {
        GameStart = pIsStart;
    }

    public void FinishGame(bool pSuccess)
    {
        mGameMode.FinishGame();
        GameStart = false;
        var tEventData = EventManager.GetEventData<GameOver>(EventKey.GameOver);
        tEventData.gameModeType = mGameMode.ModeType;
        tEventData.levelID = mCacheLevelID;
        tEventData.isSuccess = pSuccess;
        EventManager.Trigger(tEventData);
    }

    public void RetryGame()
    {
        StartGame(mCacheGameModeType, mCacheLevelID, true);
    }

    public void StartGame(GameModeType pModeType, int pLeveId)
    {
        var tGameMode = GameModeFactory.Instance.GetGameMode(pModeType);
        if (tGameMode.HasArchive)
        {
            StartGame(pModeType, pLeveId, false);
        }
        else
        {
            StartGame(pModeType, pLeveId, true);
        }
    }

    public void StartNewGame(GameModeType pModeType, int pLeveId)
    {
        StartGame(pModeType, pLeveId, true);
    }

    void StartGame(GameModeType pModeType, int pLevelID, bool pIsNewGame = false)
    {
        mCacheGameModeType = pModeType;
        mCacheIsNewGame = pIsNewGame;
        mCacheLevelID = pLevelID;

        if (PageManager.Instance.HasCreate(PageID.GamePage))
        {
            PageManager.Instance.OpenPage(PageID.GamePage);
            TryStartGame(mCacheGameModeType, mCacheLevelID, mCacheIsNewGame);
        }
        else
        {
            EventManager.Register(EventKey.PageBeginOpen, OnPageBeginOpen);
            PageManager.Instance.OpenPage(PageID.GamePage);
        }
    }

    void OnPageBeginOpen(EventData pEventData)
    {
        var tEventData = pEventData as PageOperation;
        if (tEventData.pageID == PageID.GamePage)
        {
            EventManager.Unregister(EventKey.PageBeginOpen, OnPageBeginOpen);
            TryStartGame(mCacheGameModeType, mCacheLevelID, mCacheIsNewGame);
        }
    }

    void TryStartGame(GameModeType pModeType, int pLevelID, bool pIsNewGame = false)
    {
        SetGameMode(pModeType, pIsNewGame);

        mGameMode.StartGame(pLevelID);
        GameStart = true;
        var tEventData = EventManager.GetEventData<GameStart>(EventKey.GameStart);
        tEventData.gameModeType = pModeType;
        tEventData.isNewGame = pIsNewGame;
        tEventData.levelID = pLevelID;
        EventManager.Trigger(tEventData);

        if (!pIsNewGame)
        {
            CheckGamDeath();
        }
    }

    void SetGameMode(GameModeType pModeType, bool pIsNewGame)
    {
        if (mGameMode == null || mGameMode.ModeType != pModeType)
        {
            if (GameStart)
            {
                mGameMode?.Archive();
                mGameMode?.ExitMode();
            }

            Reset();
            mGameMode = GameModeFactory.Instance.GetGameMode(pModeType);
            mGameMode.EnterMode();

            if (pIsNewGame)
            {
                mGameMode.Reset();
            }
            else
            {
                mGameMode.LoadArchive();
            }
        }
        else
        {
            if (pIsNewGame)
            {
                mGameMode.ExitMode();

                Reset();
                mGameMode.EnterMode();
                mGameMode.Reset();
            }
            else
            {
                //Daily模式切换不同时间
                if (mGameMode.ModeType == GameModeType.Daily && (mGameMode as GameMode_Daily).LevelId != mCacheLevelID)
                {
                    if (GameStart)
                    {
                        mGameMode?.Archive();
                        mGameMode?.ExitMode();
                    }

                    Reset();
                    mGameMode.EnterMode();
                    mGameMode.LoadArchive();
                }
            }
        }
    }

    void Reset()
    {
        BaseData.Reset(); 
    }

}
