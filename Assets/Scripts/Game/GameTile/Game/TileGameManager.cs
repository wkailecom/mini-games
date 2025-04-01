using Config;
using Game.UISystem;
using LLFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGameManager : Singleton<TileGameManager>
{
    public bool GameStart { get; private set; }
    public bool HasArchive => (mGameMode != null ? mGameMode : new TileGameMode()).HasArchive;
    public TileGameMode GameMode => mGameMode;
    public TileGameBaseData BaseData { get; private set; }
    public FloorViewData FloorViewData { get; private set; }
    public SlotViewData SlotViewData { get; private set; }

    TileGameMode mGameMode;

    public void Init()
    {
        BaseData = new TileGameBaseData();
        BaseData.Init();
        FloorViewData = new FloorViewData();
        FloorViewData.Init();
        SlotViewData = new SlotViewData();
        SlotViewData.Init();

        //EventManager.Register(EventDataType.ApplicationFocus, OnApplicationFocus);
        //EventManager.Register(EventDataType.TileCellClick, OnTileCellClick);
    }

    void OnApplicationFocus(EventData pEventData)
    {
        var tEventData = pEventData as ApplicationFocus;
        if (mGameMode != null && !tEventData.focus)
        {
            if (GameStart)
            {
                mGameMode.Archive();
            }
        }
    }

    void OnTileCellClick(EventData pEventData)
    {
        //var tEventData = pEventData as TileCellClick;

        // if (!IsCanPlace()) return;
        //TryFinishGame();
    }

    public bool StartGame()
    {
        var tLevel =  "";//ModuleManager.TileGame.CurrentLevel.ToString();
        if (HasArchive)
        {
            return ContinueGame(tLevel);
        }
        else
        {
            return StartNewGame(tLevel);
        }
    }

    public bool ContinueGame(string pGameParams = null)
    {
        SetGameMode(false, pGameParams);

        if (!mGameMode.StartGame(false, pGameParams)) return false;

        GameStart = true;

        TriggerStartGameEvent(false, pGameParams);
        CheckGameStatus();
        return true;
    }

    public bool StartNewGame(string pGameParams = null)
    {
        SetGameMode(true, pGameParams);

        if (!mGameMode.StartGame(true, pGameParams)) return false;

        GameStart = true;

        TriggerStartGameEvent(true, pGameParams);
        return true;
    }

    public void FinishGame(bool pSuccess)
    {
        //GameStart = false;
        //int cacheLevel = ModuleManager.TileGame.CurrentLevel;
        //mGameMode.FinishGame();
        //var tEventData = EventManager.GetEventData(EventDataType.TileGameOver) as TileGameOver;
        //tEventData.isSuccess = pSuccess;
        //tEventData.level = cacheLevel;

        //EventManager.TriggerEvent(tEventData);
    }

    public void CheckGameStatus()
    {
        if (!GameStart) return;

        if (TryFinishGame())
        {
            return;
        }
    }

    void SetGameMode(bool pIsNewGame, string pGameParams)
    {
        if (mGameMode == null)
        {
            mGameMode = new TileGameMode();
            if (pIsNewGame)
            {
                Reset();
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
                Reset();
                mGameMode.Reset();
            }
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

    void Reset()
    {
        FloorViewData.Reset();
        SlotViewData.Reset();
        BaseData.Reset();
    }

    void TriggerStartGameEvent(bool pIsNewGame, string pGameParams)
    {
        //mGameMode?.Archive();
        ////mGameParams = pGameParams;
        //PageManager.Instance.OpenPage(PageID.TileGamePage);

        //LogManager.Log($"当前游戏羊羊羊，当前关卡：{pGameParams}");

        //var tEventData = EventManager.GetEventData(EventDataType.TileStartGame) as TileStartGame;
        //tEventData.isNewGame = pIsNewGame;
        //tEventData.gameParams = pGameParams;
        //EventManager.TriggerEvent(tEventData);
    }
}
