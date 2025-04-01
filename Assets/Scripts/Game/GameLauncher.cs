using Config;
using Game.Sdk;
using Game.UISystem;
using LLFramework; 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using ThinkingData.Analytics;
using Unity.Usercentrics;
using UnityEngine;

namespace Game
{
    public class GameLauncher : MonoSingleton<GameLauncher>
    {
        bool gameStart = false;
        readonly List<Action> mInitActions = new();
        readonly List<int> mInitionWeights = new();
        int mInitTotalWeight;

        protected override void Awake()
        {
            base.Awake();

            InitAppliaction();
            AppInfoManager.Instance.Init();
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;//默认文化区域 先获取后改为默认 
            //LanguageManager.Instance.Init();
            Vibration.Init();
            GameVariable.IsDebugMode = AppInfoManager.Instance.IsDebug;
            LogManager.EnableLog = AppInfoManager.Instance.IsDebug;

#if UNITY_EDITOR
            return;
#endif 
            InitShuShu();
            //InitCMP();
        }

        void InitAppliaction()
        {
            Application.runInBackground = true;
            Application.backgroundLoadingPriority = ThreadPriority.High;
            Application.targetFrameRate = GameConst.TARGET_FRAME_RATE;
            Application.logMessageReceived += OnLogMessageReceived;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;   // 屏幕不休眠
            Screen.orientation = ScreenOrientation.Portrait; // 强制竖屏
            Input.multiTouchEnabled = false;
        }

        static void OnLogMessageReceived(string pCondition, string pStackTrace, LogType pLogType)
        {
            if (pLogType == LogType.Exception)
            {
                BQReportManager.ReportException(pCondition, pStackTrace);
            }
        }

        void InitShuShu()
        {
            //TDConfig tConfig = new TDConfig(GameConst.ShuShuAppID, "https://ta-data.jackpotlandslots.com");
            //tConfig.mode = TDMode.Debug;
            //TDAnalytics.Init(tConfig);

            TDAnalytics.Init(GameConst.ShuShuAppID, "https://ta-data.jackpotlandslots.com");
            TDAnalytics.EnableLog(AppInfoManager.Instance.IsDebug);
            Dictionary<string, object> superProperties = new Dictionary<string, object>();
            superProperties["idfa"] = AppInfoManager.Instance.UserID;
            superProperties["idfv"] = AppInfoManager.Instance.IDFV;
            superProperties["advertising_id"] = AppInfoManager.Instance.MADID;
            superProperties["android_id"] = AppInfoManager.Instance.UserID;
            TDAnalytics.SetSuperProperties(superProperties);//设置公共事件属性
                                                            //TDAnalytics.EnableThirdPartySharing(TDThirdPartyType.APPSFLYER);
                                                            //开启自动采集事件
            TDAnalytics.EnableAutoTrack(TDAutoTrackEventType.AppInstall | TDAutoTrackEventType.AppStart | TDAutoTrackEventType.AppEnd);
        }

        void InitCMP()
        {
            //Usercentrics.Instance.Options.DebugMode = ConstInfoManager.Instance.IsDebug;
#if UNITY_ANDROID
            //Usercentrics.Instance.SettingsID = "sI7jK_G5Xl0aKd";
            Usercentrics.Instance.RulesetID = "gPwGiTILsmlKNe";
#elif UNITY_IOS
            //Usercentrics.Instance.SettingsID = "DxPnGW3Cw9d904";
            Usercentrics.Instance.RulesetID = "T3vJNyN0fdwZKe";
#endif
            Debug.Log($"InitCMP start");
            Usercentrics.Instance.Initialize((status) =>
            {
                Debug.Log($"InitCMP status.geolocationRuleset.bannerRequiredAtLocation :{status.geolocationRuleset.bannerRequiredAtLocation}");
                if (status.geolocationRuleset.bannerRequiredAtLocation == false)
                {
                    StartSDk();
                    return;
                }
                GameVariable.IsCMPRequiredAtLocation = true;
                Debug.Log($"InitCMP status.shouldCollectConsent :{status.shouldCollectConsent}");
                if (status.shouldCollectConsent)
                {
                    Usercentrics.Instance.ShowFirstLayer((usercentricsConsentUserResponse) =>
                    {
                        ApplyConsent(usercentricsConsentUserResponse.consents);
                    });
                }
                else
                {
                    ApplyConsent(status.consents);
                }
            },
            (errorMessage) =>
            {
                Debug.Log("[USERCENTRICS] Error on AutoInitialize " + errorMessage);
                StartSDk();
            });
        }

        void ApplyConsent(List<UsercentricsServiceConsent> consents)
        {
            Debug.Log($"InitCMP ApplyConsent");
            StartSDk();
        }

        void StartSDk()
        {
            NativeUtil.InitATT();
            AppsflyerManager.Instance.StartSDK();
        }

        IEnumerator Start()
        {
            AddInitTask(() => { ConfigData.Init(AppInfoManager.Instance.GetCurDataPath()); });
            AddInitTask(() => { ScrewJam.ResourcesManager.SetResourceLoader(new ScrewResourceLoader()); });
            AddInitTask(() => { EventManager.Init(); });
            AddInitTask(() => { ModuleManager.Init(); });
            AddInitTask(() => { TimerManager.Instance.Init(); });
            AddInitTask(() => { IAPManager.Instance.Init(); });
            AddInitTask(() => { ADManager.Instance.Init(); });
            AddInitTask(() => { GameManager.Instance.Init(); });
            AddInitTask(() => { AppsflyerManager.Instance.Init(); });
            AddInitTask(() => { FirebaseManager.Instance.Init(); });
            AddInitTask(() => { FacebookManager.Instance.Init(); });

            AddInitTask(() => { ReportEventDefine.Init(); });
            AddInitTask(() => { BQRetryReport.Instance.Init(); });
            AddInitTask(() => { BQReportManager.Init(); });
            AddInitTask(() => { AFReportManager.Init(); });
            AddInitTask(() => { FBReportManager.Init(); });
            AddInitTask(() => { FirebaseReportManager.Init(); });

            AddInitTask(() => { AudioManager.Instance.Init(); });
            AddInitTask(() => { UIRoot.Instance.Init(); });

            yield return ExecuteInitTask();
            StartSDk();
            EnterGame();
            UIRoot.Instance.uiLoading.SetActive(false);
            gameStart = true;
        }

        void AddInitTask(Action pInitAction, int pWeight = 1)
        {
            mInitActions.Add(pInitAction);
            mInitionWeights.Add(pWeight);
            mInitTotalWeight += pWeight;
        }

        IEnumerator ExecuteInitTask()
        {
            if (UILoading.Instance != null)
            {
                for (int i = 0; i < mInitActions.Count; i++)
                {
                    mInitActions[i].Invoke();
                    float progress = mInitionWeights[i] / mInitTotalWeight;
                    UILoading.Instance.AddTargetProgress(progress);
                    yield return null;
                }

                yield return UILoading.Instance.HideTask();
            }
            else
            {
                for (int i = 0; i < mInitActions.Count; i++)
                {
                    mInitActions[i].Invoke();
                }
            }
        }

        void EnterGame()
        {
            PageManager.Instance.OpenPage(PageID.HomePage);
        }


        void Update()
        {
            if (gameStart)
            {
                PageManager.Instance.OnUpdate(Time.deltaTime);
            }
        }

        DateTime mLoseFocusTime = DateTime.Now;
        void OnApplicationFocus(bool pFocus)
        {
            if (!gameStart) return;

            var tEventData = EventManager.GetEventData<ApplicationFocus>(EventKey.ApplicationFocus);
            tEventData.focus = pFocus;
            tEventData.loseFocusSeconds = pFocus ? (int)(DateTime.Now - mLoseFocusTime).TotalSeconds : 0;
            EventManager.Trigger(tEventData);

            if (!pFocus)
            {
                PlayerPrefs.Save();
                mLoseFocusTime = DateTime.Now;
            }
        }

        void OnApplicationPause(bool pPause)
        {
            if (!gameStart) return;

        }

        void OnApplicationQuit()
        {

        }

    }
}