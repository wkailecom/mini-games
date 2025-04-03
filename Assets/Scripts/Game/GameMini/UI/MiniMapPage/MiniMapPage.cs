using Config;
using DG.Tweening;
using Game.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static Game.MiniGame.UILevelItem;

namespace Game.MiniGame
{
    public class MiniMapPage : PageBase
    {
        [SerializeField] private Text _txtTitle;
        [SerializeField] private ScrollRect _mapScroll;
        [SerializeField] private ScrollRectNevigation _mapNevigation;
        [SerializeField] private Transform _sectionRoot;
        [SerializeField] private Transform _airplane;

        [SerializeField] private Button _btnContinue;
        [SerializeField] private Button _btnNewGame;
        [SerializeField] private Button _btnHelp;
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnShop;
        [SerializeField] private Button _btnTower;

        [SerializeField] private UICountDown _timeCountDown;
        [SerializeField] private UICountDown _shopCountDown;

        [SerializeField] private RectTransform _topBg;
        [SerializeField] private UIMapSection _mapSection;
        [SerializeField] private UILevelItem _mapLevel;
        [SerializeField] private Text _txtFinish;

        [HideInInspector] public int TotalLevel => mTotalLevel;

        public Button BtnGame => _btnNewGame;


        int mTotalLevel;
        int mCurLevel;
        int mRecLevel;
        MiniGameType mGameType;

        int mSectionCount;
        int mCacheCurLevel;
        bool isNevigationing = false;
        bool isRefreshScrollRect = false;
        float mNevigationTime = 1.5f;

        int mViewWidth = 1000;
        int mViewHeight = 1500;
        int mSectionHeight = 2560;

        int mSectionLevelCount = 10;
        int mCacheSectionCount = 3;

        int mCurSectionLoopIndex;
        int mCurLevelLoopIndex;
        UIMapSection mCurSection;
        UILevelItem mCurLevelItem;

        private List<UIMapSection> mSections;
        private List<UIMapSection> mCacheSections;
        public MonoPool<UILevelItem> LevelItemPool;

        protected override void OnInit()
        {
            _btnHelp.onClick.AddListener(OnClickBtnHelp);
            _btnClose.onClick.AddListener(OnClickBtnClose);
            _btnShop.onClick.AddListener(OnClickBtnShop);
            _btnTower.onClick.AddListener(OnClickBtnTower);
            _btnNewGame.onClick.AddListener(OnClickNewGame);
            _btnContinue.onClick.AddListener(OnClickContinueGame);
            _mapScroll.onValueChanged.AddListener(OnValueChanged);

            LevelItemPool = new MonoPool<UILevelItem>(_mapLevel, transform, transform, true);

            mSections = new List<UIMapSection>();
            mCacheSections = new List<UIMapSection>();
            for (int i = 0; i < mCacheSectionCount; i++)
            {
                var tSectionItem = Instantiate(_mapSection, _sectionRoot);
                tSectionItem.Init(this);
                mSections.Add(tSectionItem);
                mCacheSections.Add(tSectionItem);
            }
        }

        protected override void OnBeginOpen()
        {
            base.OnBeginOpen();
            var tEndTime = ModuleManager.MiniGame.EndTime;
            mGameType = ModuleManager.MiniGame.GameType;
            mCurLevel = ModuleManager.MiniGame.CurLevel;
            mRecLevel = ModuleManager.MiniGame.RecLevel;
            mTotalLevel = ModuleManager.MiniGame.MaxLevel;

            //mTotalLevel = 55;
            //mCurLevel = 56;
            //mRecLevel = 42;

            var tIsCompleteAll = mCurLevel > mTotalLevel;
            _timeCountDown.StartCountDown(tEndTime, "Finished");
            _txtTitle.text = GetMapTitle(mGameType);
            mSectionCount = Mathf.CeilToInt((float)mTotalLevel / mSectionLevelCount);
            _topBg.localPosition = new Vector2(0, mSectionCount * mSectionHeight);
            _mapScroll.content.sizeDelta = new Vector2(mViewWidth, CalculateContentHeight());

            if (tIsCompleteAll)
            {
                _btnNewGame.gameObject.SetActive(false);
                _btnContinue.gameObject.SetActive(false);
                _txtFinish.gameObject.SetActive(true);
            }
            else
            {
                _btnNewGame.gameObject.SetActive(true);
                _btnContinue.gameObject.SetActive(false);
                _txtFinish.gameObject.SetActive(false);
            }

            SetLevel();
        }

        protected override void OnOpened()
        {
            AudioManager.Instance.PlayMusic(MusicID.bgm_mini_map);
            TryShowGuide();
        }

        Vector3 tViewPos;
        Vector2 tlocalPos = Vector2.zero;
        UIMapSection tUIMapSection;
        public void OnValueChanged(Vector2 pV2)
        {
            if (!isRefreshScrollRect) return;

            tUIMapSection = mCacheSections[^1];
            tViewPos = _mapScroll.content.localPosition + tUIMapSection.rect.localPosition;
            if (tViewPos.y > _mapScroll.viewport.rect.height && tUIMapSection.index >= mCacheSections.Count)
            {
                tlocalPos.y = tUIMapSection.rect.localPosition.y - mSectionHeight * mCacheSections.Count;
                tUIMapSection.rect.localPosition = tlocalPos;
                tUIMapSection.rect.SetSiblingIndex(1);

                tUIMapSection.SetData(mCacheCurLevel, mCacheSections[0].index - 1);
                mCacheSections.Remove(tUIMapSection);
                mCacheSections.Insert(0, tUIMapSection);
            }

            tUIMapSection = mCacheSections[0];
            tViewPos = _mapScroll.content.localPosition + tUIMapSection.rect.localPosition;
            if (tViewPos.y + mSectionHeight < -_mapScroll.viewport.rect.height && (tUIMapSection.index + mCacheSections.Count) < mSectionCount)
            {
                tlocalPos.y = tUIMapSection.rect.localPosition.y + mSectionHeight * mCacheSections.Count;
                tUIMapSection.rect.localPosition = tlocalPos;
                tUIMapSection.rect.SetSiblingIndex(mCacheSections.Count);

                tUIMapSection.SetData(mCacheCurLevel, mCacheSections[^1].index + 1);
                mCacheSections.Remove(tUIMapSection);
                mCacheSections.Add(tUIMapSection);
            }
        }

        private float CalculateContentHeight()
        {
            int tMaxCount = mTotalLevel;
            int sectionCount = (tMaxCount - 1) / mSectionLevelCount;
            int levelIndexInLastSection = (tMaxCount - 1) % mSectionLevelCount;

            // 获取最后一个元素的y坐标，如果tMaxCount正好是section的整数倍，就取最后一个元素的位置
            float height = UIMapSection.ItemsPos[levelIndexInLastSection].y;

            // 计算总高度 
            return sectionCount * mSectionHeight + height + 600;
        }

        private void SetLevel()
        {
            DOTween.Kill(_mapScroll, true);
            StopCoroutine(nameof(SetStartLevel));
            StartCoroutine(nameof(SetStartLevel));
        }

        IEnumerator SetStartLevel()
        {
            InputLockManager.Instance.Lock("SetStartLevel");

            mCacheCurLevel = mRecLevel;

            var tEndIndex = mSectionCount - 1;
            var tMapIndex = Mathf.CeilToInt((float)mCacheCurLevel / mSectionLevelCount) - 1;

            int tIndex;
            if (tMapIndex + mCacheSections.Count <= tEndIndex)
            {
                for (int i = 0; i < mCacheSections.Count; i++)
                {
                    tIndex = tMapIndex + i;
                    mCacheSections[i].rect.localPosition = new Vector2(0, tIndex * mSectionHeight);
                    mCacheSections[i].SetData(mCacheCurLevel, tIndex);
                }
                mCurSection = mCacheSections[0];
            }
            else
            {
                tMapIndex = tMapIndex > tEndIndex ? tEndIndex : tMapIndex;
                for (int i = mCacheSections.Count - 1; i >= 0; i--)
                {
                    tIndex = tMapIndex - (mCacheSections.Count - 1 - i);
                    mCacheSections[i].rect.localPosition = new Vector2(0, tIndex * mSectionHeight);
                    mCacheSections[i].SetData(mCacheCurLevel, tIndex);
                }
                mCurSection = mCacheSections[^1];
            }

            if (mCacheCurLevel <= mTotalLevel)
            {
                mCurLevelLoopIndex = GameMethod.GetLoopIndex(mCacheCurLevel, mSectionLevelCount);
                mCurSectionLoopIndex = mSections.IndexOf(mCurSection);
            }
            else
            {
                mCurLevelLoopIndex = GameMethod.GetLoopIndex(mTotalLevel, mSectionLevelCount);
            }
            mCurLevelItem = mCurSection.levelItems[mCurLevelLoopIndex];
            //LogManager.LogError(mCurSection.index);

            _airplane.localPosition = GetLevelPos(mCurLevelItem, mCurSection.index);
            yield return SetNevigationLevel(mCurLevelItem.gameObject);

            isRefreshScrollRect = true;
            OnValueChanged(Vector2.zero);
            yield return null;

            if (mCacheCurLevel != mCurLevel)
            {
                yield return UpdateProgress(mCacheCurLevel, mCurLevel);
            }

            InputLockManager.Instance.UnLock("SetStartLevel");
        }

        IEnumerator UpdateProgress(int pLevel, int pCurLevel)
        {
            mCacheCurLevel = pLevel;

            mCurSection = GetCurSection();
            mCurLevelLoopIndex = GameMethod.GetLoopIndex(pLevel, mSectionLevelCount);

            mCurLevelItem = mCurSection.levelItems[mCurLevelLoopIndex];

            if (pLevel < pCurLevel)
            {
                _airplane.DOLocalMove(GetLevelPos(mCurLevelItem, mCurSection.index), mNevigationTime * 0.5f);
                yield return SetNevigationLevel(mCurLevelItem.gameObject, mNevigationTime);
                yield return SetLevelPass(mCurLevelItem, true);
                mCurLevelItem.SetState(ItemState.Unlock);

                if (mCurLevelLoopIndex + 1 == mSectionLevelCount)
                {
                    mCurSectionLoopIndex = (mCurSectionLoopIndex + 1) % mSections.Count;
                }
                yield return UpdateProgress(++pLevel, pCurLevel);
            }
            else
            {
                if (pCurLevel <= mTotalLevel)
                {
                    _airplane.DOLocalMove(GetLevelPos(mCurLevelItem, mCurSection.index), mNevigationTime * 0.5f);
                    yield return SetNevigationLevel(mCurLevelItem.gameObject, mNevigationTime);
                    yield return SetLevelPass(mCurLevelItem);
                    mCurLevelItem.SetState(ItemState.Unlock);
                }
            }

            ModuleManager.MiniGame.SyncLevel();
        }

        IEnumerator SetNevigationLevel(GameObject pLevelItem, float pWaitTime = 0f)
        {
            yield return _mapNevigation.Nevigate(pLevelItem, pWaitTime);
        }

        IEnumerator SetLevelPass(UILevelItem pLevelItem, bool pIsAnim = true)
        {
            if (pIsAnim)
            {
                yield return pLevelItem.PlayPassAnim(0.5f);
            }
            else
            {
                pLevelItem.SetState(ItemState.Unlock);
            }
        }

        UIMapSection GetCurSection()
        {
            return mSections[mCurSectionLoopIndex];
        }

        Vector2 GetLevelPos(UILevelItem pItem, int pSectionIndex)
        {
            var localPos = pItem.transform.localPosition;
            return new Vector2(localPos.x, pSectionIndex * mSectionHeight + localPos.y);
        }

        string GetMapTitle(MiniGameType pType)
        {
            return pType switch
            {
                MiniGameType.Screw => "SCREW JAM",
                MiniGameType.Jam3d => "JAM CRUSH",
                MiniGameType.Tile => "TRIPLE CRUSH",
                _ => "CRUSH"
            };
        }


        #region UI事件

        void OnClickBtnHelp()
        {
            PageManager.Instance.OpenPage(PageID.MiniRulePage);
        }

        void OnClickBtnClose()
        {
            PageManager.Instance.OpenPage(PageID.HomePage);
        }

        void OnClickBtnShop()
        {
            PageManager.Instance.OpenPage(PageID.MiniShopPage);
        }

        void OnClickBtnTower()
        {
            PageManager.Instance.OpenPage(PageID.TowerMapPage);
        }

        void OnClickNewGame()
        {
            OnClickContinueGame();
        }

        void OnClickContinueGame()
        {
            if (ModuleManager.Prop.HasProp(PropID.Energy))
            {
                MiniGameManager.Instance.StartGame(mGameType, mCurLevel);
                if (mCurLevel >= MiniGameConst.AD_OPEN_LEVEL)
                {
                    ADManager.Instance.PlayInterstitial(ADShowReason.Interstitial_MiniGameStart);
                }
            }
            else
            {
                PageManager.Instance.OpenPage(PageID.AdsPropPopup, new AdsPropPageParam(PropID.Energy, null));
            }
        }

        #endregion

        #region 引导

        private void TryShowGuide()
        {
            if (mGameType == MiniGameType.Screw)
            {
                if (mCurLevel == 1 && !DataTool.GetBool(MiniGameConst.Guide_ScrewStart1))
                {
                    DataTool.SetBool(MiniGameConst.Guide_ScrewStart1, true);
                    PageManager.Instance.OpenPage(PageID.MiniGuidePage, MiniGameConst.Guide_ScrewStart1);
                }
                else if (mCurLevel == 2 && !DataTool.GetBool(MiniGameConst.Guide_ScrewStart2))
                {
                    DataTool.SetBool(MiniGameConst.Guide_ScrewStart2, true);
                    PageManager.Instance.OpenPage(PageID.MiniGuidePage, MiniGameConst.Guide_ScrewStart2);
                }
            }
            else if (mGameType == MiniGameType.Jam3d)
            {
                if (mCurLevel == 1 && !DataTool.GetBool(MiniGameConst.Guide_JamStart1))
                {
                    DataTool.SetBool(MiniGameConst.Guide_JamStart1, true);
                    PageManager.Instance.OpenPage(PageID.MiniGuidePage, MiniGameConst.Guide_JamStart1);
                }
                else if (mCurLevel == 2 && !DataTool.GetBool(MiniGameConst.Guide_JamStart2))
                {
                    DataTool.SetBool(MiniGameConst.Guide_JamStart2, true);
                    PageManager.Instance.OpenPage(PageID.MiniGuidePage, MiniGameConst.Guide_JamStart2);
                }
            }
            else if (mGameType == MiniGameType.Tile)
            {

            } 
        }
        #endregion
    }
}