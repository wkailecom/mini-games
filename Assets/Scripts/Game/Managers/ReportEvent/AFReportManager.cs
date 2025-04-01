using AppsFlyerSDK;
using Config;
using Game;
using Game.Sdk;
using System.Collections.Generic;
using UnityEngine;

public static class AFReportManager
{
    static bool[] mReportPlatform;
    public static void Init()
    {
        EventManager.Register(EventKey.VideoADRewarded, OnVideoAdRewarded);
        EventManager.Register(EventKey.ADClosed, OnADClosed);
        //EventManager.Register(EventKey.PurchaseSuccess, OnPurchaseSuccess);
        //EventManager.Register(EventKey.ValidateReceiptResult, OnValidateReceiptResult);
        //EventManager.Register(EventKey.PropCountChange, OnPropCountChange);

    }


    static void OnVideoAdRewarded(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.RewardVideo)
        {
            LogAppEvent("af_rewardedvideo_complete");
        }
    }

    static void OnADClosed(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.Interstitial)
        {
            LogAppEvent("af_interstitialvideo_complete");
        }
    }

    static void OnPurchaseSuccess(EventData pEventData)
    {
        var tEventData = pEventData as PurchaseSuccess;
        if (Mathf.Approximately(tEventData.productConfig.price, 0.99f))
        {
            LogAppEvent("af_purchase0.99");
        }
    }

    static void OnValidateReceiptResult(EventData pEventData)
    {
        var tEventData = pEventData as ValidateReceiptResult;
        var tOrderInfo = tEventData.orderInfo;
        if (ReportEventDefine.InstallDays_Value <= 14 && tOrderInfo.OrderType == OrderType.Valid)
        {
            LogAppEvent("ads_first14d_purchase",
                new Dictionary<string, string>()
                {
                    {AFInAppEvents.REVENUE, tOrderInfo.ProductPrice.ToString()},
                    {AFInAppEvents.CURRENCY,  "USD"}
                });
        }
    }

    static void OnPropCountChange(EventData pEventData)
    {
        var tEventData = pEventData as PropCountChange;


    }

    static void OnGameOver(EventData pEventData)
    {
        var tEventData = pEventData as GameOver;

        var tOverTimes = ModuleManager.Statistics.GetValue(StatsID.OverTimes, StatsGroup.Total);
        LogConfigEvent(1002, tOverTimes);

    }


    public static void LogConfigEvent(int pEventType, int pTargetValue)
    {
        var tConfig = ConfigData.logAppEventConfig.GetByIndexes(pEventType);
        if (tConfig != null && tConfig.Count > 0)
        {
            LogConfigEvent(tConfig, pTargetValue);
        }
    }

    static void LogConfigEvent(List<LogAppEventConfig> pEventConfigs, int pTargetValue)
    {
        var tInstallHours = ModuleManager.UserInfo.InstallHoursCount;
        foreach (var tConfig in pEventConfigs)
        {
            if (tInstallHours > tConfig.limitTime) continue;
            if (pTargetValue != tConfig.targetValue) continue;

            mReportPlatform = AppInfoManager.Instance.IsIOS ? tConfig.iosPlatform : tConfig.andriodPlatform;
            if (mReportPlatform.Length > 0 && mReportPlatform[0])
            {
                FirebaseManager.LogAppEvent(tConfig.eventName);
            }
            if (mReportPlatform.Length > 1 && mReportPlatform[1])
            {
                AFReportManager.LogAppEvent(tConfig.eventName);
            }
            if (mReportPlatform.Length > 2 && mReportPlatform[2])
            {
                FBReportManager.LogAppEvent(tConfig.eventName);
            }

            if (tConfig.reportBQ > 0)
            {
                BQReportManager.LogAppEvent(tConfig.eventName);
            }
        }
    }

    public static void LogAppEvent(string eventName, Dictionary<string, string> eventValues = null)
    {
        eventValues ??= new Dictionary<string, string>();
        AppsFlyer.sendEvent(eventName, eventValues);
    }

    public static void LogAdRevenue(ADEvent pADEvent, double pAdRevenue)
    {
        Dictionary<string, string> additionalParams = new Dictionary<string, string>();
        additionalParams.Add(AFAdRevenueEvent.COUNTRY, pADEvent.country);
        additionalParams.Add(AFAdRevenueEvent.AD_UNIT, pADEvent.ADID);
        additionalParams.Add(AFAdRevenueEvent.AD_TYPE, pADEvent.ADType.ToString());
        additionalParams.Add(AFAdRevenueEvent.PLACEMENT, pADEvent.platform);
        //additionalParams.Add(AFAdRevenueEvent.ECPM_PAYLOAD, "encrypt");

        //if (pADEvent.platform.Equals("Unity Ads")) //广告渠道
        //var tInstallHours = ModuleManager.UserInfo.InstallHoursCount;
        //bool tIsUnityInstal = GameVariable.AFMediaSource.Equals("unityads_int");//归因来源
        //if (!tIsUnityInstal || tInstallHours >= 8 * 24) return;

        AppsFlyerAdRevenue.logAdRevenue(pADEvent.platform,
                                        AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax,
                                        pAdRevenue,
                                        "USD",
                                        additionalParams);
    }
}