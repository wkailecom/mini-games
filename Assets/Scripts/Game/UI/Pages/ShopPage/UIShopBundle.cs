using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class UIShopBundle : ShopBaseItem
{
    public RectTransform itemsRoot;
    public UIItem item;
    public Button btnBuy;
    public TextMeshProUGUI txtPrice;

    [HideInInspector] public List<UIItem> propItems;

    //IAPProductConfig mConfig;
    Product mProduct;
    Action<string> mBuyClickEvent;

    public override void Init(IAPProductConfig tProductConfig, Action<string> pOnClickBuy)
    {
        mConfig = tProductConfig;
        mBuyClickEvent = pOnClickBuy;
        btnBuy.gameObject.SetActive(true);
        btnBuy.onClick.AddListener(OnClickBtnBuy);

        ////txtName.text = "x" + TextTool.GetText(mConfig.textID);
        //imgIcon.SetIcon(mConfig.icon);

        propItems = new List<UIItem>() { item };
        GameMethod.SetItemsActive(propItems, mConfig.propsID.Length, item, itemsRoot);
        for (int i = 0; i < propItems.Count; i++)
        {
            propItems[i].SetUIItem(SetPropIcon(mConfig.propsID[i]), mConfig.propsCount[i].ToString(), string.Empty);
        }
    }

    public override void OnShow()
    {
        if (mProduct == null)
        {
            mProduct = IAPManager.Instance.GetProduct(mConfig.productID);
            txtPrice.text = IAPManager.Instance.GetPriceText(mProduct, mConfig.price);
        }
#if UNITY_EDITOR
        txtPrice.text = "$" + mConfig.price;
#endif
    }

    public override void OnHide()
    {

    }

    Sprite SetPropIcon(int pPropId)
    {
        var tConfig = ConfigData.propConfig.GetByPrimary(pPropId);
        if (tConfig == null)
        {
            LogManager.LogError("UIShopBundle.SetPropIcon: param is null!");
            return null;
        }

        return ResTool.LoadPropIcon(tConfig.icon);
    }

    #region UI事件
    void OnClickBtnBuy()
    {
        mBuyClickEvent?.Invoke(mConfig.productID);
    }

    #endregion
}