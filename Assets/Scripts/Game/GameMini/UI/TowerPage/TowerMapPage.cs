using Config;
using DG.Tweening;
using Game.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.MiniGame
{
    public class TowerMapPage : PageBase
    {
        [SerializeField] private Text _txtTitle;
        [SerializeField] private Button _btnHelp;
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnStart;

        [SerializeField] private UICountDown _timeCountDown;
        [SerializeField] private ScrollRect _mapScroll;

        [SerializeField] private RectTransform _bgRoot;
        [SerializeField] private RectTransform _branchRoot;
        [SerializeField] private RectTransform _trunkRoot;
        [SerializeField] private RectTransform _nodeRoot;

        [SerializeField] private RectTransform _branchItem;
        [SerializeField] private RectTransform _trunkItem;
        [SerializeField] private RectTransform _nodeItem;
        [SerializeField] private RectTransform _giftItem;
        [SerializeField] private RectTransform[] _cloudItems;

        [SerializeField] private UIBalloon _balloon;
        [SerializeField] private Transform _balloonDefaultPos;

        RectTransform balloonRect;
        ScrollRectNevigation nevigation;

        int mCurFloor;
        int mRecFloor;
        int mTotalFloor;

        List<RectTransform> nodeList;
        Dictionary<int, RectTransform> giftDic;
        Dictionary<int, List<PropData>> rewardDic;

        private TowerMapPageParam mParam;
        protected override void OnInit()
        {
            //_mapScroll.onValueChanged.AddListener(OnValueChanged);
            _btnHelp.onClick.AddListener(OnClickBtnHelp);
            _btnClose.onClick.AddListener(OnClickBtnClose);
            _btnStart.onClick.AddListener(OnClickBtnStart);

            nodeList = new List<RectTransform>();
            giftDic = new Dictionary<int, RectTransform>();
            rewardDic = new Dictionary<int, List<PropData>>();
            nevigation = _mapScroll.GetComponent<ScrollRectNevigation>();
            balloonRect = _balloon.GetComponent<RectTransform>();
        }

        protected override void OnBeginOpen()
        {
            base.OnBeginOpen();

            SetBtnShow(_btnClose, false);
            SetBtnShow(_btnStart, false);
            var tActivity = ModuleManager.MiniTower.CalcCurrentActivity();
            _timeCountDown.StartCountDown(tActivity.endTime, "FINISHED", Close);

            mParam = PageParam as TowerMapPageParam;
            if (mParam == null)
            {
                SetBtnShow(_btnClose, true);
                LogManager.LogError("TowerMapPage: invalid param");
                return;
            }

            //mCurFloor = 30;
            //mRecFloor = 120;
            mCurFloor = ModuleManager.MiniTower.CurFloor;
            mRecFloor = ModuleManager.MiniTower.RecFloor;
            mTotalFloor = ModuleManager.MiniTower.TotalFloor;
            rewardDic = ModuleManager.MiniTower.nodeRewards;

            nodeList.Clear();
            giftDic.Clear();
            var tTrunkHeight = _trunkItem.sizeDelta.y;
            float tStartHeight = 800;
            float tNodeInterval = 400;

            var tContentHeight = mTotalFloor * tNodeInterval + tStartHeight + 200f;
            var tTrunkCount = Mathf.CeilToInt(tContentHeight / tTrunkHeight);
            _mapScroll.content.sizeDelta = new Vector2(0, tContentHeight);
            _mapScroll.verticalNormalizedPosition = 0;

            RandomCreateCloud();

            for (int i = 0; i < tTrunkCount; i++)
            {
                var tTrunk = Instantiate(_trunkItem, _trunkRoot);
                tTrunk.anchoredPosition = new Vector3(0, i * tTrunkHeight, 0);
            }

            for (int i = 0; i < mTotalFloor; i++)
            {
                var tFloor = i + 1;
                var tNode = Instantiate(_nodeItem, _nodeRoot);
                tNode.anchoredPosition = new Vector3(-250, i * tNodeInterval + tStartHeight, 0);
                tNode.GetComponentInChildren<Text>().text = tFloor.ToString();
                nodeList.Add(tNode);

                if (rewardDic.ContainsKey(tFloor))
                {
                    tNode.GetChild(1).gameObject.SetActive(true);

                    var tBranch = Instantiate(_branchItem, _branchRoot);
                    tBranch.anchoredPosition = new Vector3(-200, i * tNodeInterval + tStartHeight, 0);

                    var tGift = Instantiate(_giftItem, _nodeRoot);
                    tGift.anchoredPosition = new Vector3(250, tBranch.anchoredPosition.y, 0);
                    giftDic.Add(tFloor, tGift);

                    var tBtn = tGift.GetComponentInChildren<Button>(true);
                    var tAnim = tGift.GetComponentInChildren<Animator>(true);
                    var tPreview = tGift.GetComponentInChildren<UIPreviewRewards>(true);

                    int offsetX = 0;
                    var tRewardCount = rewardDic[tFloor].Count;
                    if (tRewardCount >= 5)
                    {
                        offsetX = -50 * (tRewardCount - 4);
                    }
                    tPreview.SetData(rewardDic[tFloor], offsetX);
                    tPreview.gameObject.SetActive(false);
                    tBtn.onClick.RemoveAllListeners();
                    tBtn.onClick.AddListener(() =>
                    {
                        tPreview.gameObject.SetActive(true);
                        DG.Tweening.DOVirtual.DelayedCall(3f, () =>
                        {
                            tPreview.gameObject.SetActive(false);
                        });
                    });

                    bool tIsReceive = ModuleManager.MiniTower.IsReceive(tFloor);
                    if (tIsReceive)
                    {
                        if (tFloor == mTotalFloor)
                        {
                            tAnim.SetInteger("SpecialOpen", 1);
                            tAnim.Play("Open02", 0, 1);
                        }
                        else
                        {
                            tAnim.SetInteger("NormalOpen", 1);
                            tAnim.Play("Open03", 0, 1);
                        }
                    }
                    else
                    {
                        if (tFloor == mTotalFloor)
                        {
                            tAnim.SetInteger("SpecialOpen", 2);
                        }
                        else
                        {
                            tAnim.SetInteger("NormalOpen", 2);
                        }
                    }
                }
            }

            var tCurBalloon = ModuleManager.MiniTower.CurBalloon;
            var tRecBalloon = ModuleManager.MiniTower.RecBalloon;

            //当前<记录，存在破裂并且需要上升，直接定位到上升位置 
            if (tCurBalloon < tRecBalloon && mCurFloor > mRecFloor)
            {
                mRecFloor = mCurFloor;
                ModuleManager.MiniTower.SyncFloor(false);
            }
            var tTargetNode = GetNode(mRecFloor);
            nevigation.Nevigate(tTargetNode.gameObject, 0);
            _balloon.transform.position = new Vector2(0, tTargetNode.position.y);
            _balloon.SetCount(tRecBalloon, tRecBalloon);

        }

        protected override void OnOpened()
        {
            StartCoroutine(UpdateFloor());
        }

        IEnumerator UpdateFloor()
        {
            InputLockManager.Instance.Lock("UpdateFloor");

            var tCurBalloon = ModuleManager.MiniTower.CurBalloon;
            var tRecBalloon = ModuleManager.MiniTower.RecBalloon;
            var tMaxBalloon = ModuleManager.MiniTower.TowerInfo.FailureNumber;
            var tWaitTime = 2;

            if (tCurBalloon != tRecBalloon)
            {
                if (tCurBalloon < tRecBalloon) // 存在破裂
                {
                    _balloon.SetCount(tCurBalloon, tRecBalloon);
                    yield return new WaitForSeconds(1f);
                }
                ModuleManager.MiniTower.SyncBalloon();
            }

            if (mCurFloor > mRecFloor)//上升
            {
                var tTargetNode = GetNode(mCurFloor);
                nevigation.Nevigate(tTargetNode.gameObject, tWaitTime);
                _balloon.transform.DOLocalMoveY(tTargetNode.localPosition.y, tWaitTime);
                yield return new WaitForSeconds(tWaitTime);

                ModuleManager.MiniTower.SyncFloor(false);
                yield return CheckReward();
            }
            else if (mCurFloor < mRecFloor)//下降
            {
                var tTargetNode = GetNode(mCurFloor);
                nevigation.Nevigate(tTargetNode.gameObject, tWaitTime);
                _balloon.transform.DOLocalMoveY(tTargetNode.localPosition.y, tWaitTime);
                yield return new WaitForSeconds(tWaitTime);

                ModuleManager.MiniTower.SyncFloor(true);
                if (tCurBalloon == 0)
                {
                    _balloon.ReOpen();
                    yield return new WaitForSeconds(1f);
                }
            }
            InputLockManager.Instance.UnLock("UpdateFloor");

            if (mParam?.openFrom == TowerMapPageParam.OpenFrom.EnterClick)
            {
                SetBtnShow(_btnClose, true);
                SetBtnShow(_btnStart, true);
            }
            else
            {
                mParam?.closeAction?.Invoke();
                Close();
            }

        }

        void RandomCreateCloud()
        {
            for (int i = 0; i < 30; i++)
            {
                int tIndex = Random.Range(0, _cloudItems.Length);
                var tCloud = Instantiate(_cloudItems[tIndex], _bgRoot);
                float x = Random.Range(-540 / 2f, 540f);
                float y = Random.Range(-_mapScroll.content.sizeDelta.y / 2f + 1500, _mapScroll.content.sizeDelta.y / 2f);
                float size = Random.Range(1, 2f);
                tCloud.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                tCloud.transform.localScale = Vector3.one * size;
            }
        }

        RectTransform GetNode(int pFloor)
        {
            if (pFloor <= 0) return nodeList[0];
            if (pFloor >= nodeList.Count) return nodeList[^1];

            return nodeList[pFloor - 1];
        }

        IEnumerator CheckReward()
        {
            var tHasReward = ModuleManager.MiniTower.HasReward(mCurFloor);
            if (tHasReward)
            {
                if (giftDic.TryGetValue(mCurFloor, out var tGift))
                {
                    var tAnim = tGift.GetComponentInChildren<Animator>(true);
                    tAnim.enabled = true;
                    if (mCurFloor == mTotalFloor)
                    {
                        tAnim.SetInteger("SpecialOpen", 1);
                    }
                    else
                    {
                        tAnim.SetInteger("NormalOpen", 1);
                    }
                    //tAnim.SetTrigger("Open"); 
                    yield return new WaitForSeconds(0.8f);
                }
                ModuleManager.MiniTower.ReceiveReward(mCurFloor);
            }
        }

        #region 复用
        //List<RectTransform> mCacheTrunkItems;
        //float mTrunkHeight;
        //bool isRefreshScrollRect = false;
        //Vector3 tViewPos;
        //Vector2 tlocalPos = Vector2.zero;
        //RectTransform tUITrunkItem;
        //public void OnValueChanged(Vector2 pV2)
        //{
        //    if (!isRefreshScrollRect) return;

        //    tUITrunkItem = mCacheTrunkItems[^1];
        //    tViewPos = _mapScroll.content.localPosition + tUITrunkItem.localPosition;
        //    if (tViewPos.y > _mapScroll.viewport.rect.height && tUITrunkItem.index >= mCacheTrunkItems.Count)
        //    {
        //        tlocalPos.y = tUITrunkItem.localPosition.y - mTrunkHeight * mCacheTrunkItems.Count;
        //        tUITrunkItem.localPosition = tlocalPos;
        //        tUITrunkItem.SetSiblingIndex(1);

        //        //tUITrunkItem.SetData(mCacheCurrentLevel, mCacheTrunkItems[0].index - 1);
        //        mCacheTrunkItems.Remove(tUITrunkItem);
        //        mCacheTrunkItems.Insert(0, tUITrunkItem);
        //    }

        //    tUITrunkItem = mCacheTrunkItems[0];
        //    tViewPos = _mapScroll.content.localPosition + tUITrunkItem.localPosition;
        //    if (tViewPos.y + mTrunkHeight < -_mapScroll.viewport.rect.height && (tUITrunkItem.index + mCacheTrunkItems.Count) < EndUIMapItem.index)
        //    {
        //        tlocalPos.y = tUITrunkItem.localPosition.y + mTrunkHeight * mCacheTrunkItems.Count;
        //        tUITrunkItem.localPosition = tlocalPos;
        //        tUITrunkItem.SetSiblingIndex(mCacheTrunkItems.Count);

        //        //tUITrunkItem.SetData(mCacheCurrentLevel, mCacheTrunkItems[mCacheTrunkItems.Count - 1].index + 1);
        //        mCacheTrunkItems.Remove(tUITrunkItem);
        //        mCacheTrunkItems.Add(tUITrunkItem);
        //    }
        //}
        #endregion

        void SetBtnShow(Button pBtn, bool pIsShow)
        {
            var tBtnGroup = pBtn.GetComponent<CanvasGroup>();
            if (pIsShow)
            {
                pBtn.enabled = false;
                tBtnGroup.alpha = 0;
                DOTween.Sequence().Append(tBtnGroup.DOFade(1, 0.5f)).OnComplete(() =>
                {
                    pBtn.enabled = true;
                });
            }
            else
            {
                tBtnGroup.alpha = 0;
                pBtn.enabled = false;
            }
        }

        #region UI事件

        void OnClickBtnHelp()
        {
            PageManager.Instance.OpenPage(PageID.TowerRulePage);
        }

        void OnClickBtnClose()
        {
            Close();
        }

        void OnClickBtnStart()
        {
            MiniGameManager.Instance.NextGame();
        }
        #endregion
    }

    public class TowerMapPageParam
    {
        public enum OpenFrom
        {
            LevelSuccess,
            LevelFail,
            ExitLevel,
            EnterClick,
        }

        public OpenFrom openFrom;  //打开来源
        public Action closeAction; //关闭回调

        public TowerMapPageParam(OpenFrom pFrom, Action pAction = null)
        {
            openFrom = pFrom;
            closeAction = pAction;
        }
    }
}
