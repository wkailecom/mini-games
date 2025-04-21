using Config;
using Game.UISystem;
using System;
using System.Collections;
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

        [SerializeField] private RectTransform _tranPassenger;
        [SerializeField] private Text _txtPassenger;

        public Button BtnProp1 => _btnProp1;
        public Button BtnProp2 => _btnProp2;
        public Button BtnProp3 => _btnProp3;

        bool isFreeProp = false;     //金币复活不消耗道具
        bool vipIsEnable = false;    //vip道具选中
        bool isSuccessLock = false;  //完成动画中
        bool isCooling = false;      //冷却中
        MiniGamePageParam mParam;
        MiniGameType mGameType = MiniGameType.Bus;
        protected override void OnInit()
        {
            _btnBack.onClick.AddListener(OnClickBack);
            _btnShop.onClick.AddListener(OnClickShop);
            _btnProp1.onClick.AddListener(OnClickProp1_Refresh);
            _btnProp2.onClick.AddListener(OnClickProp2_VIP);
            _btnProp3.onClick.AddListener(OnClickProp3_Sort);

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

            EventManager.Register(EventKey.BusOut_OnClickUnlockSlot, OnUnlockSlot);
            EventManager.Register(EventKey.BusOut_VIPComplete, OnVIPComplete);
            EventManager.Register(EventKey.BusOut_PassengerNumberChange, OnPassengerChange);
            EventManager.Register(EventKey.BusOut_ReadyToSuccess, OnReadyToSuccess);
            EventManager.Register(EventKey.BusOut_VIPMoveFinish, OnVIPMoveFinish);
            EventManager.Register(EventKey.BusOut_VehicleHit, OnVehicleHit);
            EventManager.Register(EventKey.BusOut_VehicleClick, OnVehicleClick);
            EventManager.Register(EventKey.BusOut_PassengerSeat, OnPassengerSeat);
        }

        protected override void UnregisterEvents()
        {
            EventManager.Unregister(EventKey.MiniGameOver, OnMiniGameOver);

            EventManager.Unregister(EventKey.BusOut_OnClickUnlockSlot, OnUnlockSlot);
            EventManager.Unregister(EventKey.BusOut_VIPComplete, OnVIPComplete);
            EventManager.Unregister(EventKey.BusOut_PassengerNumberChange, OnPassengerChange);
            EventManager.Unregister(EventKey.BusOut_ReadyToSuccess, OnReadyToSuccess);
            EventManager.Unregister(EventKey.BusOut_VIPMoveFinish, OnVIPMoveFinish);
            EventManager.Unregister(EventKey.BusOut_VehicleHit, OnVehicleHit);
            EventManager.Unregister(EventKey.BusOut_VehicleClick, OnVehicleClick);
            EventManager.Unregister(EventKey.BusOut_PassengerSeat, OnPassengerSeat);
        }

        protected override void OnBeginOpen()
        {
            isFreeProp = false;
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

            isSuccessLock = false;
            if (tEventData.isSuccess)
            {
                PageManager.Instance.OpenPage(PageID.MiniSucceedPage, new MiniSucceedPageParam());
            }
            else
            {
                PropID tUseProp = PropID.BusSortDepart;
                var tIsValid = canUseSort;
                PageManager.Instance.OpenPage(PageID.MiniFailedPage, new MiniFailedPageParam(tUseProp, tIsValid, (isProp) =>
                {
                    OnReviveDispose(isProp);
                }));
            }
        }

        void OnReadyToSuccess(EventData pEventData)
        {
            isSuccessLock = true;
            StartCoroutine(ReadyToSuccessTask());
        }

        IEnumerator ReadyToSuccessTask()
        {
            InputLockManager.Instance.Lock("ReadyToSuccessTask");
            while (isSuccessLock) yield return null;
            InputLockManager.Instance.UnLock("ReadyToSuccessTask");
        }

        void SetVIPEnable(bool pIsEnable)
        {
            vipIsEnable = pIsEnable;
            _btnProp2.transform.Find("Selected").gameObject.SetActive(pIsEnable);
            BusOut.EventManager.Instance.OnClickVIP?.Invoke(pIsEnable);
        }

        IEnumerator StartColdDown()
        {
            isCooling = true;
            yield return new WaitForSeconds(3f);
            isCooling = false;
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
        private bool canUseProp => !BusOut.EventManager.Instance.HasMovingVehicle();
        private bool canUseVIPSpot => BusOut.EventManager.Instance.CheckCanUseVIP?.Invoke() ?? false;
        private bool canUseSort => BusOut.EventManager.Instance.CheckCanUseSort?.Invoke() ?? false;

        public bool CanBreak()
        {
            if (!vipMoveFinish || !canUseProp || isCooling || isSuccessLock)
                return true;
            return false;
        }

        void OnReviveDispose(bool pIsProp)
        {
            isFreeProp = !pIsProp;
            if (isFreeProp)
            {
                ModuleManager.Prop.ExpendProp(PropID.BusSortDepart);
            }
            BusOut.EventManager.Instance.OnClickSort?.Invoke();
            BusOut.EventManager.Instance.OnTriggerReplay?.Invoke();
        }

        void OnClickProp1_Refresh()
        {
            AudioManager.Instance.PlaySound(SoundID.BtnClick);
            if (CanBreak()) return;

            var tPropID = PropID.BusRefreshColor;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                ModuleManager.Prop.ExpendProp(tPropID);
                BusOut.EventManager.Instance.OnClickRefresh?.Invoke();
            }
            else
            {
                OpenPropShop(tPropID);
            }
        }

        void OnClickProp2_VIP()
        {
            AudioManager.Instance.PlaySound(SoundID.BtnClick);
            if (CanBreak()) return;

            var tPropID = PropID.BusVIPSpot;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                if (canUseVIPSpot)
                {
                    SetVIPEnable(true);
                }
                else
                {
                    MessageHelp.Instance.ShowMessage("No more vip space.");
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

        void OnClickProp3_Sort()
        {
            AudioManager.Instance.PlaySound(SoundID.BtnClick);
            if (CanBreak()) return;

            var tPropID = PropID.BusSortDepart;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                if (canUseSort)
                {
                    ModuleManager.Prop.ExpendProp(tPropID);
                    BusOut.EventManager.Instance.OnClickSort?.Invoke();
                }
                else
                {
                    MessageHelp.Instance.ShowMessage("No car to sort.");
                }
            }
            else
            {
                OpenPropShop(tPropID);
            }
        }

        void OnUnlockSlot(EventData pEventData)
        {
            var tEventData = pEventData as BusOut_OnClickUnlockSlot;
            var tIndex = tEventData.index;

        }

        void OnPassengerChange(EventData pEventData)
        {
            var tEventData = pEventData as BusOut_PassengerNumberChange;
            _txtPassenger.text = tEventData.count.ToString();
        }

        void OnVIPMoveFinish(EventData pEventData)
        {
            vipMoveFinish = true;
        }

        void OnVehicleHit(EventData pEventData)
        {
            AudioManager.Instance.PlaySound(SoundID.Bus_VehicleHit);
        }

        void OnVehicleClick(EventData pEventData)
        {
            AudioManager.Instance.PlaySound(SoundID.Bus_VehicleClick);
        }

        void OnPassengerSeat(EventData pEventData)
        {
            AudioManager.Instance.PlaySound(SoundID.Bus_PassagerSeat);
        }

        #endregion

        #region 引导
        private void TryShowGuide()
        {

        }

        #endregion
    }
}
