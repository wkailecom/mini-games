using Config;
using Game;
using Game.UISystem;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;


public class UIShopSingle : MonoBehaviour, IShopItem
{
    public Image imgIcon;
    public TextMeshProUGUI txtName;
    public Button btnBuy;
    public TextMeshProUGUI txtPrice;

    IAPProductConfig mConfig;
    Product mProduct;
    Action<string> mBuyClickEvent;

    public void Init(IAPProductConfig tProductConfig, Action<string> pOnClickBuy)
    {
        mConfig = tProductConfig;
        mBuyClickEvent = pOnClickBuy;
        btnBuy.gameObject.SetActive(true);
        btnBuy.onClick.AddListener(OnClickBtnBuy);

        //if (mConfig.category == (int)ShopItemCategory.RemoveAD)
        //{
        //    imgIcon.SetIcon(mConfig.icon);
        //    txtName.text = TextTool.GetText(mConfig.textID);
        //}
        //else
        //{
        //    imgIcon.SetIcon(mConfig.icon);
        //    txtName.text = "x " + mConfig.propsCount[0];
        //}
    }

    public void OnShow()
    {
        if (mProduct == null)
        {
            mProduct = IAPManager.Instance.GetProduct(mConfig.productID);
            txtPrice.text = IAPManager.Instance.GetPriceText(mProduct, mConfig.price);
        }

        if (mConfig.category == (int)ShopItemCategory.RemoveAD)
        {
            //imgIcon.SetIcon(mConfig.icon);
            //txtName.text = TextTool.GetText(mConfig.textID);
            gameObject.SetActive(!GameMethod.HasRemoveAD());
        }
    }

    public void OnHide()
    {

    } 
 

    #region UI事件
    void OnClickBtnBuy()
    {
        mBuyClickEvent?.Invoke(mConfig.productID);
    }

    #endregion
}