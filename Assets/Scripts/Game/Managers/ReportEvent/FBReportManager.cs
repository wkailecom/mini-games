using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FBReportManager
{

    public static void Init()
    {
        //EventManager.Register(EventKey.ValidateReceiptResult, OnValidateReceiptResult);
    }

    static void OnValidateReceiptResult(EventData pEventData)
    {
        var tOrderInfo = (pEventData as ValidateReceiptResult).orderInfo;
        if (tOrderInfo.OrderType == OrderType.Valid)
        {
            LogPurchase(tOrderInfo.ProductPrice * 0.5f);
        }
    }

    public static void LogPurchase(float priceAmount, string priceCurrency = "USD", Dictionary<string, object> iapParameters = null)
    {
        if (FB.IsInitialized)
        {
            FB.LogPurchase(priceAmount, priceCurrency, iapParameters);
        }
        else
        {
            Debug.LogError("fbsdk is not init!!!");
        }
    }

    public static void LogAppEvent(string logEvent, float? valueToSum = null, Dictionary<string, object> parameters = null)
    {
        if (FB.IsInitialized)
        {
            FB.LogAppEvent(logEvent, valueToSum, parameters);
        }
        else
        {
            Debug.LogError("fbsdk is not init!!!");
        }
    }
}
