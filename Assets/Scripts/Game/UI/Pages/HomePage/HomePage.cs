using Config;
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
        [SerializeField] private UIBtnRemoveads _btnAD;
        [SerializeField] private Button _btnSet;
        [SerializeField] private Button _btnShop; 
        [SerializeField] private Button _btnLevel;

        [SerializeField] private UIMiniGameEnter _miniGameEnter; 
        [SerializeField] private Button _btnScrew; 
        [SerializeField] private ScrollRect _enterRoot;
        [SerializeField] private UIDailyEnter _dailyEnter;
         

        bool mIsGuideLevel = false;
        protected override void OnInit()
        {
            _btnSet.onClick.AddListener(OnOpenSettingsPage);
            _btnHeart.buttonRoot.onClick.AddListener(OnClickUIHeart);
            _btnAD.btnBuy.onClick.AddListener(OnClickUIRemoveads);

            _btnShop.onClick.AddListener(OnOpenShop);
            _btnScrew.onClick.AddListener(OnClickBtnScrew);

            _btnLevel.onClick.AddListener(OnPlayGame);

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
            PageManager.Instance.OpenPage(PageID.ShopPage);
        }

        void OnClickBtnScrew()
        {
            PageManager.Instance.OpenPage(PageID.MiniMapPage);
        }

        void OnClickUIHeart()
        {
            GameMethod.TriggerUIAction(UIActionName.AddHearts, UIPageName.PageHome, UIActionType.Click);
        }

        void OnClickUIRemoveads()
        {
            GameMethod.TriggerUIAction(UIActionName.RemoveAds, UIPageName.PageHome, UIActionType.Click);
        }

        void OnPlayGame()
        {
            GameMethod.TriggerUIAction(UIActionName.Play, UIPageName.PageHome, UIActionType.Click, ADType.Interstitial);
            if (mIsGuideLevel)
            {
                PlayEndlessGame(true);
            }
            else
            {
                PlayEndlessGame(false);
            }
        }

        public void PlayEndlessGame(bool pIsNewGame = false)
        {
            if (ModuleManager.Prop.HasProp(PropID.Health))
            {
                ADManager.Instance.PlayInterstitial(ADShowReason.Interstitial_GameStart);
                //GameManager.Instance.StartGame(GameModeType.Endless, ModuleManager.LevelInfo.EndlessLevelID);
            }
            else
            {
                _btnHeart.OpenGetADHealth();
            }
        }

        #endregion
    }
}