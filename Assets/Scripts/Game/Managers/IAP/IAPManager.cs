using Config;
using Game;
using LLFramework;
using System;
using System.Collections.Generic;
using ThinkingData.Analytics;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPManager : Singleton<IAPManager>, IDetailedStoreListener
{
    IStoreController mStoreController;
    IExtensionProvider mExtensions;
    string mBuyingProductID;
    Action<bool> mPurchaseCallback;
    bool mIsRestore = false;

    public bool Initialized => mStoreController != null;

    public List<IAPProductConfig> ProductsConfig;
    public void Init()
    {
        //获取是否测试设备（用于剔除非实际用户验单上报的影响）
        //CommonVariables.IsTestDevice = CommonMethod.CheckIsTestDevice();

        var tPurchasingModule = StandardPurchasingModule.Instance();
        var tConfigurationBuilder = ConfigurationBuilder.Instance(tPurchasingModule);

        var tIndexe = AppInfoManager.Instance.IsIOS ? 1 : 0;
        ProductsConfig = ConfigData.iAPProductConfig.GetByIndexes(tIndexe);
        foreach (var tConfig in ProductsConfig)
        {
            tConfigurationBuilder.AddProduct(tConfig.productID, (ProductType)tConfig.productType);

            if (tConfig.queryId == GameConst.QueryID_ADS) GameConst.ProductID_ADS = tConfig.productID;
            if (tConfig.queryId == GameConst.QueryID_102) GameConst.ProductID_102 = tConfig.productID;
        }

        UnityPurchasing.Initialize(this, tConfigurationBuilder);
    }

    public Product GetProduct(string pProductID)
    {
        if (!Initialized)
        {
            LogManager.Log("IAPManager.GetProduct: not initialized!", Color.red);
            return null;
        }

        return mStoreController.products.WithID(pProductID);
    }

    public void BuyProduct(string pProductID, Action<bool> pProductCallback = null)
    {
        BuyProduct(GetProduct(pProductID), pProductCallback);
    }

    public void BuyProduct(Product pProduct, Action<bool> pProductCallback = null)
    {
        mPurchaseCallback = pProductCallback;
        if (!Initialized)
        {
            mPurchaseCallback?.Invoke(false);
            string tHint = "Error 01: Purchase failed, \nplease log in to the Google Store or update the store version and try again.";
#if UNITY_IOS
            tHint = "Error: Purchase failed,\nlog in to Apple store first and purchase again.";
#endif        
            MessageHelp.Instance.ShowMessage(tHint);
            return;
        }

        if (pProduct == null)
        {
            mPurchaseCallback?.Invoke(false);
            MessageHelp.Instance.ShowMessage("Error: No Product.");
            return;
        }

        if (!string.IsNullOrEmpty(mBuyingProductID))
        {
            LogManager.LogError("IAPManager.BuyProduct: Existing Purchase Pending");
            return;
        }

        mBuyingProductID = pProduct.definition.id;
        mStoreController.InitiatePurchase(pProduct);
    }

    /// <summary>
    ///  恢复购买
    /// </summary>
    public void RestorePurchases()
    {
        if (!Initialized)
        {
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            var apple = mExtensions.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result, message) =>
            {
                if (result)
                {
                    MessageHelp.Instance.ShowMessage("Restore Successful");
                }
                else
                {
                    MessageHelp.Instance.ShowMessage(message);
                }
                LogManager.Log("RestorePurchases : " + result + "  message: " + message);
            });
        }
        else
        {
            MessageHelp.Instance.ShowMessage("Restore Error");
            LogManager.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    #region IStoreListener Implement
    public void OnInitialized(IStoreController pController, IExtensionProvider pExtensions)
    {
        LogManager.Log("IAP Initialized!");

        mStoreController = pController;
        mExtensions = pExtensions;
    }

    public void OnInitializeFailed(InitializationFailureReason pFailureReason)
    {
        switch (pFailureReason)
        {
            case InitializationFailureReason.AppNotKnown:
                LogManager.LogError("IAP InitializeFailed: Is your App correctly uploaded on the relevant publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                LogManager.LogError("IAP InitializeFailed: Billing disabled!");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                LogManager.LogError("IAP InitializeFailed: No products available for purchase!");
                break;
        }
    }
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        OnInitializeFailed(error);
    }

    public void OnPurchaseFailed(Product pProduct, PurchaseFailureReason pFailureReason)
    {
        mPurchaseCallback?.Invoke(false);
        mBuyingProductID = null;

        LogManager.LogError($"IAP PurchaseFailed! Product - {pProduct}  Reason - {pFailureReason}");
        MessageHelp.Instance.ShowMessage($"Error: Purchase Failure, \n{pFailureReason}.");

        switch (pFailureReason)
        {
            case PurchaseFailureReason.UserCancelled:
                //MessageHelp.Instance.ShowMessage("Error: Purchase failed,\nyou cancelled the purchase.");
                break;
            case PurchaseFailureReason.DuplicateTransaction:
                //MessageHelp.Instance.ShowMessage("Error: Purchase failed,\nthis is a repeated order.");
                break;
            case PurchaseFailureReason.PurchasingUnavailable:
            case PurchaseFailureReason.ExistingPurchasePending:
            case PurchaseFailureReason.ProductUnavailable:
            case PurchaseFailureReason.SignatureInvalid:
            case PurchaseFailureReason.PaymentDeclined:
            case PurchaseFailureReason.Unknown:
                //MessageHelp.Instance.ShowMessage("Error: Purchase failed,\nplease check the Internet connection and try again.");
                break;
            default:
                throw new ArgumentOutOfRangeException("FailureReason", pFailureReason, null);
        }
    }
    public void OnPurchaseFailed(Product pProduct, PurchaseFailureDescription failureDescription)
    {
        OnPurchaseFailed(pProduct, failureDescription.reason);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs pPurchaseEvent)
    {
        mPurchaseCallback?.Invoke(true);
        var tProduct = pPurchaseEvent.purchasedProduct;
        var tConfig = ConfigData.iAPProductConfig.GetByPrimary(tProduct.definition.id);
        if (tConfig == null)
        {
            LogManager.LogError($"IAPManager.ProcessPurchase: No IAPConfig with id {tProduct.definition.id}");
            mBuyingProductID = null;

            return PurchaseProcessingResult.Complete;
        }

        mIsRestore = string.IsNullOrEmpty(mBuyingProductID) || !tConfig.productID.Equals(mBuyingProductID);
        var tOrderInfo = new OrderInfo(tProduct, tConfig, mIsRestore);
        if (!mIsRestore)
        {
            mBuyingProductID = null;
        }

        bool tIsSkipAFValidate = !GameMethod.IsAllowLogPurchase();
        if (tIsSkipAFValidate)
        {
            LogPurchase(tOrderInfo, true, mIsRestore, true);
        }
        else
        {
            EventManager.Register(EventKey.ValidateReceiptResult, OnValidateReceiptResult);
        }

        var tEventData = EventManager.GetEventData<PurchaseSuccess>(EventKey.PurchaseSuccess);
        tEventData.orderInfo = tOrderInfo;
        tEventData.product = tProduct;
        tEventData.productConfig = tConfig;
        tEventData.isRestore = mIsRestore;
        tEventData.isSkipAFValidate = tIsSkipAFValidate;
        EventManager.Trigger(tEventData);

        return PurchaseProcessingResult.Complete;
    }

    #endregion

    void OnValidateReceiptResult(EventData pEventData)
    {
        EventManager.Unregister(EventKey.ValidateReceiptResult, OnValidateReceiptResult);

        var tEventData = pEventData as ValidateReceiptResult;

        bool isTest = AppInfoManager.Instance.IsDebug || GameVariable.IsTestDevice;
        bool isTrial = mIsRestore;
        bool isScalp = tEventData.orderInfo.OrderType != OrderType.Valid;
        LogPurchase(tEventData.orderInfo, isTest, isTrial, isScalp);
    }

    void LogPurchase(OrderInfo pOrderInfo, bool isTest, bool isTrial, bool isScalp)
    {
        string channel = "GooglePlay";
#if UNITY_IOS
        channel = "AppStore";
#endif

        //ShuShu 
        var properties = new Dictionary<string, object>() {
            { "channel", channel},
            { "bi_item_id", pOrderInfo.DefinitionID},
            { "bi_item_name", pOrderInfo.LocalizedTitle},
            { "usdprice", pOrderInfo.ProductPriceCents},
            { "price", pOrderInfo.LocalPriceCent},
            { "currency", pOrderInfo.LocalCurrency},
            { "istest", isTest},
            { "istrial", isTrial},
            { "isscalp", isScalp},
            { "bi_order_id", pOrderInfo.OrderID},
        };
        TDAnalytics.Track("trackrevenue", properties);
    }

    public string GetPriceText(Product pProduct, float pConfigPrice)
    {
        if (pProduct == null)
        {
            return "$" + pConfigPrice;
        }

        return pProduct.metadata.localizedPriceString;
    }
}