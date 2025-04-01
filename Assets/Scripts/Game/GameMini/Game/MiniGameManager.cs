using Config;
using Game;
using Game.MiniGame;
using Game.UISystem;
using LLFramework;
using System;
using System.Collections;
using System.Collections.Generic;

public class MiniGameManager : Singleton<MiniGameManager>
{
    public bool GameStart { get; private set; }
    public int Level => mCacheLevel;
    public MiniGameType GameType => mCacheType;

    int mCacheLevel;
    MiniGameType mCacheType;

    public void Init()
    {
        mCacheLevel = ModuleManager.MiniGame.CurLevel;
    }

    public void InitEvent(MiniGameType pGameType)
    {
        if (pGameType == MiniGameType.Screw)
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
        else if (pGameType == MiniGameType.Jam3d)
        {
            GameLogic.JamTriggerEvents jamEventTrigger = new GameLogic.JamTriggerEvents(
                () =>
                {
                    //开始游戏之后;
                },
                (level) => { TriggerEventGameOver(MiniGameType.Jam3d, true); },
                (level) => { EventManager.Trigger(EventKey.MiniGameSubSuccess); },
                (level) => { TriggerEventGameOver(MiniGameType.Jam3d, false); },
                (soundName) => { PlaySound(soundName); },
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
                        var res = AssetManager.Instance.LoadAsset<UnityEngine.Object>($"PackageJam3d/BundleResources/Terrains/Terrain_{levelGroup.prefab}");
                        return res;
                    }
                    return null;
                }, (pathInPackage) =>
                {
                    var res = AssetManager.Instance.LoadAsset<UnityEngine.Object>($"PackageJam3d/{pathInPackage}");
                    return res;
                });
            GameLogic.JamManager.GetSingleton().RegisterJamTriggerEvents(jamEventTrigger);
        }
    }

    public void StartGame(MiniGameType pGameType, int pLevel)
    {
        mCacheLevel = pLevel;
        mCacheType = pGameType;
        var tSceneName = pGameType.ToString();

        InitEvent(pGameType);

        var tLevelConfig = ModuleManager.MiniGame.GetLevelConfig(pLevel);
        if (pGameType == MiniGameType.Screw)
        {
            var tLevelID = ModuleManager.MiniGame.GetLevelID(pLevel);
            LoadScene(tSceneName, () =>
            {
                ScrewJam.GameModel.Instance.StartLevel(tLevelID);
                PageManager.Instance.OpenPage(PageID.ScrewGamePage, new MiniGamePageParam(pLevel));
                TriggerEventGameStart(pGameType, mCacheLevel);
            });
        }
        else if (pGameType == MiniGameType.Jam3d)
        {
            int[] tLevelIDs = ModuleManager.MiniGame.GetLevelIDs(pLevel, tLevelConfig.Chapter);
            LoadScene(tSceneName, () =>
            {
                var levelGroup = ConfigData.jamLevelGroupConfig.GetByPrimary(tLevelIDs[0]);
                GameLogic.JamManager.GetSingleton().UpdateStepsExchangeIndex(pLevel, levelGroup.threshold);
                GameLogic.JamManager.GetSingleton().StartGame(tLevelIDs);
                PageManager.Instance.OpenPage(PageID.Jam3DGamePage, new MiniGamePageParam(pLevel, tLevelIDs.Length));
                TriggerEventGameStart(pGameType, mCacheLevel);
            });
        }
    }


    void TriggerEventGameStart(MiniGameType pType, int levelID)
    {
        var tEventData = EventManager.GetEventData<MiniGameStart>(EventKey.MiniGameStart);
        tEventData.modeType = pType;
        tEventData.levelID = levelID;
        EventManager.Trigger(tEventData);

        AudioManager.Instance.PlayMusic(MusicID.bgm_mini_game);
        AudioManager.Instance.PlaySound(SoundID.Tile_Level_Begin);
    }

    void TriggerEventGameOver(MiniGameType pType, bool pIsSuccess)
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

    List<string> mScenes = new List<string>();
    void LoadScene(string pSceneName, Action pAction)
    {
        foreach (var item in pSceneName)
        { 
            AssetManager.Instance.UnloadScene(pSceneName);
        } 
        AssetManager.Instance.LoadSceneAsync(pSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive, (scene) =>
        {
            pAction?.Invoke();
            UIRoot.Instance.MainCamera.enabled = false;
            mScenes.Add(pSceneName);
        });
    }

    public void UnloadScene(string pSceneName, Action pAction = null)
    {
        AssetManager.Instance.UnloadSceneAsync(pSceneName, () =>
        {
            pAction?.Invoke();
            UIRoot.Instance.MainCamera.enabled = true;
            mScenes.Remove(pSceneName);
        });
    }

}
