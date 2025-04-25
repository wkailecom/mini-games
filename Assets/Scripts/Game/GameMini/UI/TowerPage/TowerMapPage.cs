using Config;
using DG.Tweening;
using Game.MiniGame;
using Game.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.TileGame
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

        RectTransform balloonRect;
        ScrollRectNevigation nevigation;

        int mCurFloor;
        int mRecFloor;
        int mTotalFloor;

        List<RectTransform> nodeList;
        Dictionary<int, List<PropData>> rewardDic;

        private TowerMapPageParam mParam;
        protected override void OnInit()
        {
            //_mapScroll.onValueChanged.AddListener(OnValueChanged);
            _btnHelp.onClick.AddListener(OnClickBtnHelp);
            _btnClose.onClick.AddListener(OnClickBtnClose);
            _btnStart.onClick.AddListener(OnClickBtnStart);

            nodeList = new List<RectTransform>();
            rewardDic = new Dictionary<int, List<PropData>>();
            nevigation = _mapScroll.GetComponent<ScrollRectNevigation>();
            balloonRect = _balloon.GetComponent<RectTransform>();
        }

        protected override void OnBeginOpen()
        {
            base.OnBeginOpen();

            //mParam = PageParam as TowerMapPageParam;
            //if (mParam == null)
            //{
            //    LogManager.LogError("TowerMapPage: invalid param");
            //    return;
            //}

            /////////////////////////// 
            rewardDic.Clear();
            for (int i = 0; i < 120; i++)
            {
                int result = Random.Range(0, 2);
                if (result == 1)
                {
                    rewardDic.Add(i + 1, new List<PropData>()
                    {
                        new PropData(PropID.Coin,10),
                        new PropData(PropID.ScrewExtraBox,20),
                        new PropData(PropID.ScrewExtraBox,20),
                        new PropData(PropID.ScrewExtraBox,20),
                        new PropData(PropID.BusExtraSpot,4),
                        new PropData(PropID.ScrewExtraBox,20),
                    });
                }
            }
            //foreach (var item in ModuleManager.MiniTower.TowerInfo.Rewards)
            //{
            //    rewardDic.Add(item.Floor, new List<PropData>());
            //}
            /////////////////////////
            ///

            mCurFloor = 30;
            mRecFloor = 120;
            mTotalFloor = 120;
            //mCurFloor = ModuleManager.MiniTower.CurFloor;
            //mRecFloor = ModuleManager.MiniTower.RecFloor;
            //mTotalFloor = ModuleManager.MiniTower.TotalFloor;

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

                    bool tIsReceive = false;
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

            var tTargetNode = GetNode(mRecFloor);
            StartCoroutine(nevigation.Nevigate(tTargetNode.gameObject, 0));
            _balloon.transform.position = new Vector2(0, tTargetNode.position.y);

            _balloon.SetCount(ModuleManager.MiniTower.CurBalloon, ModuleManager.MiniTower.CurBalloon);

        }


        protected override void OnOpened()
        {
            var tTargetNode = GetNode(mCurFloor);
            StartCoroutine(nevigation.Nevigate(tTargetNode.gameObject, 3));
            _balloon.transform.DOLocalMoveY(tTargetNode.localPosition.y, 3).OnComplete(() =>
            {
                //_balloon.SetCount(ModuleManager.MiniTower.CurBalloon, ModuleManager.MiniTower.RecBalloon);

                _balloon.SetCount(2, 3);


            });
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


        #region UI事件

        void OnClickBtnHelp()
        {
            PageManager.Instance.OpenPage(PageID.TowerRulePage);
        }

        void OnClickBtnClose()
        {
            if (mParam?.openFrom == TowerMapPageParam.OpenFrom.ReturnByInGame)
            {
                var tConfig = ModuleManager.MiniGame.GetTypeConfig((int)MiniGameManager.Instance.GameType);
                PageManager.Instance.OpenPage(PageID.MiniEnterPage, new MiniEnterPageParam(tConfig));
            }
            //if (CheckReward())
            //{
            //    //StartCoroutine(ConfirmGetReward());
            //}
            //else
            //    Close();
            //PageManager.Instance.OpenPage(PageID.HomePage);
        }

        void OnClickBtnStart()
        {
            //if (isRewarding) { return; }
            //if (CheckReward())
            //{
            //    StartCoroutine(ConfirmGetReward());
            //}
            //else if (TileGameManager.Instance.towerTop.IsOpen)
            //    TileGameManager.Instance.EnterLevel();
            //else
            //    Close();
        }
        #endregion
    }

    public class TowerMapPageParam
    {
        public enum OpenFrom
        {
            LevelSuccess,
            ReturnByInGame,
            ManualClick,
        }

        public bool manual;
        public OpenFrom openFrom;//

        public TowerMapPageParam(OpenFrom pFrom)
        {
            openFrom = pFrom;
        }
    }
}
