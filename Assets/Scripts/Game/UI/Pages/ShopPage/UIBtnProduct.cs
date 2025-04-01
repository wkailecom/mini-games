using Config;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIBtnProduct : MonoBehaviour
    {
        public Image icon;
        public Button btnBuy;
        public TextMeshProUGUI txtCount;
        public TextMeshProUGUI txtPrice;

        IAPProductConfig mConfig;
        Product mProduct;
        Action<string> mBuyClickEvent;
        public void Init(string tProductId, Action<string> pOnClickBuy)
        {
            mConfig = ConfigData.iAPProductConfig.GetByPrimary(tProductId);

            mBuyClickEvent = pOnClickBuy;
            btnBuy.onClick.AddListener(OnClickBtnBuy);

            icon.SetPropIcon(mConfig.icon, false);
            txtCount.text = $"+{mConfig.propsCount[0]}";
        }

        public void OnShow()
        {
            if (mProduct == null)
            {
                mProduct = IAPManager.Instance.GetProduct(mConfig.productID);
                txtPrice.text = IAPManager.Instance.GetPriceText(mProduct, mConfig.price);
            }
        }


        void OnClickBtnBuy()
        {
            mBuyClickEvent?.Invoke(mConfig.productID);
        }
    }
}