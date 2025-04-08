using Config;
using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class BQReportManager
{
    const string PROJECT_ID_KEY = "projectId";
    const string DATASET_NAME_KEY = "datasetName";
    const string TABLE_NAME_KEY = "tableName";
    const string DATA_NAME_KEY = "data";

    const string PROJECT_ID_VALUE = "wordswitch-cd2b3";
    const string DATASET_NAME_VALUE = "Cryptogram";

    public static void Init()
    {
        var tTestBaseDataReported = "TestBaseDataReported";
        if (!DataTool.GetBool(tTestBaseDataReported))
        {
            LogAppEvent("BaseData", (isSuccess) =>
            {
                if (isSuccess)
                {
                    DataTool.SetBool(tTestBaseDataReported, false);
                }
            });
        }
        if (!DataTool.GetBool(DataKey.BASE_DATA_REPORTED_KEY))
        {
            EventManager.Register(EventKey.AppsflyerCallBack, OnAppsflyerCallBack);
        }
        EventManager.Register(EventKey.ApplicationFocus, OnApplicationFocus);

        EventManager.Register(EventKey.ADClosed, OnADClosed, true);
        EventManager.Register(EventKey.VideoADRewarded, OnVideoAdRewarded, true);
        EventManager.Register(EventKey.ADShowFailed, OnADShowFailed, true);
        EventManager.Register(EventKey.PurchaseSuccess, OnPurchaseSuccess);
        EventManager.Register(EventKey.ValidateReceiptResult, OnValidateReceiptResult);
        EventManager.Register(EventKey.PropCountChange, OnPropCountChange);
        EventManager.Register(EventKey.GetRewards, OnGetRewards);

        EventManager.Register(EventKey.UIAction, OnUIAction);
        EventManager.Register(EventKey.PageBeginOpen, OnPageBeginOpen);
        EventManager.Register(EventKey.GameStart, OnGameStart);
        EventManager.Register(EventKey.GameOver, OnGameOver);

        EventManager.Register(EventKey.MiniGameStart, OnMiniGameStart);
        EventManager.Register(EventKey.MiniGameOver, OnMiniGameOver);

        ReportUserActive();
    }

    static void OnAppsflyerCallBack(EventData pEventData)
    {
        var tEventData = pEventData as AppsflyerCallBack;
        if (string.IsNullOrEmpty(tEventData?.conversionData)) return;

        var tConversionData = DataTool.StringToData<AFConversionData>(tEventData.conversionData);
        if (tConversionData != null)
        {
            ReportBaseData(tConversionData);
        }
    }

    static void OnApplicationFocus(EventData pEventData)
    {
        var tEventData = pEventData as ApplicationFocus;
        if (tEventData.focus && tEventData.loseFocusSeconds >= 300)
        {
            ReportUserActive();
        }
    }

    static void OnVideoAdRewarded(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.RewardVideo)
        {
            ReportADVideoComplete(tEventData);
        }
    }

    static void OnADClosed(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.Interstitial)
        {
            ReportADInterstitialComplete(tEventData);
        }
    }

    static void OnADShowFailed(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.RewardVideo)
        {
            ReportADVideoFail(tEventData);
        }
        else if (tEventData.ADType == ADType.Interstitial)
        {
            ReportADInterstitialFail(tEventData);
        }
    }

    static void OnPurchaseSuccess(EventData pEventData)
    {
        var tEventData = pEventData as PurchaseSuccess;
        var tOrderInfo = tEventData.orderInfo;
        var tConfig = tEventData.productConfig;

        if (tEventData.isSkipAFValidate)
        {
            tOrderInfo.SetOrderType(OrderType.Invalid);
            ReportUserPay(tOrderInfo);
        }

        ReportGetReward(GameMethod.ParseProps(tConfig.propsID, tConfig.propsCount), PropSource.Shop, tConfig.productID, tConfig.price);
    }

    static void OnValidateReceiptResult(EventData pEventData)
    {
        var tEventData = pEventData as ValidateReceiptResult;
        ReportUserPay(tEventData.orderInfo);
    }

    static void OnUIAction(EventData pEventData)
    {
        var tEventData = pEventData as UIAction;
        if (tEventData.isReport)
        {
            ReportPageShow(tEventData);
        }
    }
    static void OnPageBeginOpen(EventData pEventData)
    {
        var tEventData = pEventData as PageOperation;
        //if (tEventData.pageID == PageID.RevivePopup)
        //{
        //    ReportLevelFailed();
        //}
    }

    static void OnPropCountChange(EventData pEventData)
    {
        var tEventData = pEventData as PropCountChange;
        if (tEventData.changedCount < 0)
        {
            ReportPropExpend(tEventData.propID, Math.Abs(tEventData.changedCount));
        }
    }

    static void OnGetRewards(EventData pEventData)
    {
        var tEventData = pEventData as GetRewards;
        if (null != tEventData.rewards && tEventData.source != PropSource.Shop && tEventData.source != PropSource.IngamePurchase)
        {
            ReportGetReward(tEventData.rewards, tEventData.source, string.Empty, 0);
        }
    }

    static void OnGameStart(EventData pEventData)
    {
        var tEventData = pEventData as GameStart;
        if (tEventData.gameModeType != GameModeType.Guide)
        {
            ReportLevelEnter();
        }
    }

    static void OnGameOver(EventData pEventData)
    {
        var tEventData = pEventData as GameOver;
        if (tEventData.gameModeType != GameModeType.Guide)
        {
            ReoprtLevelPass(tEventData.isSuccess);
        }
    }

    static void OnMiniGameStart(EventData pEventData)
    {
        var tEventData = pEventData as MiniGameStart;

        ReportMiniLevelEnter();
    }

    static void OnMiniGameOver(EventData pEventData)
    {
        var tEventData = pEventData as MiniGameOver;

        if (tEventData.isSuccess)
        {
            ReoprtMiniLevelPass();
        }
        else
        {
            ReportMiniLevelFailed();
        }
    }


    #region 上报
    public static void Report(Dictionary<string, object> pData, Action<bool> pCallback = null, Action callback = null)
    {
        NetAPI.PostJSON(APPDefine.BQURL, pData, (pSuccess, pResponseData) =>
        {
            if (pSuccess)
            {
                pCallback?.Invoke(true);
                callback?.Invoke();
                LogManager.Log("Report: " + pData[TABLE_NAME_KEY] + " Success");
            }
            else
            {
                callback?.Invoke();
                BQRetryReport.Instance.AddPostFormReport(pData, pCallback);
                LogManager.Log($"BQReportManager.Report Error:{pData[TABLE_NAME_KEY]}- {pResponseData}");
            }
        });
    }

    public static void ReportData(string pTableName, Dictionary<string, object> pData, Action<bool> pCallback = null, Action callback = null)
    {
        if (LogManager.EnableLog)
        {
            LogManager.Log($"Report: [{pTableName}] " + DataTool.DataToString(pData), Color.green);
        }
        pData.Add(ReportEventDefine.DataInsertTime_Key, ReportEventDefine.DataInsertTime_Value);//统一补充插入时间

        var tReportData = new Dictionary<string, object>();
        tReportData.Add(PROJECT_ID_KEY, PROJECT_ID_VALUE);
        tReportData.Add(DATASET_NAME_KEY, DATASET_NAME_VALUE);
        tReportData.Add(TABLE_NAME_KEY, pTableName);
        tReportData.Add(DATA_NAME_KEY, pData);

#if !UNITY_EDITOR
        Report(tReportData, pCallback, callback);
#endif
    }

    public static void LogAppEvent(string eventName, Action<bool> pCallback = null)
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);
        tData.Add("event", eventName);

        ReportData("LogAppEvent", tData, pCallback);
    }

    public static void LogAdEcpmEvent(string eventName, ADType pADType, string pADValue)
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);
        tData.Add("event", eventName);
        tData.Add("adType", pADType.ToString());
        tData.Add("adValue", pADValue);

        ReportData("LogAdEcpmEvent", tData);
    }


    static void ReportBaseData(AFConversionData pConversionData)
    {
        EventManager.Unregister(EventKey.AppsflyerCallBack, OnAppsflyerCallBack);

        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppID_Key, ReportEventDefine.AppID_Value);
        tData.Add(ReportEventDefine.DeviceCountry_Key, ReportEventDefine.DeviceCountry_Value);
        tData.Add(ReportEventDefine.FirstInstallVersion_Key, ReportEventDefine.AppVersion_Value);
        tData.Add(ReportEventDefine.DeviceModel_Key, ReportEventDefine.DeviceModel_Value);
        tData.Add(ReportEventDefine.DeviceLanguage_Key, ReportEventDefine.DeviceLanguage_Value);
        tData.Add(ReportEventDefine.InstallTime_Key, ReportEventDefine.InstallTime_Value);
        tData.Add(ReportEventDefine.AFMediaSource_Key, pConversionData.media_source);
        tData.Add(ReportEventDefine.AFChannelType_Key, pConversionData.af_status.Contains("Non") ? 1 : 0);
        tData.Add(ReportEventDefine.AFChannel_Key, pConversionData.af_channel);
        tData.Add(ReportEventDefine.AFCampaignID_Key, string.IsNullOrEmpty(pConversionData.campaign_id) ? pConversionData.campaign : pConversionData.campaign_id);
        tData.Add(ReportEventDefine.AFCampaign_Key, pConversionData.campaign);
        tData.Add(ReportEventDefine.AFCampaignName_Key, pConversionData.af_campaign_name);
        tData.Add(ReportEventDefine.AFCID_Key, pConversionData.af_c_id);
        tData.Add(ReportEventDefine.AFADID_Key, pConversionData.ad_id);
        tData.Add(ReportEventDefine.AFAFADID_Key, pConversionData.af_ad_id);

        ReportData("BaseData", tData, (isSuccess) =>
        {
            if (isSuccess)
            {
                DataTool.SetBool(DataKey.BASE_DATA_REPORTED_KEY, true);
            }
        });
    }

    static void ReportUserActive()
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);
        tData.Add(ReportEventDefine.DeviceSystem_Key, ReportEventDefine.DeviceSystem_Value);
        tData.Add(ReportEventDefine.FirebaseUserID_Key, ReportEventDefine.FirebaseUserID_Value);
        tData.Add(ReportEventDefine.IDFA_Key, ReportEventDefine.IDFA_Value);
        tData.Add(ReportEventDefine.IDFV_Key, ReportEventDefine.IDFV_Value);
        tData.Add(ReportEventDefine.AppsflyerID_Key, ReportEventDefine.AppsflyerID_Value);
        tData.Add(ReportEventDefine.MADID_Key, ReportEventDefine.MADID_Value);
        tData.Add(ReportEventDefine.ResVersion_Key, ReportEventDefine.ResVersion_Value);

        //tData.Add(ReportEventDefine.EndlessOverTimes_Key, ReportEventDefine.EndlessOverTimes_Value);
        //tData.Add(ReportEventDefine.DailyOverTimes_Key, ReportEventDefine.DailyOverTimes_Value);
        //tData.Add(ReportEventDefine.LeftHintCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Hint));
        //tData.Add(ReportEventDefine.UseLanguage_Key, ReportEventDefine.UseLanguage_Value);
        //tData.Add(ReportEventDefine.NormalLevel_Key, ReportEventDefine.NormalLevel_Value);

        ReportData("UserActiveNEW", tData);
    }

    static void ReportADVideoComplete(ADEvent pADEvent)
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        //tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        //tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNum_Value);
        //tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        //tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);
        //tData.Add(ReportEventDefine.LevelProgress_Key, ReportEventDefine.LevelProgress_Value);
        //tData.Add(ReportEventDefine.MistakeTimes_Key, ReportEventDefine.MistakeTimes_Value);
        //tData.Add(ReportEventDefine.LeftHintCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Hint));

        tData.Add(ReportEventDefine.ADShowReason_Key, ReportEventDefine.ADShowReason_Value(pADEvent.showReason));
        tData.Add(ReportEventDefine.InstallDays_Key, ReportEventDefine.InstallDays_Value);
        tData.Add(ReportEventDefine.ActiveDays_Key, ReportEventDefine.ActiveDays_Value);
        tData.Add(ReportEventDefine.ADCount_Key, ReportEventDefine.ADVideoTimes_Value);
        tData.Add(ReportEventDefine.ADPlatform_Key, pADEvent.platform);
        tData.Add(ReportEventDefine.ADPlacementID_Key, pADEvent.placementID);
        tData.Add(ReportEventDefine.ADRewardADID_Key, pADEvent.ADID);
        tData.Add(ReportEventDefine.ADEcpm_Key, ADManager.Instance.VideoECPM);

        ReportData("ad_video_complete", tData);
    }

    static void ReportADVideoFail(ADEvent pADEvent)
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        //tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        //tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNum_Value);
        //tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        //tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);
        //tData.Add(ReportEventDefine.LevelProgress_Key, ReportEventDefine.LevelProgress_Value);
        //tData.Add(ReportEventDefine.MistakeTimes_Key, ReportEventDefine.MistakeTimes_Value);
        //tData.Add(ReportEventDefine.LeftHintCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Hint));

        tData.Add(ReportEventDefine.ADShowReason_Key, ReportEventDefine.ADShowReason_Value(pADEvent.showReason));
        tData.Add(ReportEventDefine.InstallDays_Key, ReportEventDefine.InstallDays_Value);
        tData.Add(ReportEventDefine.ActiveDays_Key, ReportEventDefine.ActiveDays_Value);
        tData.Add(ReportEventDefine.ADCount_Key, ReportEventDefine.ADVideoTimes_Value);
        tData.Add(ReportEventDefine.ADPlatform_Key, pADEvent.platform);
        tData.Add(ReportEventDefine.ADPlacementID_Key, pADEvent.placementID);
        tData.Add(ReportEventDefine.ADRewardADID_Key, pADEvent.ADID);
        tData.Add(ReportEventDefine.ADEcpm_Key, ADManager.Instance.VideoECPM);

        ReportData("ad_video_fail", tData);
    }

    static void ReportADInterstitialComplete(ADEvent pADEvent)
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        //tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        //tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNum_Value);
        //tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        //tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);
        //tData.Add(ReportEventDefine.LevelProgress_Key, ReportEventDefine.LevelProgress_Value);
        //tData.Add(ReportEventDefine.MistakeTimes_Key, ReportEventDefine.MistakeTimes_Value);
        //tData.Add(ReportEventDefine.LeftHintCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Hint));

        tData.Add(ReportEventDefine.ADShowReason_Key, ReportEventDefine.ADShowReason_Value(pADEvent.showReason));
        tData.Add(ReportEventDefine.InstallDays_Key, ReportEventDefine.InstallDays_Value);
        tData.Add(ReportEventDefine.ActiveDays_Key, ReportEventDefine.ActiveDays_Value);
        tData.Add(ReportEventDefine.ADCount_Key, ReportEventDefine.ADVideoTimes_Value);
        tData.Add(ReportEventDefine.ADPlatform_Key, pADEvent.platform);
        tData.Add(ReportEventDefine.ADPlacementID_Key, pADEvent.placementID);
        tData.Add(ReportEventDefine.ADRewardADID_Key, pADEvent.ADID);
        tData.Add(ReportEventDefine.ADEcpm_Key, ADManager.Instance.InterstitialECPM);

        ReportData("ad_interstitial_complete", tData);
    }

    static void ReportADInterstitialFail(ADEvent pADEvent)
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        //tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        //tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNum_Value);
        //tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        //tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);
        //tData.Add(ReportEventDefine.LevelProgress_Key, ReportEventDefine.LevelProgress_Value);
        //tData.Add(ReportEventDefine.MistakeTimes_Key, ReportEventDefine.MistakeTimes_Value);
        //tData.Add(ReportEventDefine.LeftHintCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Hint));

        tData.Add(ReportEventDefine.ADShowReason_Key, ReportEventDefine.ADShowReason_Value(pADEvent.showReason));
        tData.Add(ReportEventDefine.InstallDays_Key, ReportEventDefine.InstallDays_Value);
        tData.Add(ReportEventDefine.ActiveDays_Key, ReportEventDefine.ActiveDays_Value);
        tData.Add(ReportEventDefine.ADCount_Key, ReportEventDefine.ADVideoTimes_Value);
        tData.Add(ReportEventDefine.ADPlatform_Key, pADEvent.platform);
        tData.Add(ReportEventDefine.ADPlacementID_Key, pADEvent.placementID);
        tData.Add(ReportEventDefine.ADRewardADID_Key, pADEvent.ADID);
        tData.Add(ReportEventDefine.ADEcpm_Key, ADManager.Instance.InterstitialECPM);

        ReportData("ad_interstitial_failed", tData);
    }

    static void ReportADBannerComplete(ADEvent pADEvent)
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        //tData.Add(ReportEventDefine.ADShowReason_Key, ReportEventDefine.ADShowReason_Value(pADEvent.showReason));
        tData.Add(ReportEventDefine.InstallDays_Key, ReportEventDefine.InstallDays_Value);
        tData.Add(ReportEventDefine.ActiveDays_Key, ReportEventDefine.ActiveDays_Value);
        tData.Add(ReportEventDefine.ADCount_Key, ReportEventDefine.ADVideoTimes_Value);
        tData.Add(ReportEventDefine.ADPlatform_Key, pADEvent.platform);
        tData.Add(ReportEventDefine.ADPlacementID_Key, pADEvent.placementID);
        tData.Add(ReportEventDefine.ADRewardADID_Key, pADEvent.ADID);
        tData.Add(ReportEventDefine.ADEcpm_Key, ADManager.Instance.InterstitialECPM);

        ReportData("ad_banner_completed", tData);
    }

    static void ReportUserPay(OrderInfo pOrderInfo)
    {
        pOrderInfo ??= new OrderInfo();

        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        tData.Add(ReportEventDefine.PayProductID_Key, pOrderInfo.ProductID);
        tData.Add(ReportEventDefine.PayProductPrice_Key, pOrderInfo.ProductPrice.ToString());
        tData.Add(ReportEventDefine.PayReason_Key, GameVariable.UserBuyFrom);

        //tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        //tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNum_Value);
        //tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        //tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);
        //tData.Add(ReportEventDefine.BuyLevel_Key, ModuleManager.LevelInfo.EndlessLevelID);
        //tData.Add(ReportEventDefine.TotalMistakeTimes_Key, ReportEventDefine.TotalMistakeTimes_Value);

        //tData.Add(ReportEventDefine.LeftHealthCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Health));//没有购买项，单独处理

        //List<PropID> tPropIDs = new List<PropID> {
        //    PropID.Hint,
        //    PropID.ScrewExtraSlot, PropID.ScrewHammer, PropID. ScrewExtraBox,
        //    PropID.Jam3DShuffle,PropID.Jam3DReplace, PropID.Jam3DRevert,
        //};

        //foreach (var tPropId in tPropIDs)
        //{
        //    int tBuyLeftCount = ReportEventDefine.LeftPropCount_Value(tPropId);
        //    int tLeftCount = tBuyLeftCount - GameMethod.GetIAPProductPropCount(pOrderInfo.ProductConfig, tPropId);

        //    (string, string) tName = tPropId switch
        //    {
        //        PropID.Hint => (ReportEventDefine.LeftHintCount_Key, ReportEventDefine.BuyLeftHintCount_Key),
        //        PropID.ScrewExtraSlot => (ReportEventDefine.LeftExSlotCount_Key, ReportEventDefine.BuyLeftExSlotCount_Key),
        //        PropID.ScrewHammer => (ReportEventDefine.LeftHammerCount_Key, ReportEventDefine.BuyLeftHammerCount_Key),
        //        PropID.ScrewExtraBox => (ReportEventDefine.leftExBoxCount_Key, ReportEventDefine.BuyleftExBoxCount_Key),
        //        PropID.Jam3DShuffle => (ReportEventDefine.LeftShuffleCount_Key, ReportEventDefine.BuyLeftShuffleCount_Key),
        //        PropID.Jam3DReplace => (ReportEventDefine.LeftReplaceCount_Key, ReportEventDefine.BuyLeftReplaceCount_Key),
        //        PropID.Jam3DRevert => (ReportEventDefine.LeftRevertCount_Key, ReportEventDefine.BuyLeftRevertCount_Key),
        //        _ => (string.Empty, string.Empty),
        //    };
        //    if (!string.IsNullOrEmpty(tName.Item1))
        //    {
        //        tData.Add(tName.Item1, tLeftCount);
        //    }
        //    if (!string.IsNullOrEmpty(tName.Item2))
        //    {
        //        tData.Add(tName.Item2, tBuyLeftCount);
        //    }
        //}

        tData.Add(ReportEventDefine.InstallDays_Key, ReportEventDefine.InstallDays_Value);
        tData.Add(ReportEventDefine.PayIntervalDays_Key, ReportEventDefine.PayIntervalDays_Value);
        tData.Add(ReportEventDefine.PayTimesTotal_Key, ReportEventDefine.PayTimesTotal_Value);
        tData.Add(ReportEventDefine.PayOrderID_Key, pOrderInfo.OrderID);
        tData.Add(ReportEventDefine.PayOrderType_Key, (int)pOrderInfo.OrderType);
        tData.Add(ReportEventDefine.PayOrderCreateTime_Key, pOrderInfo.OrderCreateTime.ToString());

        ReportData("userBuyNew", tData);
    }

    static void ReportPageShow(UIAction pUIAction)
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNum_Value);
        tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);

        tData.Add(ReportEventDefine.UIPage_Key, pUIAction.UIPageName);
        tData.Add(ReportEventDefine.UIName_Key, pUIAction.UIName);
        tData.Add(ReportEventDefine.UIAction_Key, (int)pUIAction.actionType);

        if (pUIAction.ADType == ADType.Interstitial)
        {
            tData.Add(ReportEventDefine.IsADValid_Key, ADManager.Instance.IsRewardVideoReady ? 1 : 0);
        }
        else if (pUIAction.ADType == ADType.RewardVideo)
        {
            tData.Add(ReportEventDefine.IsADValid_Key, ADManager.Instance.IsInterstitialReady ? 1 : 0);
        }

        ReportData("Pageshow", tData);
    }

    static void ReportGetReward(List<PropData> pRewardData, PropSource pSource, string pProductID, float pPrice)
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        //tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        //tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNum_Value);
        //tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        //tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);

        //tData.Add(ReportEventDefine.PayProductID_Key, pProductID);
        //tData.Add(ReportEventDefine.PayProductPrice_Key, pPrice.ToString());
        //tData.Add(ReportEventDefine.RewardSource_Key, ReportEventDefine.PropReason_Value(pSource));

        //foreach (var tProp in pRewardData)
        //{
        //    int tGetCount = tProp.Count;
        //    string tName = tProp.ID switch
        //    {
        //        PropID.Health => ReportEventDefine.GetHealthCount_Key,
        //        PropID.Hint => ReportEventDefine.GetHintCount_Key,
        //        PropID.ScrewExtraSlot => ReportEventDefine.GetExSlotCount_Key,
        //        PropID.ScrewHammer => ReportEventDefine.GetHammerCount_Key,
        //        PropID.ScrewExtraBox => ReportEventDefine.GetExBoxCount_Key,
        //        PropID.Jam3DShuffle => ReportEventDefine.GetShuffleCount_Key,
        //        PropID.Jam3DReplace => ReportEventDefine.GetReplaceCount_Key,
        //        PropID.Jam3DRevert => ReportEventDefine.GetRevertCount_Key,
        //        _ => string.Empty,
        //    };
        //    if (!string.IsNullOrEmpty(tName))
        //    {
        //        tData.Add(tName, tGetCount);
        //    }
        //}

        //tData.Add(ReportEventDefine.LeftHealthCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Health));
        //tData.Add(ReportEventDefine.LeftHintCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Hint));
        //tData.Add(ReportEventDefine.LeftExSlotCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.ScrewExtraSlot));
        //tData.Add(ReportEventDefine.LeftHammerCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.ScrewHammer));
        //tData.Add(ReportEventDefine.leftExBoxCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.ScrewExtraBox));
        //tData.Add(ReportEventDefine.LeftShuffleCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Jam3DShuffle));
        //tData.Add(ReportEventDefine.LeftReplaceCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Jam3DReplace));
        //tData.Add(ReportEventDefine.LeftRevertCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Jam3DRevert));

        //tData.Add(ReportEventDefine.GetPropCount_Key, ReportEventDefine.GetPropCount_Value(pRewardData));
        //tData.Add(ReportEventDefine.LeftPropCount_Key, ReportEventDefine.LeftPropCount_Value(pRewardData));

        ReportData("getreward_new", tData);
    }

    static void ReportPropExpend(PropID pPropID, int pConsumeCount)
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        //tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        //tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNum_Value);
        //tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        //tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);

        //tData.Add(ReportEventDefine.LevelProgress_Key, ReportEventDefine.LevelProgress_Value);
        //tData.Add(ReportEventDefine.MistakeTimes_Key, ReportEventDefine.MistakeTimes_Value);

        //tData.Add(ReportEventDefine.PropName_Key, ReportEventDefine.PropName_Value(pPropID));
        ////tData.Add(ReportEventDefine.LeftPropCount_Key, ReportEventDefine.LeftPropCount_Value(pPropID));
        ////tData.Add(ReportEventDefine.LeftHealthCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Health)); 
        //tData.Add(ReportEventDefine.LeftHintCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Hint));
        //tData.Add(ReportEventDefine.LeftExSlotCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.ScrewExtraSlot));
        //tData.Add(ReportEventDefine.LeftHammerCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.ScrewHammer));
        //tData.Add(ReportEventDefine.leftExBoxCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.ScrewExtraBox));
        //tData.Add(ReportEventDefine.LeftShuffleCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Jam3DShuffle));
        //tData.Add(ReportEventDefine.LeftReplaceCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Jam3DReplace));
        //tData.Add(ReportEventDefine.LeftRevertCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Jam3DRevert));

        //if (pPropID == PropID.Hint)
        //{
        //    tData.Add(ReportEventDefine.HinthWord_Key, ReportEventDefine.HinthWord_Value);
        //}

        ReportData("HintConsumeNew", tData);
    }

    static void ReportLevelEnter()
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        //tData.Add(ReportEventDefine.LevelUniqueID_Key, ReportEventDefine.LevelUniqueID_Value);
        //tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        //tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNum_Value);
        //tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        //tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);

        //tData.Add(ReportEventDefine.LeftHintCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Hint));
        //tData.Add(ReportEventDefine.LeftHealthCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Health));

        ReportData("LevelEntry", tData);
    }

    static void ReportLevelFailed()
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        //tData.Add(ReportEventDefine.LevelUniqueID_Key, ReportEventDefine.LevelUniqueID_Value);
        //tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        //tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNum_Value);
        //tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        //tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);

        //tData.Add(ReportEventDefine.DeathWord_Key, ReportEventDefine.DeathWord_Value);

        ReportData("LevelFailed", tData);
    }

    static void ReoprtLevelPass(bool pLevelPass)
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        //tData.Add(ReportEventDefine.LevelUniqueID_Key, ReportEventDefine.LevelUniqueID_Value);
        //tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        //tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNumOver_Value);
        //tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        //tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);

        //tData.Add(ReportEventDefine.EndlessOverTimes2_Key, ReportEventDefine.EndlessOverTimes_Value);
        //tData.Add(ReportEventDefine.DailyOverTimes2_Key, ReportEventDefine.DailyOverTimes_Value);
        //tData.Add(ReportEventDefine.LeftHintCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Hint));
        //tData.Add(ReportEventDefine.LeftHealthCount_Key, ReportEventDefine.LeftPropCount_Value(PropID.Health));
        //tData.Add(ReportEventDefine.PlayTime_Key, ReportEventDefine.PlayTime_Value);
        //tData.Add(ReportEventDefine.MistakeTimes_Key, ReportEventDefine.MistakeTimes_Value);

        //tData.Add(ReportEventDefine.EmptyCount_Key, ReportEventDefine.EmptyCount_Value);
        //tData.Add(ReportEventDefine.SingleLockCount_Key, ReportEventDefine.SingleLockCount_Value);
        //tData.Add(ReportEventDefine.DoubleLockCount_Key, ReportEventDefine.DoubleLockCount_Value);
        //tData.Add(ReportEventDefine.RetryCount_Key, ReportEventDefine.RetryCountOver_Value);
        //tData.Add(ReportEventDefine.LevelDifficulty_Key, ReportEventDefine.LevelDifficulty_Value);
        //tData.Add(ReportEventDefine.LetterCount_Key, ReportEventDefine.LetterCount_Value);
        //tData.Add(ReportEventDefine.WordCount_Key, ReportEventDefine.WordCount_Value);
        //tData.Add(ReportEventDefine.IsWinStreak_Key, ReportEventDefine.IsWinStreak_Value);


        ReportData("LevelPass", tData);
    }

    static void ReportSectionReplay()
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        //tData.Add(ReportEventDefine.LevelUniqueID_Key, ReportEventDefine.LevelUniqueID_Value);
        //tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        //tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNum_Value);
        //tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        //tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);

        //tData.Add("inputTimestamp", ""); //TODO 死亡单词 
        //tData.Add("emptyProgress", ""); //TODO 目前的已键入empties/总empties 
        //tData.Add("wordProgress", ""); //TODO 目前已完型的单词/总单词数量 
        //tData.Add("wordCompleted", ""); //TODO 现在有完整填写的单词吗？有就按完形顺序依次罗列，没有就是0
        //tData.Add(ReportEventDefine.UnlockCount_Key, ReportEventDefine.UnlockCount_Value);
        //tData.Add(ReportEventDefine.NoUnlockCount_Key, ReportEventDefine.NoUnlockCount_Value);
        //tData.Add(ReportEventDefine.UsedHintCount_Key, ReportEventDefine.UsedHintCount_Value);
        //tData.Add(ReportEventDefine.MistakeTimes_Key, ReportEventDefine.MistakeTimes_Value);

        ReportData("SectionReplay", tData);
    }

    static void ReportTutorial()
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNum_Value);
        tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);

        tData.Add("tutSteps", ""); //TODO 一共7步教学，分散在前15关，每一个步骤完成报数，从1开始报 

        ReportData("NewplayerGuide", tData);
    }

    //小游戏打点列表
    static void ReportMiniLevelEnter()
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNum_Value);
        tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);

        tData.Add(ReportEventDefine.LeftPropCount_Key, ReportEventDefine.MiniGameLeftPropCount_Value());

        ReportData("miniLevelEntry", tData);
    }

    static void ReportMiniLevelFailed()
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNum_Value);
        tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);

        tData.Add(ReportEventDefine.LeftPropCount_Key, ReportEventDefine.MiniGameLeftPropCount_Value());

        ReportData("miniLevelFailed", tData);
    }

    static void ReoprtMiniLevelPass()
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, ReportEventDefine.UserGroup_Value);
        tData.Add(ReportEventDefine.UserID_Key, ReportEventDefine.UserID_Value);
        tData.Add(ReportEventDefine.AppVersion_Key, ReportEventDefine.AppVersion_Value);

        tData.Add(ReportEventDefine.SceneName_Key, ReportEventDefine.SceneName_Value);
        tData.Add(ReportEventDefine.LevelNum_Key, ReportEventDefine.LevelNum_Value);
        tData.Add(ReportEventDefine.SeedNum_Key, ReportEventDefine.SeedNum_Value);
        tData.Add(ReportEventDefine.IssueNum_Key, ReportEventDefine.IssueNum_Value);

        tData.Add(ReportEventDefine.LeftPropCount_Key, ReportEventDefine.MiniGameLeftPropCount_Value());

        ReportData("miniLevelPass", tData);
    }

    #endregion

    // Warning:以下字段的取值不要依赖其他系统，因为其他系统可能尚未初始化
    public static void ReportException(string pCondition, string pStackTrace)
    {
        var tData = new Dictionary<string, object>();
        tData.Add(ReportEventDefine.UserGroup_Key, AppInfoManager.Instance.UserGroupName);
        tData.Add(ReportEventDefine.UserID_Key, AppInfoManager.Instance.UserID);
        tData.Add(ReportEventDefine.AppVersion_Key, AppInfoManager.Instance.AppVersion);
        tData.Add(ReportEventDefine.AppName_Key, AppInfoManager.Instance.AppName);
        tData.Add(ReportEventDefine.AppOS_Key, AppInfoManager.Instance.AppOS);
        tData.Add(ReportEventDefine.AppBuildType_Key, AppInfoManager.Instance.AppBuildType);
        tData.Add(ReportEventDefine.ResVersion_Key, AppInfoManager.Instance.ResVersion);
        tData.Add("errorType", pCondition);
        tData.Add("errorLog", pStackTrace);

        ReportData("UnityLog", tData);
    }

}