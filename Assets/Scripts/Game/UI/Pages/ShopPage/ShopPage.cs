using Config;
using DG.Tweening;
using Game.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ShopPage : PageBase
    {
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnHome;
        [SerializeField] private GameObject _NodeBottomTab;
        [SerializeField] private UIShopFree _freeItem;
        [SerializeField] private UIShopSingle _singleItem;
        [SerializeField] private UIShopBundle _bundleItem;
        [SerializeField] private ScrollRect _productRoot;
        [SerializeField] private RectTransform _productParent;
        [SerializeField] private GameObject _loadingMask;

        public float shopItemAnimInterval = 0.15f;
        public float moveDuration = 0.2f;
        public AnimationCurve moveEase;

        RectTransform mRectScroll;

        //Dictionary<string, ShopBaseItem> mShopItems;
        List<ShopBaseItem> mCacheItems;
        List<ShopBaseItem> mCurItems;
        ShopPageParam mParam;
        protected override void OnInit()
        {
            mRectScroll = _productRoot.GetComponent<RectTransform>();
            _btnClose.onClick.AddListener(Close);
            _btnHome.onClick.AddListener(Close);
            mCacheItems = new List<ShopBaseItem>();
            mCurItems = new List<ShopBaseItem>();
            //mShopItems = new Dictionary<string, ShopBaseItem>();

            var tFreeItem = InstantiateItem(_freeItem);
            tFreeItem.Init(null, null);
            mCacheItems.Add(tFreeItem);
            //mShopItems.Add("freeItem", tFreeItem);

            foreach (var tConfig in IAPManager.Instance.ProductsConfig)
            {
                if (tConfig.shopId != 1) continue;

                var tItem = InstantiateItem(tConfig.category == (int)ProductPack.Bundle ? _bundleItem : _singleItem);
                tItem.Init(tConfig, OnClickBuy);
                mCacheItems.Add(tItem);
                //mShopItems.Add(tConfig.productID, tItem);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_productParent);
            _freeItem.gameObject.SetActive(false);
            _singleItem.gameObject.SetActive(false);
            _bundleItem.gameObject.SetActive(false);
        }

        ShopBaseItem InstantiateItem(ShopBaseItem pShopItem)
        {
            return Instantiate(pShopItem, _productParent);
        }

        protected override void OnBeginOpen()
        {
            mParam = PageParam as ShopPageParam;
            bool tIsCoinFirst = mParam?.shopGroup == ShopPageParam.ShopGroup.CoinFirst;
            _btnClose.gameObject.SetActive(tIsCoinFirst);
            _NodeBottomTab.gameObject.SetActive(!tIsCoinFirst);
            mRectScroll.offsetMin = new Vector2(mRectScroll.offsetMin.x, tIsCoinFirst ? 0 : 360f);

            if (tIsCoinFirst)
            {
                mCurItems.Clear();
                var tOtherItems = new List<ShopBaseItem>();
                foreach (var tItem in mCacheItems)
                {
                    if (tItem.mConfig == null ||
                       (tItem.mConfig.propsID.Length == 1 && tItem.mConfig.propsID[0] == (int)PropID.Coin))
                    {
                        mCurItems.Add(tItem);//免费和单个金币靠前
                    }
                    else
                    {
                        tOtherItems.Add(tItem);
                    }
                }
                mCurItems.AddRange(tOtherItems);
            }
            else
            {
                mCurItems.Clear();
                mCurItems.AddRange(mCacheItems);
            }

            for (int i = 0; i < mCurItems.Count; i++)
            {
                mCurItems[i].transform.SetSiblingIndex(i);
                mCurItems[i].OnShow();
            }

            _loadingMask.SetActive(false);
            _productRoot.verticalNormalizedPosition = 1f;
            StartCoroutine(PlayAnim());
        }

        IEnumerator PlayAnim()
        {
            InputLockManager.Instance.Lock("ShopAnim");
            yield return null;

            _productRoot.content.GetComponent<VerticalLayoutGroup>().enabled = false;
            foreach (var item in mCurItems)
            {
                item.GetComponent<Animation>().enabled = false;
                //item.transform.DOLocalMoveX(-1500, 0);
                var tTran = item.GetComponent<RectTransform>();
                // 设置锚点到中心
                tTran.anchorMin = new Vector2(0.5f, 0.5f);
                tTran.anchorMax = new Vector2(0.5f, 0.5f);
                // 设置 pivot 到中心
                tTran.pivot = new Vector2(0.5f, 0.5f);
                // 设置位置为中心
                tTran.anchoredPosition = new Vector2(1500, tTran.anchoredPosition.y);
            }
            yield return new WaitForSeconds(GetAnimationTime(true) / 2);
            foreach (var item in mCurItems)
            {
                item.GetComponent<Animation>().enabled = true;
                item.GetComponent<Animation>().Play("Shop_Appear");
                //item.transform.DOLocalMoveX(0, moveDuration).SetEase(moveEase); 
                yield return new WaitForSeconds(shopItemAnimInterval);
            }
            _productRoot.content.GetComponent<VerticalLayoutGroup>().enabled = true;
            InputLockManager.Instance.UnLock("ShopAnim");
        }

        protected override void OnClosed()
        {
            foreach (var tItem in mCurItems)
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

            //刷新购买项
            for (int i = 0; i < mCurItems.Count; i++)
            {
                mCurItems[i].transform.SetSiblingIndex(i);
                mCurItems[i].OnShow();
            }
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

    public class ShopPageParam
    {
        public ShopGroup shopGroup { get; private set; }

        public ShopPageParam(ShopGroup pShopGroup)
        {
            shopGroup = pShopGroup;
        }

        public enum ShopGroup
        {
            None,
            CoinFirst, //金币优先
        }
    }
}
