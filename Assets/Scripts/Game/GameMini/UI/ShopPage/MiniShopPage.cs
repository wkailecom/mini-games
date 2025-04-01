using Config;
using Game.UISystem;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MiniGame
{
    public class MiniShopPage : PageBase
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
            btnClose.onClick.AddListener(Close);
        }

        protected override void OnBeginOpen()
        {
            base.OnBeginOpen();

            int shopId = ModuleManager.MiniGame.GameType switch
            {
                MiniGameType.Screw => 101,
                MiniGameType.Jam3d => 102,
                MiniGameType.Tile => 103,
                _ => 101,
            };

            timeCountDown.StartCountDown(ModuleManager.MiniGame.EndTime, "Finished");
            foreach (var tConfig in IAPManager.Instance.ProductsConfig)
            {
                if (tConfig.shopId != shopId) continue;

                var tItem = Instantiate(shopItem, productRarentRoot);
                tItem.Init(tConfig, OnClickBuy);
                tItem.gameObject.SetActive(true);
                mShopItems.Add(tItem);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(productRarentRoot);

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
                GameVariable.UserBuyFrom = UserBuyFrom.MiniBundle;
            }
        }

    }
}