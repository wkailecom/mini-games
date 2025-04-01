using Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Purchasing;

public class OrderInfo
{
    public string OrderID { get; private set; }
    public DateTime OrderCreateTime { get; private set; }
    public OrderType OrderType { get; private set; }
    public IAPProductConfig ProductConfig { get; private set; }
    public string ProductID => ProductConfig.productID;
    public float ProductPrice => ProductConfig.price;


    public string DefinitionID { get; private set; }
    public string LocalizedTitle { get; private set; }
    public int ProductPriceCents { get; private set; }
    public int LocalPriceCent { get; private set; }
    public string LocalCurrency { get; private set; }
    public bool IsRestoreOrder { get; private set; }

    public OrderInfo() { }

    public OrderInfo(Product pProduct, IAPProductConfig pConfig, bool pIsRestoreOrder = false)
    {
        OrderID = GetProductOrderID(pProduct);
        OrderCreateTime = DateTime.Now;
        OrderType = OrderType.PendingValidate;
        ProductConfig = pConfig;

        DefinitionID = pProduct.definition.id;
        LocalizedTitle = pProduct.metadata.localizedTitle;
        ProductPriceCents = (int)Math.Round(ProductPrice * 100, MidpointRounding.ToEven);//0.5舍
        LocalPriceCent = Mathf.RoundToInt((float)pProduct.metadata.localizedPrice * 100);
        LocalCurrency = pProduct.metadata.isoCurrencyCode;
        IsRestoreOrder = pIsRestoreOrder;
    }

    public void SetOrderType(OrderType pOrderType)
    {
        OrderType = pOrderType;
    }

    string GetProductOrderID(Product pProduct)
    {
#if UNITY_EDITOR
        return "Editor";
#endif

#if UNITY_IOS
        return pProduct.transactionID;
#endif

        var tReceipt = JsonConvert.DeserializeObject<Dictionary<string, object>>(pProduct.receipt);
        if (tReceipt == null || !tReceipt.ContainsKey("Payload"))
        {
            LogManager.LogError($"GetProductOrderID: Receipt is invalid. Receipt - {pProduct.receipt}");
            return string.Empty;
        }

        var tPayload = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)tReceipt["Payload"]);
        if (tPayload == null || !tPayload.ContainsKey("json"))
        {
            LogManager.LogError($"GetProductOrderID: Payload is invalid. Payload - {(string)tReceipt["Payload"]}");
            return string.Empty;
        }

        var tJson = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)tPayload["json"]);
        if (tJson == null || !tJson.ContainsKey("orderId"))
        {
            LogManager.LogError($"GetProductOrderID: Json is invalid. Json - {(string)tReceipt["json"]}");
            return string.Empty;
        }

        return (string)tJson["orderId"];
    }

    #region 序列化 & 反序列化
    public string Serialize()
    {
        StringBuilder tResult = new StringBuilder();
        tResult.Append(OrderID).Append(',')
               .Append(OrderCreateTime.Ticks).Append(',')
               .Append((int)OrderType).Append(',')
               .Append(ProductID);

        return tResult.ToString();
    }

    public OrderInfo(string pSerializedString)
    {
        if (string.IsNullOrEmpty(pSerializedString))
        {
            LogManager.LogError("OrderInfo: pSerializedString is null!");
            return;
        }

        var tSpStr = pSerializedString.Split(',');

        if (tSpStr.Length > 0) OrderID = tSpStr[0];
        if (tSpStr.Length > 1) OrderCreateTime = new DateTime(tSpStr[1].ToLong());
        if (tSpStr.Length > 2) OrderType = (OrderType)tSpStr[2].ToInt();
        if (tSpStr.Length > 3) ProductConfig = ConfigData.iAPProductConfig.GetByPrimary(tSpStr[3], false);

        if (tSpStr.Length != 4)
        {
            LogManager.LogError($"OrderInfo: invalid pSerializedString - {pSerializedString}");
            return;
        }
    }
    #endregion
}

public enum OrderType
{
    Invalid,
    Valid,
    PendingValidate,
}