using Config;
using Game;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;


public class UIShopSingle : ShopBaseItem
{
    public Image imgIcon;
    public TextMeshProUGUI txtName;
    public Button btnBuy;
    public TextMeshProUGUI txtPrice;

    //IAPProductConfig mConfig;
    Product mProduct;
    Action<string> mBuyClickEvent;

    public override void Init(IAPProductConfig tProductConfig, Action<string> pOnClickBuy)
    {
        mConfig = tProductConfig;
        mBuyClickEvent = pOnClickBuy;

        btnBuy.gameObject.SetActive(true);
        btnBuy.onClick.AddListener(OnClickBtnBuy);

        if (string.IsNullOrEmpty(mConfig.icon))
        {
            imgIcon.SetPropIcon(mConfig.propsID[0]);
        }
        else
        {
            imgIcon.SetPropIcon(mConfig.icon);
        }

        if (mConfig.category == (int)ProductPack.RemoveAD)
        {
            txtName.text = mConfig.textID; //TextTool.GetText(mConfig.textID);
        }
        else
        {
            txtName.text = "x " + mConfig.propsCount[0];
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

        if (mConfig.category == (int)ProductPack.RemoveAD)
        {
            gameObject.SetActive(!GameMethod.HasRemoveAD());
        }
    }

    public override void OnHide()
    {

    }


    #region UI事件
    void OnClickBtnBuy()
    {
        mBuyClickEvent?.Invoke(mConfig.productID);
    }

    #endregion
}