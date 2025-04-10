using Config;
using Game.MiniGame;
using Game.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class HomePage : PageBase
    {
        [SerializeField] private UIBtnHeart _btnHeart;
        [SerializeField] private UIBtnProp _btnCoin;
        [SerializeField] private UIBtnRemoveads _btnAD;
        [SerializeField] private Button _btnSet;
        [SerializeField] private Button _btnShop;

        [SerializeField] private ScrollRect _enterRoot;

        [SerializeField] private Button _btnEvent;
        [SerializeField] private UIMiniEnter _miniEnter;

        [SerializeField] private float enterItemAnimInterval = 0.15f;

        List<UIMiniEnter> mMiniEnter;
        List<UIMiniEnter> mCurMiniEnter;
        bool mIsGuideLevel = false;
        protected override void OnInit()
        {
            mMiniEnter = new List<UIMiniEnter>();
            mCurMiniEnter = new List<UIMiniEnter>();

            _btnSet.onClick.AddListener(OnOpenSettingsPage);
            _btnHeart.buttonRoot.onClick.AddListener(OnClickUIHeart);
            _btnCoin.buttonRoot.onClick.AddListener(OnClickUICoin);
            _btnAD.btnBuy.onClick.AddListener(OnClickUIRemoveads);

            _btnShop.onClick.AddListener(OnOpenShop);

        }

        protected override void OnBeginOpen()
        {
            AudioManager.Instance.PlayMusic(MusicID.bgm_main);

            EnterControl();

            GuideShow(mIsGuideLevel);
        }

        public override void OnCoverPageRemove()
        {
            base.OnCoverPageRemove();
        }

        protected override void RegisterEvents()
        {

        }

        protected override void UnregisterEvents()
        {

        }

        void OnPageBeginOpen(EventData pEventData)
        {
            var tEventData = pEventData as PageOperation;

        }

        void OnPageClosed(EventData pEventData)
        {
            var tEventData = pEventData as PageOperation;
        }

        void EnterControl()
        {
            var tDataList = ModuleManager.MiniFavor.GetShowSort();
            mMiniEnter.SetItemsActive(tDataList.Count, _miniEnter, _enterRoot.content);
            mCurMiniEnter.Clear();
            for (int i = 0; i < tDataList.Count; i++)
            {
                var tConfig = ModuleManager.MiniGame.GetTypeConfig(tDataList[i]);
                mMiniEnter[i].Init(tConfig);
                mCurMiniEnter.Add(mMiniEnter[i]);
            }
            _enterRoot.verticalNormalizedPosition = 1f;
            _miniEnter.gameObject.SetActive(false);
            StartCoroutine(PlayAnim());
        }

        IEnumerator PlayAnim()
        {
            yield return null;
            foreach (var item in mCurMiniEnter)
            {
                item.GetComponent<Animator>().enabled = false;
                item.GetComponent<CanvasGroup>().alpha = 0;
            }
            yield return new WaitForSeconds(GetAnimationTime(true) / 2);
            for (int i = 0; i < mCurMiniEnter.Count; i++)
            {
                mCurMiniEnter[i].GetComponent<Animator>().enabled = true;
                if (i % 2 == 1) yield return new WaitForSeconds(enterItemAnimInterval);
            }
        }

        public void GuideShow(bool pIsGuide)
        {

        }

        #region UI事件

        void OnOpenSettingsPage()
        {
            GameMethod.TriggerUIAction(UIActionName.Settings, UIPageName.PageHome, UIActionType.Click);
            PageManager.Instance.OpenPage(PageID.SettingsPage);
        }

        void OnOpenShop()
        {
            GameMethod.TriggerUIAction(UIActionName.EnterShop, UIPageName.PageHome, UIActionType.Click);
            PageManager.Instance.OpenPage(PageID.ShopPage, new ShopPageParam(ShopPageParam.ShopGroup.None));
        }

        void OnClickUIHeart()
        {
            GameMethod.TriggerUIAction(UIActionName.AddHeart, UIPageName.PageHome, UIActionType.Click);
        }

        void OnClickUICoin()
        {
            GameMethod.TriggerUIAction(UIActionName.AddCoin, UIPageName.PageHome, UIActionType.Click);
        }

        void OnClickUIRemoveads()
        {
            GameMethod.TriggerUIAction(UIActionName.RemoveAds, UIPageName.PageHome, UIActionType.Click);
        }


        #endregion
    }
}