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
            _btnProp1.onClick.AddListener(OnClickProp1_Shuffle);
            _btnProp2.onClick.AddListener(OnClickProp2_Revert);
            _btnProp3.onClick.AddListener(OnClickProp3_Replace);

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
            EventManager.Register(EventKey.MiniGameOver, OnMiniGameOver);

            EventManager.Register(EventKey.MiniGameSubSuccess, OnMiniGameSubSuccess);
        }

        protected override void UnregisterEvents()
        {
            EventManager.Unregister(EventKey.MiniGameOver, OnMiniGameOver);

            EventManager.Unregister(EventKey.MiniGameSubSuccess, OnMiniGameSubSuccess);
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

            if (tEventData.isSuccess)
            {
                //PageManager.Instance.OpenPage(PageID.MiniSucceedPage, new MiniSucceedPageParam());
            }
            else
            {
                PropID tUseProp = PropID.Jam3DReplace;
                var tIsValid = GameLogic.JamManager.GetSingleton().Board.CanReplace();
                PageManager.Instance.OpenPage(PageID.MiniFailedPage, new MiniFailedPageParam(tUseProp, tIsValid, (isProp) =>
                {
                    OnReviveDispose(isProp);
                }));
            }
        }

        void SetPauseTime(bool pIsPause)
        {
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

        void OnReviveDispose(bool pIsProp)
        {
            if (JamManager.GetSingleton().ContinueGame())
            {
                AudioManager.Instance.PlaySound(SoundID.Mini_Prop_Recall);
                if (pIsProp)
                {
                    ModuleManager.Prop.ExpendProp(PropID.Jam3DReplace);
                }
            }
        }

        void OnClickProp3_Replace()
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

        void OnClickProp2_Revert()
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

        void OnClickProp1_Shuffle()
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
            var tLevel = ModuleManager.MiniGame.GetCurLevel((int)mGameType);
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
