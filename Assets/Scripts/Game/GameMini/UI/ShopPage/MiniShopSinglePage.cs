using Config;
using Game.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.MiniGame
{
    public class MiniShopSinglePage : PageBase
    {
        public Button btnClose;
        public UICountDown timeCountDown;
        public UIMiniShopItem shopItem;
        public GameObject loadingMask;

        int mProductCount = 0;
        MiniShopSinglePageParam mParam;
        protected override void OnInit()
        {
            shopItem.gameObject.SetActive(true);
            btnClose.onClick.AddListener(OnClickCloseBtn);
        }

        protected override void OnBeginOpen()
        {
            base.OnBeginOpen();

            mProductCount = 1;
            mParam = PageParam as MiniShopSinglePageParam;
            if (mParam == null)
            {
                LogManager.LogError("MiniShopSinglePage.OnBeginOpen: PageParam is Invalid");
                return;
            }

            timeCountDown.StartCountDown(ModuleManager.MiniGame.EndTime, "Finished");
            foreach (var tConfig in IAPManager.Instance.ProductsConfig)
            {
                if (tConfig.shopId != 2) continue;

                if (tConfig.propsID[0] == (int)mParam.propID)
                {
                    mProductCount = tConfig.propsCount[0];
                    shopItem.Init(tConfig, OnClickBuy);
                    shopItem.OnShow();
                    break;
                }
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
                GameVariable.IngamePurchase = true;
                GameVariable.UserBuyFrom = UserBuyFrom.MiniSingle;
            }
            Close();
        }

        void OnClickCloseBtn()
        {
            Close();
            if (ADManager.Instance.IsRewardVideoReady)
            {
                PageManager.Instance.OpenPage(PageID.AdsPropPopup, new AdsPropPageParam(mParam.propID, null));
            }
        }
    }

    public class MiniShopSinglePageParam
    {
        public PropID propID { get; private set; }

        public UnityAction ConfirmAction;

        public MiniShopSinglePageParam(PropID pPropID)
        {
            propID = pPropID;
        }
    }

}