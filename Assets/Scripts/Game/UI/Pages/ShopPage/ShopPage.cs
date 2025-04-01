using Config;
using Game.UISystem;
using GameLogic;
using System.Collections;
using System.Collections.Generic;
using ThinkingData.Analytics;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ShopPage : PageBase
    {
        [SerializeField] private Button _btnClose;
        [SerializeField] private UIShopFree _freeItem;
        [SerializeField] private UIShopSingle _singleItem;
        [SerializeField] private UIShopBundle _bundleItem;
        [SerializeField] private ScrollRect _productRoot;
        [SerializeField] private RectTransform _productParent;
        [SerializeField] private GameObject _loadingMask;

        Dictionary<string, IShopItem> mShopItems;
        List<IShopItem> mCurShopItems;
        protected override void OnInit()
        {
            _btnClose.onClick.AddListener(Close);
            mCurShopItems = new List<IShopItem>();
            mShopItems = new Dictionary<string, IShopItem>();

            var tFreeItem = InstantiateItem(_freeItem);
            tFreeItem.Init(null, null);
            mShopItems.Add("freeItem", tFreeItem);

            foreach (var tConfig in IAPManager.Instance.ProductsConfig)
            {
                if (tConfig.shopId != 1) continue;

                var tItem = InstantiateItem(tConfig.category == (int)ShopItemCategory.Bundle ? _bundleItem : _singleItem);
                tItem.Init(tConfig, OnClickBuy);
                mShopItems.Add(tConfig.productID, tItem);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_productParent);
            _freeItem.gameObject.SetActive(false);
            _singleItem.gameObject.SetActive(false);
            _bundleItem.gameObject.SetActive(false);
        }

        IShopItem InstantiateItem(MonoBehaviour pShopItem)
        {
            return Instantiate(pShopItem, _productParent) as IShopItem;
        }

        protected override void OnBeginOpen()
        { 

            foreach (var tItem in mCurShopItems)
            {
                tItem.OnShow();
            }

            _loadingMask.SetActive(false);
            _productRoot.verticalNormalizedPosition = 1f;
        }

        protected override void OnClosed()
        {
            foreach (var tItem in mCurShopItems)
            {
                tItem.OnHide();
            }
        }

        protected override void RegisterEvents()
        {
            EventManager.Register(EventKey.PurchaseSuccess, OnPurchaseSuccess);

        }

        protected override void UnregisterEvents()
        {
            EventManager.Unregister(EventKey.PurchaseSuccess, OnPurchaseSuccess);

        }

        void OnPurchaseSuccess(EventData pEventData)
        {
            var tEventData = pEventData as PurchaseSuccess;
            var tConfig = tEventData.productConfig;


        }

        void OnClickBuy(string pProductID)
        {
            _loadingMask.SetActive(true);
            IAPManager.Instance.BuyProduct(pProductID, OnBuyCallback);
        }

        void OnBuyCallback(bool pIsComplete)
        {
            _loadingMask.SetActive(false);
            if (pIsComplete)
            {
                GameVariable.UserBuyFrom = UserBuyFrom.ShopPage;
            }
        }

        #region UI事件




        #endregion
    }
}
