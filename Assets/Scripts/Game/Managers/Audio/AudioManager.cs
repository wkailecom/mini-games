using System.Collections.Generic;
using LLFramework;
using UnityEngine;

namespace Game
{
    public class AudioManager : Singleton<AudioManager>
    {
        const int SoundAudioSourceCount = 9;
        const string SoundSwitchKey = "SoundSwitchKey";
        const string MusicSwitchKey = "MusicSwitchKey";
        const string VibrateSwitchKey = "VibrateSwitchKey";
        const MusicID DefaultMusic = MusicID.bgm_main;
        string AudioPath(string pAudioName) => "Audios/" + pAudioName;

        bool mSoundSwitch;
        bool mMusicSwitch;
        bool mVibrateSwitch;
        Dictionary<int, AudioClip> mSounds;
        Dictionary<int, AudioClip> mMusics;

        public bool SoundSwitch
        {
            get => mSoundSwitch;
            set
            {
                if (mSoundSwitch == value)
                {
                    return;
                }
                mSoundSwitch = value;
                PlayerPrefs.SetInt(SoundSwitchKey, mSoundSwitch ? 1 : 0);

                if (!mSoundSwitch)
                {
                    StopAudio();
                }
            }
        }
        public bool MusicSwitch
        {
            get => mMusicSwitch;
            set
            {
                if (mMusicSwitch == value)
                {
                    return;
                }
                mMusicSwitch = value;
                PlayerPrefs.SetInt(MusicSwitchKey, mMusicSwitch ? 1 : 0);

                if (mMusicSwitch)
                {
                    PlayMusic();
                }
                else
                {
                    StopMusic();
                }
            }
        }

        public bool VibrateSwitch
        {
            get => mVibrateSwitch;
            set
            {
                if (mVibrateSwitch == value)
                {
                    return;
                }
                mVibrateSwitch = value;
                PlayerPrefs.SetInt(VibrateSwitchKey, mVibrateSwitch ? 1 : 0);

                if (mVibrateSwitch)
                {
                    PlayVibrate(VibrateID.Heavy);
                }
            }
        }

        AudioSource[] mSoundAudioSources;
        int mCurrentSoundIndex;
        AudioSource mMusicAudioSource;
        AudioPlayer mPlayer;

        public void Init()
        {
            var tAudioRoot = new GameObject(typeof(AudioManager).Name, typeof(AudioListener));
            tAudioRoot.transform.SetParent(GameLauncher.Instance.transform);

            var tSoundAudioSourceRoot = new GameObject("SoundAudioSource");
            tSoundAudioSourceRoot.transform.SetParent(tAudioRoot.transform);
            var tMusicAudioSourceRoot = new GameObject("MusicAudioSource");
            tMusicAudioSourceRoot.transform.SetParent(tAudioRoot.transform);

            mSoundAudioSources = new AudioSource[SoundAudioSourceCount];
            for (int i = 0; i < SoundAudioSourceCount; i++)
            {
                var tSoundAudioSource = tSoundAudioSourceRoot.AddComponent<AudioSource>();
                tSoundAudioSource.playOnAwake = false;
                tSoundAudioSource.loop = false;
                mSoundAudioSources[i] = tSoundAudioSource;
            }

            mMusicAudioSource = tMusicAudioSourceRoot.AddComponent<AudioSource>();
            mMusicAudioSource.playOnAwake = false;
            mMusicAudioSource.loop = true;
            mMusicAudioSource.volume = 0.5f;

            mMusicSwitch = PlayerPrefs.GetInt(MusicSwitchKey, 1) == 1;
            mSoundSwitch = PlayerPrefs.GetInt(SoundSwitchKey, 1) == 1;
            mVibrateSwitch = PlayerPrefs.GetInt(VibrateSwitchKey, 1) == 1;
            mSounds = new Dictionary<int, AudioClip>();
            mMusics = new Dictionary<int, AudioClip>();

            mPlayer = new AudioPlayer();
            mPlayer.Init();
        }

        public void PlayVibrate(VibrateID pVibrateID)
        {
            if (!VibrateSwitch || !Vibration.HasVibrator())
            {
                return;
            }

#if UNITY_IOS
            if (pVibrateID == VibrateID.Light)
            {
                Vibration.VibrateIOS(ImpactFeedbackStyle.Light);
            }
            else if (pVibrateID == VibrateID.Medium)
            {
                Vibration.VibrateIOS(ImpactFeedbackStyle.Medium);
            }
            else if (pVibrateID == VibrateID.Heavy)
            {
                //Vibration.VibrateIOS(ImpactFeedbackStyle.Heavy);
                Vibration.VibratePeek();
            }
#elif UNITY_ANDROID
            if (pVibrateID == VibrateID.Light)
            {
                Vibration.VibrateAndroid(30);
            }
            else if (pVibrateID == VibrateID.Medium)
            {
                Vibration.VibrateAndroid(40);
            }
            else if (pVibrateID == VibrateID.Heavy)
            {
                Vibration.VibrateAndroid(50);
            }
#endif

        }

        public void PlaySound(SoundID pID)
        {
            if (!SoundSwitch)
            {
                return;
            }

            if (!mSounds.TryGetValue((int)pID, out var tAudioClip))
            {
                tAudioClip = LoadAudioClip(pID.ToString());
                mSounds.Add((int)pID, tAudioClip);
            }

            if (tAudioClip == null)
            {
                return;
            }

            mCurrentSoundIndex = ++mCurrentSoundIndex % SoundAudioSourceCount;
            mSoundAudioSources[mCurrentSoundIndex].clip = tAudioClip;
            mSoundAudioSources[mCurrentSoundIndex].Play();
        }

        public void PlayMusic(MusicID pID)
        {
            if (!MusicSwitch)
            {
                return;
            }

            if (!mMusics.TryGetValue((int)pID, out var tAudioClip))
            {
                tAudioClip = LoadAudioClip(pID.ToString());
                mMusics.Add((int)pID, tAudioClip);
            }

            if (tAudioClip == null)
            {
                return;
            }

            if (mMusicAudioSource.clip == tAudioClip && mMusicAudioSource.isPlaying)
            {
                return;
            }

            mMusicAudioSource.clip = tAudioClip;
            mMusicAudioSource.Play();
        }

        public void PlayMusic()
        {
            if (!mMusicSwitch)
            {
                return;
            }

            if (mMusicAudioSource.clip != null)
            {
                mMusicAudioSource.Play();
            }
            else
            {
                PlayMusic(DefaultMusic);
            }
        }

        public void StopAudio()
        {
            for (int i = 0; i < SoundAudioSourceCount; i++)
            {
                mSoundAudioSources[i].Stop();
            }
        }

        public void StopMusic()
        {
            mMusicAudioSource.Stop();
        }

        AudioClip LoadAudioClip(string pAudioName)
        {
            var tAudioClip = AssetManager.Instance.LoadAsset<AudioClip>(AudioPath(pAudioName));
            if (tAudioClip == null)
            {
                LogManager.LogError($"AudioManager.LoadAudioClip: no Audio Named {pAudioName}");
            }

            return tAudioClip;
        }
    }

    public enum SoundID
    {
        //按钮绑定（顺序不可动）
        BtnClick,
        BtnClick_Paly,

        //方法调用
        Erase = 1001,
        Mini_Prop_Recall,
        Mini_Prop_Magnet,
        Mini_Prop_Shuffle,
        Mini_Prop_ExtraSlot,

        Tile_Brick_Click,
        Tile_Brick_eliminate,
        Tile_Level_Begin,
        Tile_Level_Succeed,
        Tile_Level_Failed,

        Module_Close,
        Panel_Break2,
        Screw_Clicked_Success,

        EraseEncourage_Good,
        EraseEncourage_Great,
        EraseEncourage_Excellent,
        EraseEncourage_Outstanding,
        EraseEncourage_Amazing,
        EraseEncourage_Unbelievable,

        GameOver,
        GameOver_Lose1,
        GameOver_Lose2,
        GameOver_Win1,
        GameOver_Win2,
        GetReward,
        Countdown,

        Eff_Clover_Centered,
        Eff_Clover_Split,
        Eff_Magnet,
        Eff_Chainsaw,
    }

    public enum MusicID
    {
        bgm_main,
        bgm_mini_map,
        bgm_mini_game,
        Max
    }

    public enum VibrateID
    {
        Light,
        Medium,
        Heavy,
        //Rigid,
        //Soft,
    }
}