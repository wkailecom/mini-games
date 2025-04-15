using Config;
using Game;
using Game.MiniGame;
using Game.UI;
using Game.UISystem;
using LLFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MiniGameManager : Singleton<MiniGameManager>
{
    public bool GameStart { get; private set; }
    public int Level => mCacheLevel;
    public MiniGameType GameType => mCacheType;

    int mCacheLevel;
    MiniGameType mCacheType;

    public void Init()
    {
        mCacheType = MiniGameType.Invalid;
        mCacheLevel = 1;
    }

    public void InitEvent(MiniGameType pGameType)
    {
        if (pGameType == MiniGameType.Screw)
        {
            RegisterScrew();
        }
        else if (pGameType == MiniGameType.Jam3d)
        {
            RegisterJam3D();
        }
        else if (pGameType == MiniGameType.Tile)
        {
            RegisterTile();
        }
        else if (pGameType == MiniGameType.Bus)
        {
            RegisterBusOut();
        }
        else if (pGameType == MiniGameType.Triple)
        {
            RegisterTriple();
        }
    }

    #region 注册事件
    public void RegisterScrew()
    {
        ScrewJam.ScrewTriggerEvents screwEventTrigger = new ScrewJam.ScrewTriggerEvents(
            () =>
            {
                //游戏成功
                TriggerEventGameOver(MiniGameType.Screw, true);
            },
            () =>
            {
                //游戏失败
                TriggerEventGameOver(MiniGameType.Screw, false);
            },
            () =>
            {
                //使用ExtraSlot
            },
            () =>
            {
                //点击Hammer
            },
            () =>
            {
                //取消点击Hammer
            },
            () =>
            {
                //使用ExtraBox
            },
            () =>
            {
                //使用道具1成功
                TriggerEventUsePropComplete(MiniGameType.Screw, PropID.ScrewExtraSlot);
            },
            () =>
            {
                //使用道具2成功
                TriggerEventUsePropComplete(MiniGameType.Screw, PropID.ScrewHammer);
            },
            () =>
            {
                //使用道具3成功
                TriggerEventUsePropComplete(MiniGameType.Screw, PropID.ScrewExtraBox);
            },
            () =>
            {
                //重试
            },
            (soundName) =>
            {
                //播放声音
                PlaySound(soundName);
            });
        ScrewJam.EventManager.Instance.RegisterTriggerEvents(screwEventTrigger);
    }

    public void RegisterJam3D()
    {
        GameLogic.JamTriggerEvents jamEventTrigger = new GameLogic.JamTriggerEvents(
            () =>
            {
                //开始游戏之后;
            },
            (level) =>
            {
                //游戏成功
                TriggerEventGameOver(MiniGameType.Jam3d, true);
            },
            (level) =>
            {
                //小节成功
                EventManager.Trigger(EventKey.MiniGameSubSuccess);
            },
            (level) =>
            {
                //游戏失败
                TriggerEventGameOver(MiniGameType.Jam3d, false);
            },
            (soundName) =>
            {
                //播放音效
                PlaySound(soundName);
            },
            () =>
            {
                UnloadScene(MiniGameType.Jam3d.ToString());
            },
            (name, parent) =>
            {
                var levelGroup = ConfigData.jamLevelGroupConfig.GetByPrimary(name);
                var levelGo = AssetManager.Instance.LoadPrefab($"PackageJam3d/BundleResources/Prefabs/Gen/Level_{levelGroup.prefab}", parent);
                return levelGo;
            },
            (levelGroupId) =>
            {
                var levelGroup = ConfigData.jamLevelGroupConfig.GetByPrimary(levelGroupId);
                return levelGroup.stepsExchange;
            },
            (name, resType) =>
            {
                if (resType == GameLogic.JamTriggerEvents.ResType.TerrainConfig)
                {
                    var levelGroup = ConfigData.jamLevelGroupConfig.GetByPrimary(name);
                    var res = ResTool.Load<UnityEngine.Object>($"PackageJam3d/BundleResources/Terrains/Terrain_{levelGroup.prefab}");
                    return res;
                }
                return null;
            }, (pathInPackage) =>
            {
                var res = ResTool.Load<UnityEngine.Object>($"PackageJam3d/{pathInPackage}");
                return res;
            });
        GameLogic.JamManager.GetSingleton().RegisterJamTriggerEvents(jamEventTrigger);
    }

    public void RegisterTile()
    {

    }

    public void RegisterBusOut()
    {
        BusOut.BusOutTriggerEvents busOutTriggerEvents = new BusOut.BusOutTriggerEvents(
           () => { TriggerEventGameOver(MiniGameType.Bus, true); }, //游戏成功
           () => { TriggerEventGameOver(MiniGameType.Bus, false); },//游戏失败
           (int pIndex) =>
           {
               //点击解锁停车位
               var tEventData = EventManager.GetEventData<BusOut_OnClickUnlockSlot>(EventKey.BusOut_OnClickUnlockSlot);
               tEventData.index = pIndex;
               EventManager.Trigger(tEventData);
           },
           () => { EventManager.Trigger(EventKey.BusOut_VIPComplete); },   //vip道具使用成功
           (int pNumber) =>
           {
               //等待的乘客数量改变
               var tEventData = EventManager.GetEventData<BusOut_PassengerNumberChange>(EventKey.BusOut_PassengerNumberChange);
               tEventData.count = pNumber;
               EventManager.Trigger(tEventData);
           },
           () => { EventManager.Trigger(EventKey.BusOut_ReadyToSuccess); },//完成游戏（动画前）
           () => { EventManager.Trigger(EventKey.BusOut_VIPMoveFinish); }, //vip移动完成
           () => { EventManager.Trigger(EventKey.BusOut_VehicleHit); },    //车辆被击中
           () => { EventManager.Trigger(EventKey.BusOut_VehicleClick); },  //车辆被点击
           () => { EventManager.Trigger(EventKey.BusOut_PassengerSeat); }  //乘客到达座位
       );
        BusOut.EventManager.Instance.RegisterTriggerEvents(busOutTriggerEvents);
    }

    public void RegisterTriple()
    {
        TripleMath.TripleMathTriggerEvents eventTrigger = new TripleMath.TripleMathTriggerEvents(
            () => { TriggerEventGameOver(MiniGameType.Triple, true); }, //游戏成功
            (failReason) => { TriggerEventGameOver(MiniGameType.Triple, false); },//游戏失败
            (matchType, count) =>
            {
                //EventManager.Trigger<string, int>(EventKey.TripleMath_Submitted, matchType, count);
            },
            (matchType, count) =>
            {
                //EventManager.Trigger<string, int>(EventKey.Triple_MathReset, matchType, count);
            },
            (leftTime) =>
            {
                //EventManager.Trigger<int>(EventKey.TripleMath_CountDownTime, leftTime);
            },
            () => { EventManager.Trigger(EventKey.TripleMath_MagnetComplete); },
            () => { EventManager.Trigger(EventKey.TripleMath_UndoComplete); },
            () => { EventManager.Trigger(EventKey.TripleMath_CompassComplete); },
            () => { EventManager.Trigger(EventKey.TripleMath_FreezeComplete); },
            () => { EventManager.Trigger(EventKey.TripleMath_FreezeFinish); },
            () => { EventManager.Trigger(EventKey.TripleMath_CompassFinish); },
            (targetTrans) =>
            {
                //EventManager.Trigger<Transform>(EventKey.TripleMath_CompassRefresh, targetTrans);
            },
            (soundName) => { PlaySound(soundName); },//播放音效
            () => { EventManager.Trigger(EventKey.TripleMath_ReadyToSuccess); },
            () => { EventManager.Trigger(EventKey.TripleMath_BroomComplete); },
            () => { EventManager.Trigger(EventKey.TripleMath_Recall3ObjectComplete); },
            () => { EventManager.Trigger(EventKey.TripleMath_HourglassComplete); },
            () => { EventManager.Trigger(EventKey.TripleMath_BroomFinish); }
        );
        TripleMath.EventManager.Instance.RegisterTriggerEvents(eventTrigger);
    }

    #endregion
    public void StartGame(MiniGameType pGameType, int pLevel)
    {
        mCacheLevel = pLevel;
        mCacheType = pGameType;

        InitEvent(pGameType);

        var tSceneName = GetSceneName(pGameType);
        var tLevelConfig = ModuleManager.MiniGame.GetLevelConfig(pGameType, pLevel);
        if (pGameType == MiniGameType.Screw)
        {
            var tLevelID = ModuleManager.MiniGame.GetLevelID(pGameType, pLevel);
            LoadScene(tSceneName, () =>
            {
                ScrewJam.GameModel.Instance.StartLevel(tLevelID);
                PageManager.Instance.OpenPage(PageID.ScrewGamePage, new MiniGamePageParam(pLevel));
                TriggerEventGameStart(pGameType, mCacheLevel);
            });
        }
        else if (pGameType == MiniGameType.Jam3d)
        {
            int[] tLevelIDs = ModuleManager.MiniGame.GetLevelIDs(pGameType, pLevel, tLevelConfig.Chapter);
            LoadScene(tSceneName, () =>
            {
                var levelGroup = ConfigData.jamLevelGroupConfig.GetByPrimary(tLevelIDs[0]);
                GameLogic.JamManager.GetSingleton().UpdateStepsExchangeIndex(pLevel, levelGroup.threshold);
                GameLogic.JamManager.GetSingleton().StartGame(tLevelIDs);
                PageManager.Instance.OpenPage(PageID.Jam3DGamePage, new MiniGamePageParam(pLevel, tLevelIDs.Length));
                TriggerEventGameStart(pGameType, mCacheLevel);
            });
        }
        else if (pGameType == MiniGameType.Tile)
        {
            var tLevelID = ModuleManager.MiniGame.GetLevelID(pGameType, pLevel);
            LoadScene(tSceneName, () =>
            {
                ScrewJam.GameModel.Instance.StartLevel(tLevelID);
                PageManager.Instance.OpenPage(PageID.TileGamePage, new MiniGamePageParam(pLevel));
                TriggerEventGameStart(pGameType, mCacheLevel);
            });
        }
        else if (pGameType == MiniGameType.Bus)
        {
            var tLevelID = ModuleManager.MiniGame.GetLevelID(pGameType, pLevel);
            LoadScene(tSceneName, () =>
            {
                BusOut.GameModel.Instance.StartLevel(tLevelID, pLevel != 1, tLevelConfig.IconNumber);
                PageManager.Instance.OpenPage(PageID.BusGamePage, new MiniGamePageParam(pLevel));
                TriggerEventGameStart(pGameType, mCacheLevel);
            });
        }
        else if (pGameType == MiniGameType.Triple)
        {
            var tLevelID = ModuleManager.MiniGame.GetLevelID(pGameType, pLevel);
            var tLimitTime = ModuleManager.MiniGame.GetTripleLevelTimes(tLevelConfig, tLevelID);
            LoadScene(tSceneName, () =>
            {
                Camera uiCamera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
                TripleMath.TripleMathManager.Instance.InitLevel(tLevelID, uiCamera, tLimitTime);
                PageManager.Instance.OpenPage(PageID.TripleGamePage, new MiniGamePageParam(pLevel));
                TriggerEventGameStart(pGameType, mCacheLevel);
            });
        }
    }


    void TriggerEventGameStart(MiniGameType pType, int levelID)
    {
        var tEventData = EventManager.GetEventData<MiniGameStart>(EventKey.MiniGameStart);
        tEventData.modeType = pType;
        tEventData.levelID = levelID;
        tEventData.isNewGame = ModuleManager.MiniGame.IsNewGame((int)pType);
        EventManager.Trigger(tEventData);

        AudioManager.Instance.PlayMusic(MusicID.bgm_mini_game);
        AudioManager.Instance.PlaySound(SoundID.Tile_Level_Begin);
    }

    public void TriggerEventGameOver(MiniGameType pType, bool pIsSuccess)
    {
        var tEventData = EventManager.GetEventData<MiniGameOver>(EventKey.MiniGameOver);
        tEventData.modeType = pType;
        tEventData.levelID = mCacheLevel;
        tEventData.isSuccess = pIsSuccess;
        EventManager.Trigger(tEventData);
    }

    void TriggerEventUsePropComplete(MiniGameType pType, PropID pPropID)
    {
        var tEventData = EventManager.GetEventData<MiniGameUsePropComplete>(EventKey.MiniGameUsePropComplete);
        tEventData.modeType = pType;
        tEventData.propID = pPropID;
        EventManager.Trigger(tEventData);
    }


    void PlaySound(string pSoundName)
    {
        if (Enum.TryParse(pSoundName, out SoundID result))
        {
            AudioManager.Instance.PlaySound(result);
        }
        else
        {
            LogManager.LogError($"无法将字符串转换为枚举 :{pSoundName}");
        }
    }

    #region 小游戏场景管理

    List<string> mScenes = new List<string>();

    string GetSceneName(MiniGameType pType)
    {
        return ModuleManager.MiniGame.GetTypeConfig((int)pType).sceneName;
    }

    void LoadScene(string pSceneName, Action pAction)
    {
        bool tIsExist = false;
        foreach (var item in mScenes)
        {
            if (item == pSceneName)
            {
                tIsExist = true;
            }
            else
            {
                AssetManager.Instance.UnloadScene(item);
            }
        }
        if (tIsExist)
        {
            UnloadScene(pSceneName, () =>
            {
                AssetManager.Instance.LoadSceneAsync(pSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive, (scene) =>
                {
                    pAction?.Invoke();
                    UIRoot.Instance.MainCamera.enabled = false;
                    mScenes.Add(pSceneName);
                });
            });
        }
        else
        {
            AssetManager.Instance.LoadSceneAsync(pSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive, (scene) =>
            {
                pAction?.Invoke();
                UIRoot.Instance.MainCamera.enabled = false;
                mScenes.Add(pSceneName);
            });
        }

    }

    public void UnloadScene(string pSceneName, Action pAction = null)
    {
        AssetManager.Instance.UnloadScene(pSceneName, () =>
        {
            pAction?.Invoke();
            UIRoot.Instance.MainCamera.enabled = true;
            mScenes.Remove(pSceneName);
        });
    }

    public void UnloadCurTypeScene(Action pAction = null)
    {
        string tSceneName = GetSceneName(mCacheType);
        AssetManager.Instance.UnloadScene(tSceneName, () =>
        {
            pAction?.Invoke();
            UIRoot.Instance.MainCamera.enabled = true;
            mScenes.Remove(tSceneName);
        });
    }

    #endregion

    #region GM方法

    #endregion
}
