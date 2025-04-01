using Config;
using Firebase.Analytics;
using Game;
using Game.Sdk;
using LLFramework;
using System.Collections;
using System.Collections.Generic;
using ThinkingData.Analytics;
using UnityEngine;

public static class FirebaseReportManager
{
    static Dictionary<string, double> mEcpmTotal;
    static Dictionary<string, int> mReportCount;
    static List<LogEcpmEventConfig> mEcpmLogList;
    static bool[] mReportPlatform;

    public static void Init()
    {
        mEcpmLogList = new List<LogEcpmEventConfig>();
        mEcpmTotal = DataTool.Deserialize<Dictionary<string, double>>(DataKey.ECPM_LOOP_SUM);
        mReportCount = DataTool.Deserialize<Dictionary<string, int>>(DataKey.ECPM_REPORT_SUM);

        var tCountry = AppInfoManager.Instance.DeviceCountry;
        foreach (var tConfig in ConfigData.logEcpmEventConfig.DataList)
        {
            var tTargetCountry = new List<string>(tConfig.targetCountry);
            if (tTargetCountry.Contains(tCountry))
            {
                mEcpmLogList.Add(tConfig);
            }
        }

        if (mEcpmLogList.Count <= 0)
        {
            foreach (var tConfig in ConfigData.logEcpmEventConfig.DataList)
            {
                mEcpmLogList.Add(tConfig);
            }
        }

        //EventManager.Register(EventKey.ADClosed, OnADClosed);
        ////EventManager.Register(EventDataType.VideoADRewarded, OnVideoADRewarded);
        EventManager.Register(EventKey.ADForecastRevenue, OnADForecastRevenue);
    }

    //static void OnVideoADRewarded(EventData pEventData)
    //{
    //    var tEventData = pEventData as ADEvent;
    //    if (tEventData.ADType == ADType.RewardVideo)
    //    {
    //        LogECPM(ADType.RewardVideo, ADManager.Instance.VideoECPM);
    //    }
    //}

    static void OnADForecastRevenue(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        //if (tEventData.ADType == ADType.Banner)
        //{
        //    LogECPM(ADType.Banner, ADManager.Instance.BannerECPM);
        //}

        LogEvent_ADRevenue(tEventData);
        LogEvent_ShuShuADRevenue(tEventData);
        LogEvent_ECPM(tEventData.ADType, tEventData.ADRevenue);
    }

    static void OnADClosed(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.Interstitial)
        {
            LogECPM(ADType.Interstitial, ADManager.Instance.InterstitialECPM);
        }
        else if (tEventData.ADType == ADType.RewardVideo)
        {
            LogECPM(ADType.RewardVideo, ADManager.Instance.VideoECPM);
        }
        else if (tEventData.ADType == ADType.Banner)
        {
            LogECPM(ADType.Banner, ADManager.Instance.BannerECPM);
        }
    }

    static void LogECPM(ADType pADType, string pEventData)
    {
        if (string.IsNullOrEmpty(pEventData)) return;
        if (!double.TryParse(pEventData, out double tECPM)) return;
        if (tECPM <= 0 || tECPM > 10000) return;//防止ecpm异常过大造成大量上报

#if GM_MODE
        LogManager.Log($"AD ecpm : ADType-{pADType}  Ecpm-{tECPM}");
#endif     
        TaskManager.Instance.StartTask(LogECPMEvent(tECPM));

        LogEvent_ADTimes(tECPM, pADType);
    }

    static readonly WaitForSeconds WaitFor = new WaitForSeconds(0.01f);
    static IEnumerator LogECPMEvent(double pECPM)
    {
        var tInstallHours = ModuleManager.UserInfo.InstallHoursCount;
        foreach (var item in mEcpmLogList)
        {
            if (tInstallHours > item.limitTime * 24) continue;
            if (!mEcpmTotal.ContainsKey(item.eventName))
            {
                mEcpmTotal.Add(item.eventName, 0);
            }
            if (!mReportCount.ContainsKey(item.eventName))
            {
                mReportCount.Add(item.eventName, 0);
            }
            if (item.isOnce == 1 && mReportCount[item.eventName] >= 1) continue;

            var tTotalEcpm = mEcpmTotal[item.eventName] + pECPM;
            int tCount = (int)(tTotalEcpm / item.targetValue);
            for (int i = 0; i < tCount + 1; i++)
            {
                if (item.isOnce == 1 && mReportCount[item.eventName] >= 1) break;
                if (tTotalEcpm >= item.targetValue)
                {
                    mReportCount[item.eventName] += 1;
                    LogEventECPM(item);
                    tTotalEcpm -= item.targetValue;
                    yield return WaitFor;
                }
                else
                {
                    mEcpmTotal[item.eventName] = tTotalEcpm;
                }
            }
        }
        DataTool.Serialize(DataKey.ECPM_LOOP_SUM, mEcpmTotal);
        DataTool.Serialize(DataKey.ECPM_REPORT_SUM, mReportCount);
    }

    static void LogEventECPM(LogEcpmEventConfig pConfig)
    {
#if GM_MODE
        LogManager.Log($"FirebaseReportManager.LogEvent: eventName-{pConfig.eventName}  targetValue-{pConfig.reportValue}  ");
#endif
        //double tTotalValue = (double)pConfig.targetValue / 1000;//ecpm 转实际价值
        bool tNoReportValue = pConfig.reportValue == 0;
        mReportPlatform = AppInfoManager.Instance.IsIOS ? pConfig.iosPlatform : pConfig.andriodPlatform;
        if (mReportPlatform.Length > 0 && mReportPlatform[0])
        {
            if (tNoReportValue)
            {
                FirebaseManager.LogAppEvent(pConfig.eventName);
            }
            else
            {
                FirebaseManager.LogAppEvent(pConfig.eventName,
                    new Parameter(ReportEventDefine.VALUE, pConfig.reportValue),
                    new Parameter(ReportEventDefine.CURRENCY, "USD"));
            }
        }
        if (mReportPlatform.Length > 1 && mReportPlatform[1])
        {
            if (tNoReportValue)
            {
                AFReportManager.LogAppEvent(pConfig.eventName);
            }
            else
            {
                AFReportManager.LogAppEvent(pConfig.eventName,
                    new Dictionary<string, string>()
                    {
                        {AFInAppEvents.REVENUE, ((decimal)pConfig.reportValue).ToString()},
                        {AFInAppEvents.CURRENCY,  "USD"}
                    });
            }
        }
        if (mReportPlatform.Length > 2 && mReportPlatform[2])
        {
            if (tNoReportValue)
            {
                FBReportManager.LogAppEvent(pConfig.eventName);
            }
            else
            {
                FBReportManager.LogAppEvent(pConfig.eventName, null,
                    new Dictionary<string, object>()
                    {
                        {ReportEventDefine.VALUE, pConfig.reportValue},
                        {ReportEventDefine.CURRENCY,  "USD"}
                    });
            }
        }

        if (pConfig.reportBQ > 0)
        {
            BQReportManager.LogAppEvent(pConfig.eventName);
        }
    }

    static void LogEvent_ADRevenue(ADEvent pEventData)
    {
        if (ModuleManager.UserInfo.InstallHoursCount > 9 * 24) return;

        if (!double.TryParse(pEventData.ADRevenue, out double tRevenue)) return;
        if (tRevenue <= 0 || tRevenue * 1000 > 10000) return;//防止ecpm异常过大造成大量上报

#if GM_MODE
        LogManager.Log($"AD Revenue : ADType-{pEventData.ADType}  Revenue-{tRevenue}");
#endif     

        AFReportManager.LogAdRevenue(pEventData, tRevenue);
        AFReportManager.LogAppEvent("ads_adview_7d", new Dictionary<string, string>()
        {
            {AFInAppEvents.REVENUE, ((decimal)tRevenue).ToString()},
            {AFInAppEvents.CURRENCY,  "USD"},
            {"af_adType" ,  pEventData.ADType.ToString()}
        });
        BQReportManager.LogAdEcpmEvent("ads_adview_7d", pEventData.ADType, ((decimal)tRevenue).ToString());
    }

    static void LogEvent_ECPM(ADType pADType, string pRevenue)
    {
        if (ModuleManager.UserInfo.InstallHoursCount > 9 * 24) return;

        if (!double.TryParse(pRevenue, out double tRevenue)) return;
        var tECPM = tRevenue * 1000;
        if (tRevenue <= 0 || tECPM > 10000) return;//防止ecpm异常过大造成大量上报

#if GM_MODE
        LogManager.Log($"AD ecpm : ADType-{pADType}  Ecpm-{tECPM}");
#endif     
        TaskManager.Instance.StartTask(LogECPMEvent(tECPM));
    }

    static void LogEvent_ADTimes(double pTotalValue, ADType pADType)
    {
        //插屏和激励播放总次数
        if (pADType != ADType.Interstitial && pADType != ADType.RewardVideo) return;
        var tADInsertTotal = ModuleManager.Statistics.GetValue(StatsID.ADInterstitial, StatsGroup.Total);
        var tADVideoTotal = ModuleManager.Statistics.GetValue(StatsID.ADVideo, StatsGroup.Total);
        var tADTimes = tADInsertTotal + tADVideoTotal;

#if GM_MODE
        LogManager.Log($"AD Times : 插屏-{tADInsertTotal}  激励-{tADVideoTotal}");
#endif

        AFReportManager.LogConfigEvent(1001, tADTimes);
    }

    static void LogEvent_ShuShuADRevenue(ADEvent pEventData)
    {
        if (!double.TryParse(pEventData.ADRevenue, out double tRevenue)) return;
        if (tRevenue <= 0 || tRevenue > 10) return;//防止ecpm异常过大

        //ShuShu 
        var properties = new Dictionary<string, object>() {
            { "revenue", tRevenue},
        };
        TDAnalytics.Track("applovin_ad_revenue_impression_level", properties);
        //TDAnalytics.Track("admob_ad_revenue", properties);
        //TDAnalytics.Track("ironsource_ad_revenue_impression_level", properties);
    }
#if GM_MODE
    public static void GMLogECPMEvent(ADType pADType, double pAddECPM)
    {
        if (pAddECPM <= 0 || pAddECPM > 10000) return;//防止ecpm异常过大造成大量上报

        LogManager.Log($"AD ecpm : ADType-{pADType}  Ecpm-{pAddECPM}");

        TaskManager.Instance.StartTask(LogECPMEvent(pAddECPM));
        //LogEvent_AFECPM(pAddECPM, pADType);
    }

#endif
}