using Config;
using Game.UISystem;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MiniGame
{
    public class TripleGamePage : PageBase
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
        MiniGamePageParam mParam;
        MiniGameType mGameType = MiniGameType.Tile;
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
        }

        protected override void UnregisterEvents()
        {
            EventManager.Unregister(EventKey.MiniGameOver, OnMiniGameOver); 
        }

        protected override void OnBeginOpen()
        {
            isFreeProp = false; 
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
                        ModuleManager.Prop.ExpendProp(PropID.Energy, 1);
                        PageManager.Instance.OpenPage(PageID.MiniGameOverPage, tOverParam); 
                    }
                };
                PageManager.Instance.OpenPage(PageID.MiniRevivePopup, tReviveParam);
            }
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
       
        void OnFreeExtraSlot()
        {
            
        }

        void OnClickReplace()
        {
          
        }

        void OnClickExtraSlot()
        {
           
        }

        void OnClickHammer()
        {
            
        }

        void OnClickExtraBox()
        {
            
        }


        #endregion

        #region 引导
        private void TryShowGuide()
        {
            var tLevel = ModuleManager.MiniGame.CurLevel;
          
        }
         
        #endregion
    }
}
