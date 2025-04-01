using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;
using System;
using Game.Sdk;
using LLFramework;
using ThinkingData.Analytics;

namespace Game.Sdk
{
    public class AppsflyerManager : MonoSingleton<AppsflyerManager>, IAppsFlyerConversionData, IAppsFlyerValidateReceipt
    {

        private bool isDebug = false;

        public void Start()
        {
            EventManager.Register(EventKey.PurchaseSuccess, OnValidateReceipt);
        }

        public void Init()
        {
            AppsFlyer.setIsDebug(isDebug);
            //Apple环境（生产环境或沙盒环境）中的应用内购票据验证。默认值为“false”。
            //#if UNITY_IOS && !UNITY_EDITOR && GM_MODE
            //        bool isSandbox = true;
            //        AppsFlyeriOS.setUseReceiptValidationSandbox(isSandbox);
            //#endif

            string tAppID = string.Empty;
#if UNITY_IOS && !UNITY_EDITOR
            tAppID = GameConst.IosAppID;
#elif UNITY_ANDROID && !UNITY_EDITOR
            tAppID = Application.identifier;
#endif
            AppsFlyer.initSDK(GameConst.AppsflyerDevkey, tAppID, this);
            AppsFlyer.enableTCFDataCollection(true);
        }

        public void StartSDK()
        {
            AppsFlyer.setCustomerUserId(AppInfoManager.Instance.UserID);
            AppsFlyer.startSDK();

            AppsFlyerAdRevenue.start();
            //AppsFlyerAdRevenue.setIsDebug(isDebug);
        }

        #region 接口函数

        // Mark AppsFlyer CallBacks
        public void onConversionDataSuccess(string conversionData)
        {
            AppsFlyer.AFLog("didReceiveConversionData", conversionData);
            Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
            // add deferred deeplink logic here

            if (conversionDataDictionary != null && conversionDataDictionary.ContainsKey("media_source"))
            {
                GameVariable.AFMediaSource = conversionDataDictionary["media_source"].ToString();
            }

            //ShuShu
            TDAnalytics.Track("sendagentattribute", conversionDataDictionary);

            //if (!DataTool.HasKey(DataKey.AF_CallBackData) && !string.IsNullOrEmpty(conversionData))
            //{
            //    DataTool.SetString(DataKey.AF_CallBackData, conversionData);
            //}

            var tEventData = EventManager.GetEventData<AppsflyerCallBack>(EventKey.AppsflyerCallBack);
            tEventData.conversionData = conversionData;
            EventManager.Trigger(tEventData);
        }

        public void onConversionDataFail(string error)
        {
            AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
        }

        public void onAppOpenAttribution(string attributionData)
        {
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
            Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
            // add direct deeplink logic here
        }

        public void onAppOpenAttributionFailure(string error)
        {
            AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
        }

        public void didFinishValidateReceipt(string result)
        {
            mValidatingOrder.SetOrderType(OrderType.Valid);

            var tEventData = EventManager.GetEventData<ValidateReceiptResult>(EventKey.ValidateReceiptResult);
            tEventData.orderInfo = mValidatingOrder;
            EventManager.Trigger(tEventData);
        }

        public void didFinishValidateReceiptWithError(string error)
        {
            mValidatingOrder.SetOrderType(OrderType.Invalid);

            var tEventData = EventManager.GetEventData<ValidateReceiptResult>(EventKey.ValidateReceiptResult);
            tEventData.orderInfo = mValidatingOrder;
            EventManager.Trigger(tEventData);
        }

        #endregion
        OrderInfo mValidatingOrder;
        private void OnValidateReceipt(EventData pEventData)
        {
            var tEventData = pEventData as PurchaseSuccess;
            //跳过验单
            if (tEventData.isSkipAFValidate)
            {
                return;
            }

            mValidatingOrder = tEventData.orderInfo;

#if UNITY_IOS && !UNITY_EDITOR
        AppsFlyer.validateAndSendInAppPurchase(
            tEventData.product.definition.id, 
            tEventData.productConfig.price.ToString(), 
            "USD",
            tEventData.product.transactionID,
            new Dictionary<string, string>
            {
                {"WordCrypto", "WordCrypto"}
            }, this);
#elif UNITY_ANDROID && !UNITY_EDITOR
        var recptToJSON = (Dictionary<string, object>)AFMiniJSON.Json.Deserialize(tEventData.product.receipt);
        if (recptToJSON == null || !recptToJSON.ContainsKey("Payload"))
        {
            Debug.LogError("rectToJSON is invalid. Product.receipt : " + tEventData.product.receipt);
            return;
        }

        var receiptPayload = (Dictionary<string, object>)AFMiniJSON.Json.Deserialize((string)recptToJSON["Payload"]);
        if (receiptPayload == null || !receiptPayload.ContainsKey("signature") || !receiptPayload.ContainsKey("json"))
        {
            Debug.LogError("receiptPayload is invalid. Payload : " + (string)recptToJSON["Payload"]);
            return;
        }

        AppsFlyer.validateAndSendInAppPurchase(
            GameConst.GooglePlayPublicKey,
            (string)receiptPayload["signature"],
            (string)receiptPayload["json"],
            tEventData.productConfig.price.ToString(),
            "USD",
            new Dictionary<string, string>
            {
                { "WordCrypto", "WordCrypto" }
            }, this);
#endif
        }

    }
}
