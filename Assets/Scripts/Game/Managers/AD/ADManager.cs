using Config;
using Game;
using LLFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ADManager : Singleton<ADManager>
{
    public bool IsRewardVideoReady => mInitialized && NativeUtil.IsRewardVideoReady();
    public bool IsInterstitialReady => mInitialized && NativeUtil.IsInterstitialReady();
    public bool IsBannerReady => mInitialized && NativeUtil.IsBannerReady();
    public string VideoECPM => mInitialized ? NativeUtil.GetRewardVideoEcpm() : string.Empty;
    public string InterstitialECPM => mInitialized ? NativeUtil.GetInterstitialEcpm() : string.Empty;
    public string BannerECPM => mInitialized ? NativeUtil.GetBannerEcpm() : string.Empty;

    public ADShowReason ADShowReason { get; private set; }
    public bool IsShowingVideoAd { get; private set; }
    public bool IsVideoAdRewarded { get; private set; }


    bool mInitialized;
    RewardVideoLogic mRewardVideoLogic;
    InterstitialLogic mInterstitialLogic;

    public void Init()
    {
        //var tAdRoot = new GameObject("AdEvents", typeof(AdEvents));
        var tAdRoot = AdEvents.Instance;
        tAdRoot.transform.SetParent(GameLauncher.Instance.transform);
        new BannerLogic().Init();
        mInterstitialLogic = new InterstitialLogic();
        mInterstitialLogic.Init();
        mRewardVideoLogic = new RewardVideoLogic();
        mRewardVideoLogic.Init();

        AdEvents.OnAdDisplayedEvent += OnAdShown;
        AdEvents.OnAdClosedEvent += OnADClosed;
        AdEvents.OnVideoAdRewardedEvent += OnVideoADRewarded;
        AdEvents.OnAdLoadedEvent += OnVideoADLoaded;
        AdEvents.OnAdDisplayFailedEvent += OnADShowFailed;
        //AdEvents.OnBanerRevenueEvent += OnBanerRevenue;
        AdEvents.OnAdForecastRevenueEvent += OnAdForecastRevenueEvent;

        EventManager.Register(EventKey.StartNewDay, OnStartNewDay);

        mInitialized = true;
        NativeUtil.InitAdlib();
    }

    void OnAdShown(string pADType, string pPlatform, string pPlacementID, string pADID)
    {
        ADType tADType = GetADType(pADType);
        if (tADType == ADType.Banner || tADType == ADType.Invalid)
        {
            return;
        }

        if (tADType == ADType.RewardVideo)
        {
            IsShowingVideoAd = true;
        }

        var tEventData = EventManager.GetEventData<ADEvent>(EventKey.ADShown);
        tEventData.ADType = tADType;
        tEventData.platform = pPlatform;
        tEventData.placementID = pPlacementID;
        tEventData.ADID = pADID;
        tEventData.showReason = ADShowReason;
        EventManager.Trigger(tEventData);
    }

    void OnADClosed(string pADType, string pPlatform, string pPlacementID, string pADID)
    {
        ADType tADType = GetADType(pADType);
        if (tADType == ADType.Banner || tADType == ADType.Invalid)
        {
            return;
        }

        if (tADType == ADType.RewardVideo)
        {
            IsShowingVideoAd = false;
            if (IsVideoAdRewarded)
            {
                TriggerAdRewardedEvent(pPlatform, pPlacementID, pADID);
            }
        }

        var tEventData = EventManager.GetEventData<ADEvent>(EventKey.ADClosed);
        tEventData.ADType = tADType;
        tEventData.platform = pPlatform;
        tEventData.placementID = pPlacementID;
        tEventData.ADID = pADID;
        tEventData.showReason = ADShowReason;
        EventManager.Trigger(tEventData);
    }

    void OnVideoADRewarded(string pADType, string pPlatform, string pPlacementID, string pADID)
    {
        if (IsShowingVideoAd)
        {
            IsVideoAdRewarded = true;
        }
        else
        {
            TriggerAdRewardedEvent(pPlatform, pPlacementID, pADID);
        }
    }

    void OnVideoADLoaded(string pADType, string pPlatform, string pPlacementID, string pADID)
    {
        ADType tADType = GetADType(pADType);
        if (tADType == ADType.RewardVideo)
        {
            var tEventData = EventManager.GetEventData<ADEvent>(EventKey.VideoADLoaded);
            tEventData.ADType = tADType;
            tEventData.platform = pPlatform;
            tEventData.placementID = pPlacementID;
            tEventData.ADID = pADID;
            EventManager.Trigger(tEventData);
        }
    }

    void OnADShowFailed(string pADType, string pPlatform, string pPlacementID, string pADID)
    {
        ADType tADType = GetADType(pADType);
        if (tADType == ADType.Banner || tADType == ADType.Invalid)
        {
            return;
        }

        var tEventData = EventManager.GetEventData<ADEvent>(EventKey.ADShowFailed);
        tEventData.ADType = tADType;
        tEventData.platform = pPlatform;
        tEventData.placementID = pPlacementID;
        tEventData.ADID = pADID;
        EventManager.Trigger(tEventData);
    }

    //void OnBanerRevenue(string pADType, string pPlatform, string pPlacementID, string pADID)
    //{
    //    var tEventData = EventManager.GetEventData(EventDataType.BanerRevenue) as ADEvent;
    //    tEventData.ADType = ADType.Banner;
    //    tEventData.platform = pPlatform;
    //    tEventData.placementID = pPlacementID;
    //    tEventData.ADID = pADID;
    //    EventManager.TriggerEvent(tEventData);
    //}

    void OnAdForecastRevenueEvent(string pADType, string pPlatform, string pPlacementID, string pADID, string pCountry, string pADRevenue)
    {
        ADType tADType = GetADType(pADType);
        if (tADType == ADType.Invalid)
        {
            return;
        }

        var tEventData = EventManager.GetEventData<ADEvent>(EventKey.ADForecastRevenue);
        tEventData.ADType = tADType;
        tEventData.platform = pPlatform;
        tEventData.placementID = pPlacementID;
        tEventData.ADID = pADID;
        tEventData.country = pCountry;
        tEventData.ADRevenue = pADRevenue;
        EventManager.Trigger(tEventData);
    }

    ADType GetADType(string pADType)
    {
        if (pADType.Equals(AdEvents.Param_AdTypeReward))
        {
            return ADType.RewardVideo;
        }

        if (pADType.Equals(AdEvents.Param_AdTypeInterstitial))
        {
            return ADType.Interstitial;
        }

        if (pADType.Equals(AdEvents.Param_AdTypeBanner))
        {
            return ADType.Banner;
        }

        return ADType.Invalid;
    }

    void TriggerAdRewardedEvent(string pPlatform, string pPlacementID, string pADID)
    {
        IsVideoAdRewarded = false;

        var tEventData = EventManager.GetEventData<ADEvent>(EventKey.VideoADRewarded);
        tEventData.ADType = ADType.RewardVideo;
        tEventData.platform = pPlatform;
        tEventData.placementID = pPlacementID;
        tEventData.ADID = pADID;
        tEventData.showReason = ADShowReason;
        EventManager.Trigger(tEventData);
    }

    public void PlayRewardVideo(ADShowReason pAdShowReason, Action<bool> pAction = null)
    {
        if (!IsRewardVideoReady)
        {
            MessageHelp.Instance.ShowMessage("no internet connection");
            return;
        }
        mRewardVideoLogic.TryShowRewardVideo(pAdShowReason, pAction);
    }

    public void PlayInterstitial(ADShowReason pAdShowReason, Action<bool> pAction = null)
    {
        if (GameMethod.HasRemoveAD() || !IsInterstitialReady)
        {
            pAction?.Invoke(false);
            return;
        }
         
        mInterstitialLogic.TryShowInterstitial(pAdShowReason, pAction);
    }

    public void ShowRewardVideo(ADShowReason pAdShowReason)
    {
        if (!IsRewardVideoReady)
        {
            MessageHelp.Instance.ShowMessage("no internet connection");
            //LogManager.LogError("ADManager: Reward Video not Ready");
            return;
        }

        if (CanShow(pAdShowReason, true))
        {
            ADShowReason = pAdShowReason;
#if UNITY_EDITOR
            TriggerAdRewardedEvent("Editor", "Editor", "Editor");
            return;
#else
            UIRoot.Instance.adLoadingManager.Show();
            NativeUtil.ShowRewardVideo();
#endif
        }
    }

    public void ShowInterstitial(ADShowReason pAdShowReason)
    {
        if (GameMethod.HasRemoveAD())
        {
            return;
        }

        if (!IsInterstitialReady)
        {
            LogManager.LogError("ADManager: Interstitial not Ready");
            return;
        }

        if (CanShow(pAdShowReason, true))
        {
            ADShowReason = pAdShowReason;
#if UNITY_EDITOR
            OnADClosed(AdEvents.Param_AdTypeInterstitial, "Editor", "Editor", "Editor");
            return;
#else
            UIRoot.Instance.adLoadingManager.Show();
            NativeUtil.ShowInterstitial();
#endif
        }
    }

    public void ShowBanner()
    {
        NativeUtil.ShowBanner();
    }

    public void HideBanner()
    {
        NativeUtil.HideBanner();
    }

    #region  冷却时间和每日次数的限制

    const string AD_PLAY_RECTORD_KEY = "ADPlayRecord";
    const string AD_FIRST_PLAY_TIME_KEY = "ADFirstTime";
    Dictionary<int, (long, int)> mADPlayRecord;

    void OnStartNewDay(EventData pEventData)
    {
        mADPlayRecord = new Dictionary<int, (long, int)>();
        PlayerPrefs.SetString(AD_FIRST_PLAY_TIME_KEY, DateTime.Now.Ticks.ToString());
        DataTool.Serialize(AD_PLAY_RECTORD_KEY, mADPlayRecord);
    }

    public bool CanShow(ADShowReason pAdShowReason, bool pIsExpend = false)
    {
        if (mADPlayRecord == null)
        {
            var tDateTime = new DateTime(PlayerPrefs.GetString(AD_FIRST_PLAY_TIME_KEY, "0").ToLong());
            if (DateTime.Equals(DateTime.Now.Date, tDateTime.Date))
            {
                mADPlayRecord = DataTool.Deserialize<Dictionary<int, (long, int)>>(AD_PLAY_RECTORD_KEY);
            }
            else
            {
                mADPlayRecord = new Dictionary<int, (long, int)>();
                PlayerPrefs.SetString(AD_FIRST_PLAY_TIME_KEY, DateTime.Now.Ticks.ToString());
            }
        }

        bool tResult;
        var tConfig = GetADVideoConfig(pAdShowReason);
        if (mADPlayRecord.ContainsKey((int)pAdShowReason))
        {
            var tRecord = mADPlayRecord[(int)pAdShowReason];
            TimeSpan timeSpan = DateTime.Now - new DateTime(tRecord.Item1);
            if (timeSpan.Minutes >= tConfig.cdTime && tRecord.Item2 < tConfig.everydayCount)
            {
                if (pIsExpend)
                {
                    tRecord.Item1 = DateTime.Now.Ticks;
                    tRecord.Item2++;
                    mADPlayRecord[(int)pAdShowReason] = tRecord;
                    DataTool.Serialize(AD_PLAY_RECTORD_KEY, mADPlayRecord);
                }
                tResult = true;
            }
            else
            {
                tResult = false;
            }
            LogManager.Log($"ADManager.CanShow: {pAdShowReason}/{tResult}_{tRecord.Item2}/{tConfig.everydayCount} ");
        }
        else
        {
            if (tConfig.everydayCount > 0)
            {
                if (pIsExpend)
                {
                    mADPlayRecord.Add((int)pAdShowReason, (DateTime.Now.Ticks, 1));
                    DataTool.Serialize(AD_PLAY_RECTORD_KEY, mADPlayRecord);
                }
                tResult = true;
            }
            else
            {
                tResult = false;
            }
            LogManager.Log($"ADManager.CanShow: {pAdShowReason}/{tResult}_{(tConfig.everydayCount > 0 ? 1 : 0)}/{tConfig.everydayCount} ");
        }

        return tResult;
    }

    ADVideoConfig GetADVideoConfig(ADShowReason pAdShowReason)
    {
        var tConfigs = ConfigData.aDVideoConfig.GetByIndexes(pAdShowReason.ToString());

        if (tConfigs == null || tConfigs.Count <= 0)
        {
            return ConfigData.aDVideoConfig.GetByPrimary(1);
        }

        return tConfigs[0];
    }

    #endregion

}

public enum ADType
{
    Invalid = -1,
    RewardVideo,
    Interstitial,
    Banner,
}

//已换使用配置表ADVideo生成
//public enum ADShowReason
//{
//    Video_ShopFreeRotate, 
//    Video_GameRebirth, 

//    Interstitial_GameStart,
//    Interstitial_GameOver,
//    Interstitial_GameRetry,

//    Video_GMCommand,
//    Interstitial_GMCommand,
//}