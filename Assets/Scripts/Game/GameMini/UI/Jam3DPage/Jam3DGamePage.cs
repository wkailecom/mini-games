using Config;
using Game.UI;
using Game.UISystem;
using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MiniGame
{
    public class Jam3DGamePage : PageBase
    {
        [SerializeField] private Text _txtLevel;
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnProp1;
        [SerializeField] private Button _btnProp2;
        [SerializeField] private Button _btnProp3;
        [SerializeField] private Button _btnShop;

        [SerializeField] private Transform _nodeRoot;
        [SerializeField] private Transform _tranNode1;
        [SerializeField] private Transform _tranNode2;

        [SerializeField] private Button _gmBtn1;
        [SerializeField] private Button _gmBtn2;

        public Button BtnProp1 => _btnProp1;
        public Button BtnProp2 => _btnProp2;
        public Button BtnProp3 => _btnProp3;


        List<Transform> mNodes = new List<Transform>();
        MiniGamePageParam mParam;
        MiniGameType mGameType = MiniGameType.Jam3d;
        protected override void OnInit()
        {
            _btnBack.onClick.AddListener(OnClickBack);
            _btnShop.onClick.AddListener(OnClickShop);
            _btnProp1.onClick.AddListener(OnClickShuffle);
            _btnProp2.onClick.AddListener(OnClickRevert);
            _btnProp3.onClick.AddListener(OnClickReplace);

            for (int i = 0; i < _nodeRoot.childCount; i++)
            {
                mNodes.Add(_nodeRoot.GetChild(i));
            }

#if UNITY_EDITOR || GM_MODE
            _gmBtn1.gameObject.SetActive(true);
            _gmBtn2.gameObject.SetActive(true);
            _gmBtn1.onClick.AddListener(() => { JamManager.GetSingleton().EndGame(true); });
            _gmBtn2.onClick.AddListener(() => { JamManager.GetSingleton().EndGame(false); });
#else
            _gmBtn1.gameObject.SetActive(false);
            _gmBtn2.gameObject.SetActive(false);
#endif
        }

        protected override void RegisterEvents()
        {
            EventManager.Register(EventKey.MiniGameSubSuccess, OnMiniGameSubSuccess);
            EventManager.Register(EventKey.MiniGameOver, OnMiniGameOver);
        }

        protected override void UnregisterEvents()
        {
            EventManager.Register(EventKey.MiniGameSubSuccess, OnMiniGameSubSuccess);
        }

        protected override void OnBeginOpen()
        {
            if (JamManager.GetSingleton()._mainCamera != null)
                JamManager.GetSingleton()._mainCamera.gameObject.SetActive(true);

            mParam = PageParam as MiniGamePageParam;
            if (mParam == null)
            {
                LogManager.LogError("MiniGamePage.OnBeginOpen PageParam is null!!!");
                return;
            }

            _txtLevel.text = $"LEVEL {mParam.level}";

            mNodes.SetItemsActive(mParam.nodeNum, _tranNode2, _nodeRoot);
            SetNode(JamManager.GetSingleton().LevelIndex);
        }

        protected override void OnOpened()
        {
            TryShowGuide();
        }

        void SetNode(int pIndex)
        {
            for (int i = 0; i < mNodes.Count; i++)
            {
                mNodes[i].GetChild(2).gameObject.SetActive(i <= pIndex);
            }
        }

        void OnMiniGameSubSuccess(EventData pEventData)
        {
            SetNode(JamManager.GetSingleton().LevelIndex);
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
                        OnClickRevive();
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

        void OnClickRevive()
        {
            var tPropID = PropID.Jam3DReplace;
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

        void OnClickReplace()
        {
            var tPropID = PropID.Jam3DReplace;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                if (JamManager.GetSingleton().Board.CanReplace() && JamManager.GetSingleton().Replace())
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

        void OnClickRevert()
        {
            var tPropID = PropID.Jam3DRevert;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                if (JamManager.GetSingleton().Undo())
                {
                    ModuleManager.Prop.ExpendProp(tPropID);
                    AudioManager.Instance.PlaySound(SoundID.Mini_Prop_Magnet);
                }
            }
            else
            {
                OpenPropShop(tPropID);
            }
        }

        void OnClickShuffle()
        {
            var tPropID = PropID.Jam3DShuffle;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                if (JamManager.GetSingleton().Shuffle())
                {
                    ModuleManager.Prop.ExpendProp(tPropID);
                    AudioManager.Instance.PlaySound(SoundID.Mini_Prop_Shuffle);
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
            var tLevel = ModuleManager.MiniGame.CurLevel;
            if (!DataTool.GetBool(MiniGameConst.Guide_JamRules) && tLevel == 1)
            {
                DataTool.SetBool(MiniGameConst.Guide_JamRules, true);
                PageManager.Instance.OpenPage(PageID.MiniGuidePage, MiniGameConst.Guide_JamRules);
            }
            else if (!DataTool.GetBool(MiniGameConst.Guide_JamProps) && tLevel == 2)
            {
                DataTool.SetBool(MiniGameConst.Guide_JamProps, true);
                PageManager.Instance.OpenPage(PageID.MiniGuidePage, MiniGameConst.Guide_JamProps);
            }
        }

        #endregion
    }

    public class MiniGamePageParam
    {
        public int level { get; private set; }
        public int nodeNum { get; private set; }

        public MiniGamePageParam(int pLevel, int pNodeNum = 1)
        {
            level = pLevel;
            nodeNum = pNodeNum;
        }
    }
}
