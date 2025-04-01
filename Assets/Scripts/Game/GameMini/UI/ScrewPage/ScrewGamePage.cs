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
            _btnProp1.onClick.AddListener(OnClickExtraSlot);
            _btnProp2.onClick.AddListener(OnClickHammer);
            _btnProp3.onClick.AddListener(OnClickExtraBox);

#if UNITY_EDITOR || GM_MODE
            _gmBtn1.gameObject.SetActive(true);
            _gmBtn2.gameObject.SetActive(true);
            _gmBtn1.onClick.AddListener(() => { TriggerEventGameOver(true); });
            _gmBtn2.onClick.AddListener(() => { TriggerEventGameOver(false); });
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

            ScrewJam.EventManager.Instance.OnChangeClickState?.Invoke(true);
        }

        protected override void OnOpened()
        {
            TryShowGuide();
        }

        protected override void OnBeginClose()
        {
            ScrewJam.EventManager.Instance.OnChangeClickState?.Invoke(false);
        }

        void OnMiniGameOver(EventData pEventData)
        {
            var tEventData = pEventData as MiniGameOver;
            if (tEventData.modeType != mGameType) return;

            var tOverParam = new MiniGameOverPageParam(mParam.level, tEventData.isSuccess);
            if (tEventData.isSuccess)
            {
                PageManager.Instance.OpenPage(PageID.MiniGameOverPage, tOverParam);
            }
            else
            {
                var tReviveParam = new MiniRevivePopupParam();
                tReviveParam.isReturn = false;
                tReviveParam.level = mParam.level;
                tReviveParam.clickAction = (isRight) =>
                {
                    if (isRight)
                    {
                        OnClickReplace();
                    }
                    else
                    {
                        ModuleManager.Prop.ExpendProp(PropID.Health, 1);
                        PageManager.Instance.OpenPage(PageID.MiniGameOverPage, tOverParam); 
                    }
                };
                PageManager.Instance.OpenPage(PageID.MiniRevivePopup, tReviveParam);
            }
        }

        void SetHammerEnable(bool pIsEnable)
        {
            isEnableHammer = pIsEnable;
            _btnProp2.transform.GetChild(0).gameObject.SetActive(pIsEnable);
        }

        void TriggerEventGameOver(bool pIsSuccess)
        {
            var tEventData = EventManager.GetEventData<MiniGameOver>(EventKey.MiniGameOver);
            tEventData.modeType = mGameType;
            tEventData.levelID = mParam.level;
            tEventData.isSuccess = pIsSuccess;
            EventManager.Trigger(tEventData);
        }

        #region UI事件

        void OnClickBack()
        {
            var tPageParam = new MiniRevivePopupParam();
            tPageParam.isReturn = true;
            tPageParam.level = mParam.level; 
            PageManager.Instance.OpenPage(PageID.MiniRevivePopup, tPageParam);
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
                return;
            }

            ModuleManager.Prop.ExpendProp(tEventData.propID);

            if (tEventData.propID== PropID.ScrewExtraBox)
            {
                AudioManager.Instance.PlaySound(SoundID.Mini_Prop_Magnet);
            }
            else if (tEventData.propID == PropID.ScrewExtraSlot)
            {
                AudioManager.Instance.PlaySound(SoundID.Mini_Prop_ExtraSlot);
            }
        }

        void OnFreeExtraSlot()
        {
            isFreeProp = true;
            ScrewJam.EventManager.Instance.OnClickAddHoleSlot?.Invoke();
            ScrewJam.EventManager.Instance.OnTriggerReplay?.Invoke();
        }

        void OnClickReplace()
        {
            var tPropID = PropID.ScrewExtraSlot;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                ScrewJam.EventManager.Instance.OnClickAddHoleSlot?.Invoke();
                ScrewJam.EventManager.Instance.OnTriggerReplay?.Invoke();
            }
            else
            {
                OpenPropShop(tPropID);
            }
        }

        void OnClickExtraSlot()
        {
            var tPropID = PropID.ScrewExtraSlot;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                ScrewJam.EventManager.Instance.OnClickAddHoleSlot?.Invoke();
            }
            else
            {
                //GameMethod.TriggerUIAction(UIActionName.AdHints, UIPageName.PageGame, UIActionType.Click, ADType.Interstitial);
                //OpenAdsPropPopup(tPropID);
                OpenPropShop(tPropID);
            }
        }

        void OnClickHammer()
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

        void OnClickExtraBox()
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
            var tLevel = ModuleManager.MiniGame.CurLevel;
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
