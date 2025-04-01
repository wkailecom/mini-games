using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class UIMiniShopItem : MonoBehaviour
{
    public UIPropItem propItem;
    public RectTransform propRoot;

    public Button buyButton;
    public Text priceText;
    public GameObject discountRoot;
    public Text discountText;

    IAPProductConfig mConfig;
    Product mProduct;
    Action<string> mBuyClickEvent;

    List<UIPropItem> mUIPropItems;
    public void Init(IAPProductConfig tProductConfig, Action<string> pOnClickBuy)
    {
        mConfig = tProductConfig;
        mBuyClickEvent = pOnClickBuy;

        propItem.SetData(new PropData((PropID)tProductConfig.propsID[0], tProductConfig.propsCount[0]));
        mUIPropItems = new List<UIPropItem> { propItem };
        for (int i = 1; i < tProductConfig.propsID.Length; i++)
        {
            var tItem = Instantiate(propItem, propRoot);
            tItem.SetData(new PropData((PropID)tProductConfig.propsID[i], tProductConfig.propsCount[i]));
            mUIPropItems.Add(tItem);
        }

        if (mUIPropItems.Count == 1)
        {
            mUIPropItems[0].transform.localPosition = Vector3.zero;
        }
        else if (mUIPropItems.Count == 2)
        {
            mUIPropItems[0].transform.localPosition = new Vector3(-propRoot.rect.width / 4, 0);
            mUIPropItems[1].transform.localPosition = new Vector3(propRoot.rect.width / 4, 0);
        }
        else if (mUIPropItems.Count == 3)
        {
            mUIPropItems[0].transform.localPosition = new Vector3(0, propRoot.rect.height / 4);
            mUIPropItems[1].transform.localPosition = new Vector3(-propRoot.rect.width / 4, -propRoot.rect.height / 4);
            mUIPropItems[2].transform.localPosition = new Vector3(propRoot.rect.width / 4, -propRoot.rect.height / 4);
        }
        else if (mUIPropItems.Count == 4)
        {
            mUIPropItems[0].transform.localPosition = new Vector3(-propRoot.rect.width / 4, propRoot.rect.height / 4);
            mUIPropItems[1].transform.localPosition = new Vector3(propRoot.rect.width / 4, propRoot.rect.height / 4);
            mUIPropItems[2].transform.localPosition = new Vector3(-propRoot.rect.width / 4, -propRoot.rect.height / 4);
            mUIPropItems[3].transform.localPosition = new Vector3(propRoot.rect.width / 4, -propRoot.rect.height / 4);
        }

        discountRoot.SetActive(mConfig.discount > 0);
        discountText.text = TextTool.Percent(mConfig.discount / 100f);
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnclickBuyButton);
    }

    public void OnShow()
    {
        if (mProduct == null)
        {
            mProduct = IAPManager.Instance.GetProduct(mConfig.productID);
            priceText.text = IAPManager.Instance.GetPriceText(mProduct, mConfig.price);
        }
    }

    public void OnHide()
    {

    }

    #region UI事件
    public void OnclickBuyButton()
    {
        mBuyClickEvent?.Invoke(mConfig.productID);
    }

    #endregion
}