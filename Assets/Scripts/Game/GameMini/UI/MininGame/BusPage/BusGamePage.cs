using Config;
using Game.UISystem;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MiniGame
{
    public class BusGamePage : PageBase
    {
        [SerializeField] private Text _txtLevel;
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnProp1;
        [SerializeField] private Button _btnProp2;
        [SerializeField] private Button _btnProp3;
        [SerializeField] private Button _btnShop;

        [SerializeField] private Button _gmBtn1;
        [SerializeField] private Button _gmBtn2;

        public Button BtnProp1 => _btnProp1;
        public Button BtnProp2 => _btnProp2;
        public Button BtnProp3 => _btnProp3;

        bool vipIsEnable = false;
        MiniGamePageParam mParam;
        MiniGameType mGameType = MiniGameType.Bus;
        protected override void OnInit()
        {
            _btnBack.onClick.AddListener(OnClickBack);
            _btnShop.onClick.AddListener(OnClickShop);
            _btnProp1.onClick.AddListener(OnClickRefresh);
            _btnProp2.onClick.AddListener(OnClickVIP);
            _btnProp3.onClick.AddListener(OnClickSort);

#if UNITY_EDITOR || GM_MODE
            _gmBtn1.gameObject.SetActive(true);
            _gmBtn2.gameObject.SetActive(true);
            _gmBtn1.onClick.AddListener(() => { MiniGameManager.Instance.TriggerEventGameOver(mGameType, true); });
            _gmBtn2.onClick.AddListener(() => { MiniGameManager.Instance.TriggerEventGameOver(mGameType, false); });
#else
            _gmBtn1.gameObject.SetActive(false);
            _gmBtn2.gameObject.SetActive(false);
#endif
        }

        protected override void RegisterEvents()
        {
            EventManager.Register(EventKey.MiniGameOver, OnMiniGameOver);
            EventManager.Register(EventKey.BusOut_VIPComplete, OnVIPComplete);
        }

        protected override void UnregisterEvents()
        {
            EventManager.Unregister(EventKey.MiniGameOver, OnMiniGameOver);
        }

        protected override void OnBeginOpen()
        {
            SetVIPEnable(false);
            mParam = PageParam as MiniGamePageParam;
            if (mParam == null)
            {
                LogManager.LogError("MiniGamePage.OnBeginOpen PageParam is null!!!");
                return;
            }

            _txtLevel.text = $"LEVEL {mParam.level}";

        }

        protected override void OnOpened()
        {
            TryShowGuide();
        }

        protected override void OnBeginClose()
        {

        }

        void OnMiniGameOver(EventData pEventData)
        {
            var tEventData = pEventData as MiniGameOver;
            if (tEventData.modeType != mGameType) return;

            if (tEventData.isSuccess)
            {
                PageManager.Instance.OpenPage(PageID.MiniSucceedPage, new MiniSucceedPageParam());
            }
            else
            {
                PropID tUseProp = PropID.Invalid;
                var tIsValid = false;
                PageManager.Instance.OpenPage(PageID.MiniFailedPage, new MiniFailedPageParam(tUseProp, tIsValid, () =>
                {
                    //OnClickRevive();
                }));
            }
        }

        void SetVIPEnable(bool pIsEnable)
        {
            vipIsEnable = pIsEnable;
            _btnProp2.transform.Find("Selected").gameObject.SetActive(pIsEnable); 
            BusOut.EventManager.Instance.OnClickVIP?.Invoke(pIsEnable);
        }

        #region UI事件

        void OnClickBack()
        {
            PageManager.Instance.OpenPage(PageID.MiniExitPage);
        }

        void OnClickShop()
        {
            PageManager.Instance.OpenPage(PageID.MiniShopPage);
        }

        void OpenAdsPropPopup(PropID pPropID, Action<bool> pCallBack = null)
        {
            PageManager.Instance.OpenPage(PageID.AdsPropPopup, new AdsPropPageParam(pPropID, pCallBack));
        }

        void OpenPropShop(PropID pPropID)
        {
            PageManager.Instance.OpenPage(PageID.MiniShopSinglePage, new MiniShopSinglePageParam(pPropID));
        }

        #endregion

        #region 道具事件

        private bool guideRule = false;
        private bool vipMoveFinish = true;
        private bool CanUseProp => !BusOut.EventManager.Instance.HasMovingVehicle();

        public bool CanBlock()
        {
            //if (!vipMoveFinish || !CanUseProp || !isColdDown || toOpenFailedPage || !canUseSort || readyToSuccess || isClearing)
            //    return true;
            return false;
        }

        void OnClickRefresh()
        {
            var tPropID = PropID.BusRefreshColor;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                AudioManager.Instance.PlaySound(SoundID.BtnClick);
                ModuleManager.Prop.ExpendProp(tPropID);
                BusOut.EventManager.Instance.OnClickRefresh?.Invoke();
            }
            else
            {
                OpenPropShop(tPropID);
            }
        }

        void OnClickVIP()
        {
            var tPropID = PropID.BusVIPSpot;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                var tCanVIPSpot = BusOut.EventManager.Instance.CheckCanUseVIP?.Invoke() ?? false;
                if (tCanVIPSpot)
                {
                    AudioManager.Instance.PlaySound(SoundID.BtnClick);
                    SetVIPEnable(true);
                }
                else
                {
                    MessageHelp.Instance.ShowMessage("No more vip space");
                }
            }
            else
            {
                OpenPropShop(tPropID);
            }
        }

        void OnVIPComplete(EventData pEventData)
        {
            SetVIPEnable(false); 
            ModuleManager.Prop.ExpendProp(PropID.BusVIPSpot);
        }

        void OnClickSort()
        {
            var tPropID = PropID.BusSortDepart;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                if (JamManager.GetSingleton().ContinueGame())
                {
                    ModuleManager.Prop.ExpendProp(tPropID);
                    AudioManager.Instance.PlaySound(SoundID.Mini_Prop_Recall);
                }
            }
            else
            {
                OpenPropShop(tPropID);
            }
        }


        #endregion

        #region 引导
        private void TryShowGuide()
        {

        }

        #endregion
    }
}
