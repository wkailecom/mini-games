using Config;
using Game.UISystem;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MiniGame
{
    public class ScrewGamePage : PageBase
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

        bool isFreeProp = false;
        bool isEnableHammer = false;
        MiniGamePageParam mParam;
        MiniGameType mGameType = MiniGameType.Screw;
        protected override void OnInit()
        {
            _btnBack.onClick.AddListener(OnClickBack);
            _btnShop.onClick.AddListener(OnClickShop);
            _btnProp1.onClick.AddListener(OnClickProp1_ExtraSlot);
            _btnProp2.onClick.AddListener(OnClickProp2_Hammer);
            _btnProp3.onClick.AddListener(OnClickProp3_ExtraBox);

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
            EventManager.Register(EventKey.MiniGameUsePropComplete, OnMiniGameUsePropComplete);
        }

        protected override void UnregisterEvents()
        {
            EventManager.Unregister(EventKey.MiniGameOver, OnMiniGameOver);
            EventManager.Unregister(EventKey.MiniGameUsePropComplete, OnMiniGameUsePropComplete);
        }

        protected override void OnBeginOpen()
        {
            isFreeProp = false;
            SetHammerEnable(false);

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
            SetPauseTime(false);
            TryShowGuide();
        }
        protected override void OnBeginClose()
        {
            SetPauseTime(true);
        }
        public override void OnCoveredByOtherPage()
        {
            SetPauseTime(true);
        }
        public override void OnCoverPageRemove()
        {
            SetPauseTime(false);
        }

        void OnMiniGameOver(EventData pEventData)
        {
            var tEventData = pEventData as MiniGameOver;
            if (tEventData.modeType != mGameType) return;

            if (tEventData.isSuccess)
            {
                // PageManager.Instance.OpenPage(PageID.MiniSucceedPage, new MiniSucceedPageParam());
            }
            else
            {
                PropID tUseProp = PropID.ScrewExtraSlot;
                var tIsValid = ScrewJam.EventManager.Instance.CheckCanExtraSlotUse.Invoke();
                PageManager.Instance.OpenPage(PageID.MiniFailedPage, new MiniFailedPageParam(tUseProp, tIsValid, (isProp) =>
                {
                    OnReviveDispose(isProp);
                }));
            }
        }

        void SetHammerEnable(bool pIsEnable)
        {
            isEnableHammer = pIsEnable;
            _btnProp2.transform.Find("Selected").gameObject.SetActive(pIsEnable);
        }

        void SetPauseTime(bool pIsPause)
        {
            ScrewJam.EventManager.Instance.OnChangeClickState?.Invoke(!pIsPause);
            MiniGameManager.Instance.SetPlayTime(pIsPause);
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

        void OpenPropShop(PropID pPropID)
        {
            PageManager.Instance.OpenPage(PageID.MiniShopSinglePage, new MiniShopSinglePageParam(pPropID));
        }

        void OpenAdsPropPopup(PropID pPropID, Action<bool> pCallBack = null)
        {
            PageManager.Instance.OpenPage(PageID.AdsPropPopup, new AdsPropPageParam(pPropID, pCallBack));
        }

        #endregion

        #region 道具事件

        void OnMiniGameUsePropComplete(EventData pEventData)
        {
            var tEventData = pEventData as MiniGameUsePropComplete;
            if (tEventData.modeType != mGameType) return;

            if (tEventData.propID == PropID.ScrewHammer)
            {
                SetHammerEnable(false);
            }

            if (isFreeProp)
            {
                isFreeProp = false;
            }
            else
            {
                ModuleManager.Prop.ExpendProp(tEventData.propID);
            }

            if (tEventData.propID == PropID.ScrewExtraBox)
            {
                AudioManager.Instance.PlaySound(SoundID.Mini_Prop_Magnet);
            }
            else if (tEventData.propID == PropID.ScrewExtraSlot)
            {
                AudioManager.Instance.PlaySound(SoundID.Mini_Prop_ExtraSlot);
            }
        }

        void OnReviveDispose(bool pIsProp)
        {
            isFreeProp = !pIsProp;
            ScrewJam.EventManager.Instance.OnClickAddHoleSlot?.Invoke();
            ScrewJam.EventManager.Instance.OnTriggerReplay?.Invoke();
        }

        void OnClickProp1_ExtraSlot()
        {
            var tPropID = PropID.ScrewExtraSlot;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                ScrewJam.EventManager.Instance.OnClickAddHoleSlot?.Invoke();
            }
            else
            {
                OpenPropShop(tPropID);
            }
        }

        void OnClickProp2_Hammer()
        {
            var tPropID = PropID.ScrewHammer;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                SetHammerEnable(!isEnableHammer);
                ScrewJam.EventManager.Instance.OnClickHammer?.Invoke();
            }
            else
            {
                OpenPropShop(tPropID);
            }
        }

        void OnClickProp3_ExtraBox()
        {
            var tPropID = PropID.ScrewExtraBox;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                ScrewJam.EventManager.Instance.OnClickToolbox?.Invoke();
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
            var tLevel = ModuleManager.MiniGame.GetCurLevel((int)mGameType);
            if (!DataTool.GetBool(MiniGameConst.Guide_ScreRules) && tLevel == 1)
            {
                DataTool.SetBool(MiniGameConst.Guide_ScreRules, true);
                PageManager.Instance.OpenPage(PageID.MiniGuidePage, MiniGameConst.Guide_ScreRules);
            }
            else if (!DataTool.GetBool(MiniGameConst.Guide_ScreProps) && tLevel == 2)
            {
                DataTool.SetBool(MiniGameConst.Guide_ScreProps, true);
                PageManager.Instance.OpenPage(PageID.MiniGuidePage, MiniGameConst.Guide_ScreProps);
            }
        }

        #endregion
    }
}
