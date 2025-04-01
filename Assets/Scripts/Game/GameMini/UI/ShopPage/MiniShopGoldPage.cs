using Config;
using Game.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MiniGame
{
    public class MiniShopGoldPage : PageBase
    {
        public Button btnClose;
        public UIMiniShopItem shopItem;
        public UICountDown timeCountDown;
        public RectTransform productRarentRoot;
        public GameObject loadingMask;

        List<UIMiniShopItem> mShopItems;

        protected override void OnInit()
        {
            mShopItems = new List<UIMiniShopItem>();

            shopItem.gameObject.SetActive(false);
            foreach (var tConfig in IAPManager.Instance.ProductsConfig)
            {
                if (tConfig.shopId != 4) continue;

                var tItem = Instantiate(shopItem, productRarentRoot);
                tItem.Init(tConfig, OnClickBuy);
                tItem.gameObject.SetActive(true);
                mShopItems.Add(tItem);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(productRarentRoot);
        }

        protected override void OnBeginOpen()
        {
            base.OnBeginOpen();

            timeCountDown.StartCountDown(ModuleManager.MiniGame.EndTime, "Finished");
            foreach (var tItem in mShopItems)
            {
                tItem.OnShow();
            }

            loadingMask.SetActive(false);
        }

        void OnClickBuy(string pProductID)
        {
            loadingMask.SetActive(true);
            IAPManager.Instance.BuyProduct(pProductID, OnBuyCallback);
        }

        void OnBuyCallback(bool pIsComplete)
        {
            loadingMask.SetActive(false);
            if (pIsComplete)
            {
                Close();
            }
        }
    }

}