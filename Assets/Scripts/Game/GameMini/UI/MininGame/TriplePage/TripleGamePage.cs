using Config;
using DG.Tweening;
using Game.UI;
using Game.UISystem;
using System;
using System.Collections;
using TMPro;
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
        [SerializeField] private Button _btnProp4;
        [SerializeField] private Button _btnShop;

        [SerializeField] private Button _gmBtn1;
        [SerializeField] private Button _gmBtn2;
        [SerializeField] private Button _gmBtnTime;

        public UITripleTargetCtrl targetRoot;
        public UITripleArrowCtrl arrowRoot;
        public RectTransform contentArea;
        public TripleMath.BoardController boardRoot;

        public TextMeshProUGUI txtLeftTime;
        public Image imgLeftTime;

        public GameObject effFreeze1;
        public GameObject effFreeze2;
        public RectTransform effFreezeScreen;
        public GameObject effAddTime;
        public TextMeshProUGUI txtAddTime;
        public GameObject effTimeRed;

        public Button BtnProp1 => _btnProp1;
        public Button BtnProp2 => _btnProp2;
        public Button BtnProp3 => _btnProp3;
        public Button BtnProp4 => _btnProp4;

        bool isSuccessLock = false;
        bool isTimeRedEff = false;
        Vector3 leftTimePos;
        int curLeftTime;
        MiniGamePageParam mParam;
        MiniGameType mGameType = MiniGameType.Triple;
        protected override void OnInit()
        {
            _btnBack.onClick.AddListener(OnClickBack);
            _btnShop.onClick.AddListener(OnClickShop);
            _btnProp1.onClick.AddListener(OnClickProp1_Broom);
            _btnProp2.onClick.AddListener(OnClickProp2_Recall);
            _btnProp3.onClick.AddListener(OnClickProp3_Hint);
            _btnProp4.onClick.AddListener(OnClickProp4_AddTime);

#if UNITY_EDITOR || GM_MODE
            _gmBtn1.gameObject.SetActive(true);
            _gmBtn2.gameObject.SetActive(true);
            _gmBtnTime.gameObject.SetActive(true);
            _gmBtn1.onClick.AddListener(() => { MiniGameManager.Instance.TriggerEventGameOver(mGameType, true); });
            _gmBtn2.onClick.AddListener(() => { MiniGameManager.Instance.TriggerEventGameOver(mGameType, false); });
            _gmBtnTime.onClick.AddListener(() =>
            {
                //var beforePlayTime = TileGameArchive.GetTileGamePlayTime();
                //var time = (float)((GameManager.Instance.ServerTime - playBeginTime).TotalSeconds + beforePlayTime);
                //_gmBtnTime.transform.Find("Time").GetComponent<Text>().text = ((int)time).ToString();
            });

#else
            _gmBtn1.gameObject.SetActive(false);
            _gmBtn2.gameObject.SetActive(false);
            _gmBtnTime.gameObject.SetActive(false);
#endif

            targetRoot.Init();
        }

        protected override void RegisterEvents()
        {
            EventManager.Register(EventKey.MiniGameOver, OnMiniGameOver);
            EventManager.Register(EventKey.TripleMath_Failed, OnFailed);
            EventManager.Register(EventKey.TripleMath_ReadyToSuccess, OnReadyToSuccess);

            EventManager.Register(EventKey.TripleMath_Submitted, OnSubmitted);
            EventManager.Register(EventKey.TripleMath_Reset, OnReset);
            EventManager.Register(EventKey.TripleMath_CountDownTime, OnSetCountDownTime);

            EventManager.Register(EventKey.TripleMath_MagnetComplete, OnMagnetComplete);
            EventManager.Register(EventKey.TripleMath_UndoComplete, OnUndoComplete);
            EventManager.Register(EventKey.TripleMath_CompassComplete, OnHintComplete);
            EventManager.Register(EventKey.TripleMath_FreezeComplete, OnFreezeComplete);
            EventManager.Register(EventKey.TripleMath_FreezeFinish, OnFreezeFinish);
            EventManager.Register(EventKey.TripleMath_CompassFinish, OnHintFinish);
            EventManager.Register(EventKey.TripleMath_CompassRefresh, OnHintRefresh);
            EventManager.Register(EventKey.TripleMath_AddTime, OnAddTime);

            EventManager.Register(EventKey.TripleMath_BroomComplete, OnBroomComplete);
            EventManager.Register(EventKey.TripleMath_Recall3ObjectComplete, OnRecallComplete);
            EventManager.Register(EventKey.TripleMath_HourglassComplete, OnAddTimeComplete);
            EventManager.Register(EventKey.TripleMath_BroomFinish, OnBroomFinish);

            if (!GameMethod.HasRemoveAD())
            {
                EventManager.Register(EventKey.PropCountChange, OnPropCountChange);
            }
        }

        protected override void UnregisterEvents()
        {
            EventManager.Unregister(EventKey.MiniGameOver, OnMiniGameOver);
            EventManager.Unregister(EventKey.TripleMath_Failed, OnFailed);
            EventManager.Unregister(EventKey.TripleMath_ReadyToSuccess, OnReadyToSuccess);

            EventManager.Unregister(EventKey.TripleMath_Submitted, OnSubmitted);
            EventManager.Unregister(EventKey.TripleMath_Reset, OnReset);
            EventManager.Unregister(EventKey.TripleMath_CountDownTime, OnSetCountDownTime);

            EventManager.Unregister(EventKey.TripleMath_MagnetComplete, OnMagnetComplete);
            EventManager.Unregister(EventKey.TripleMath_UndoComplete, OnUndoComplete);
            EventManager.Unregister(EventKey.TripleMath_CompassComplete, OnHintComplete);
            EventManager.Unregister(EventKey.TripleMath_FreezeComplete, OnFreezeComplete);
            EventManager.Unregister(EventKey.TripleMath_FreezeFinish, OnFreezeFinish);
            EventManager.Unregister(EventKey.TripleMath_CompassFinish, OnHintFinish);
            EventManager.Unregister(EventKey.TripleMath_CompassRefresh, OnHintRefresh);
            EventManager.Unregister(EventKey.TripleMath_AddTime, OnAddTime);

            EventManager.Unregister(EventKey.TripleMath_BroomComplete, OnBroomComplete);
            EventManager.Unregister(EventKey.TripleMath_Recall3ObjectComplete, OnRecallComplete);
            EventManager.Unregister(EventKey.TripleMath_HourglassComplete, OnAddTimeComplete);
            EventManager.Unregister(EventKey.TripleMath_BroomFinish, OnBroomFinish);
        }

        protected override void OnBeginOpen()
        {
            mParam = PageParam as MiniGamePageParam;
            if (mParam == null)
            {
                LogManager.LogError("MiniGamePage.OnBeginOpen PageParam is null!!!");
                return;
            }

            _txtLevel.text = $"LEVEL {mParam.level}";
            leftTimePos = txtLeftTime.transform.position;
            isSuccessLock = false;

            TripleMath.TripleMathManager.Instance.InitBoard(boardRoot);
            var targetItemDic = TripleMath.TripleMathManager.Instance.GetTargetItemDic();
            targetRoot.InitTarget(targetItemDic);

            RefreshFinishAll();
            RefreshAdapter();

            TripleMath.EventManager.Instance.OnChangeClickState?.Invoke(true);
            TripleMath.EventManager.Instance.OnPauseTime?.Invoke(false);
            TripleMath.EventManager.Instance.OnStartCountDown?.Invoke();
        }

        protected override void OnOpened()
        {
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

            isSuccessLock = false;
            if (tEventData.isSuccess)
            {
                PageManager.Instance.OpenPage(PageID.MiniSucceedPage, new MiniSucceedPageParam());
            }
            else
            {
                //PropID tUseProp = PropID.TripleAddTime;
                //var tIsValid = false;
                //PageManager.Instance.OpenPage(PageID.MiniFailedPage, new MiniFailedPageParam(tUseProp, tIsValid, () =>
                //{
                //    OnReviveDispose();
                //}));
            }
        }

        void OnFailed(EventData pEventData)
        {
            var tEventData = pEventData as TripleMath_Failed;
            var tFailReson = tEventData.failReson;

            PropID tUseProp = PropID.Invalid;
            var tIsValid = false;
            if (tFailReson == FailReson.TimeOut)
            {
                tUseProp = PropID.TripleAddTime;
                tIsValid = ModuleManager.Prop.HasProp(tUseProp);
            }
            else if (tFailReson == FailReson.NoSoftLeft)
            {
                tUseProp = PropID.TripleRevert;
                tIsValid = ModuleManager.Prop.HasProp(tUseProp);
            }

            PageManager.Instance.OpenPage(PageID.MiniFailedPage, new MiniFailedPageParam(tUseProp, tIsValid, () =>
            {
                OnReviveDispose(tUseProp);
            }));
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

        void OnReviveDispose(PropID pPropID)
        {
            var addTime = 0;
            if (pPropID == PropID.TripleRevert)
            {
                OnClickProp2_Recall();
            }
            else if (pPropID == PropID.TripleAddTime)
            {
                if (curLeftTime <= 2)
                {
                    addTime = 20;
                    var tEventData = EventManager.GetEventData<TripleMath_AddTime>(EventKey.TripleMath_AddTime);
                    tEventData.time = addTime;
                    EventManager.Trigger(tEventData);
                }
                OnClickProp4_AddTime();
            }

            TripleMath.EventManager.Instance.OnTriggerReplay?.Invoke(FailReson.NULL, addTime, false, 0);
        }

        void OnSubmitted(EventData pEventData)
        {
            var tEventData = pEventData as TripleMath_Submitted;
            targetRoot.RefershTarget(tEventData.matchType, tEventData.count * -1);
        }

        void OnReset(EventData pEventData)
        {
            var tEventData = pEventData as TripleMath_Reset;
            targetRoot.RefershTarget(tEventData.matchType, tEventData.count);
        }

        void OnSetCountDownTime(EventData pEventData)
        {
            var tEventData = pEventData as TripleMath_CountDownTime;
            curLeftTime = tEventData.leftTime;
            GameVariable.TripleCurTime = curLeftTime;
            var minute = curLeftTime / 60;
            var second = curLeftTime % 60;
            var timeStr = (minute > 9 ? "" : "0") + minute + (second > 9 ? ":" : ":0") + second;
            txtLeftTime.text = timeStr;
            CheckTimeEffect(curLeftTime);
        }

        void ResetTimeEffect()
        {
            isTimeRedEff = false;
            txtLeftTime.color = Color.white;
            imgLeftTime.color = Color.white;
            effTimeRed.gameObject.SetActive(isTimeRedEff);
        }

        void CheckTimeEffect(int countDownTiem)
        {
            if (isTimeRedEff != (countDownTiem <= 10))
            {
                isTimeRedEff = countDownTiem <= 10;
                if (isTimeRedEff)
                {
                    txtLeftTime.color = new Color(1f, 28 / 255f, 28 / 255f);
                    imgLeftTime.color = new Color(1f, 28 / 255f, 28 / 255f);
                }
                else
                {
                    txtLeftTime.color = Color.white;
                    imgLeftTime.color = Color.white;
                }
                effTimeRed.gameObject.SetActive(isTimeRedEff);
            }
        }

        void OnAddTime(EventData pEventData)
        {
            var tEventData = pEventData as TripleMath_AddTime;

            txtAddTime.gameObject.SetActive(false);
            txtAddTime.text = "+" + tEventData.time + "S";
            txtAddTime.transform.localPosition = Vector3.zero;
            effAddTime.gameObject.SetActive(true);
            DOVirtual.DelayedCall(1.8f, () =>
            {
                txtAddTime.gameObject.SetActive(true);
                txtAddTime.transform.DOMove(leftTimePos, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    effFreeze2.transform.position = txtLeftTime.transform.position;
                    effFreeze2.SetActive(true);
                    OnAddTimeFinish();
                });
            });

        }

        void OnAddTimeFinish()
        {
            txtAddTime.gameObject.SetActive(false);
            DOVirtual.DelayedCall(1f, () =>
            {
                effFreeze2.SetActive(false);
                effAddTime.gameObject.SetActive(false);
                if (mHourglassFinish)
                {
                    OnAddTimeFinish(null);
                }
            });

        }

        void OnPropCountChange(EventData pEventData)
        {
            var tEventData = pEventData as PropCountChange;
            if (tEventData.propID == PropID.RemoveAD)
            {
                RefreshAdapter();
            }
        }

        void RefreshFinishAll()
        {
            OnBroomFinish(null);
            OnHintFinish(null);
            OnAddTimeFinish(null);

            OnFreezeFinish(null);
            OnAddTimeFinish();
            ResetTimeEffect();
        }

        void RefreshAdapter()
        {
            Vector3[] corners = new Vector3[4]; //0: 左下，1: 右下，2: 右上，3: 左上
            contentArea.GetWorldCorners(corners);
            float topHeight = (corners[3].y + corners[2].y) / 2f;
            float bottomHeight = (corners[1].y + corners[0].y) / 2f;
            float rate = 1;

            TripleMath.EventManager.Instance.OnRefreshTopAndBottomHeight?.Invoke(topHeight * rate, bottomHeight * rate);
            arrowRoot.OnRefreshTopAndBottomHeight(topHeight * rate, bottomHeight * rate);
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
            SetPauseTime(true);
            PageManager.Instance.OpenPage(PageID.MiniShopSinglePage, new MiniShopSinglePageParam(pPropID));
        }

        #endregion

        #region 道具事件
        bool mBroomNotFinish = false;
        bool mCompassNotFinish = false;
        bool mHourglassFinish = false;

        void SetPauseTime(bool pIsPause)
        {
            TripleMath.EventManager.Instance.OnPauseTime?.Invoke(pIsPause);
        }

        void OnClickProp1_Broom()
        {
            if (mBroomNotFinish) return;
            AudioManager.Instance.PlaySound(SoundID.BtnClick);

            var tPropID = PropID.TripleEraser;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                var tCount = GameMethod.GetPropConfig(tPropID).param1.ToInt();
                TripleMath.EventManager.Instance.OnClickBroom?.Invoke(tCount);
            }
            else
            {
                OpenPropShop(tPropID);
            }
        }
        void OnBroomComplete(EventData pEventData)
        {
            mBroomNotFinish = true;
            ModuleManager.Prop.ExpendProp(PropID.TripleEraser);
        }
        void OnBroomFinish(EventData pEventData)
        {
            mBroomNotFinish = false;
        }

        void OnClickProp2_Recall()
        {
            AudioManager.Instance.PlaySound(SoundID.BtnClick);

            var tPropID = PropID.TripleRevert;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                var tCount = GameMethod.GetPropConfig(tPropID).param1.ToInt();
                TripleMath.EventManager.Instance.OnClickRecell3Object?.Invoke(tCount);
            }
            else
            {
                OpenPropShop(tPropID);
            }
        }
        void OnRecallComplete(EventData pEventData)
        {
            ModuleManager.Prop.ExpendProp(PropID.TripleRevert);
        }

        void OnClickProp3_Hint()
        {
            if (mCompassNotFinish) return;
            AudioManager.Instance.PlaySound(SoundID.BtnClick);

            var tPropID = PropID.TripleHint;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                var targetTrans = TripleMath.EventManager.Instance.OnClickCompass?.Invoke();
                if (targetTrans != null)
                {
                    arrowRoot.StartShowArrow(targetTrans);
                }
            }
            else
            {
                OpenPropShop(tPropID);
            }
        }
        void OnHintComplete(EventData pEventData)
        {
            ModuleManager.Prop.ExpendProp(PropID.TripleHint);
        }
        void OnHintFinish(EventData pEventData)
        {
            mCompassNotFinish = false;
            arrowRoot.StopShowArrow();
        }
        void OnHintRefresh(EventData pEventData)
        {
            var tEventData = pEventData as TripleMath_CompassRefresh;
            if (tEventData?.targetTrans != null)
            {
                arrowRoot.StartShowArrow(tEventData?.targetTrans);
            }
        }


        void OnClickProp4_AddTime()
        {
            if (mHourglassFinish) return;
            AudioManager.Instance.PlaySound(SoundID.BtnClick);

            var tPropID = PropID.TripleAddTime;
            if (ModuleManager.Prop.HasProp(tPropID))
            {
                var tAddTime = GameMethod.GetPropConfig(tPropID).param1.ToInt();
                TripleMath.EventManager.Instance.OnClickHourglass?.Invoke(tAddTime);
            }
            else
            {
                OpenPropShop(tPropID);
            }
        }
        void OnAddTimeComplete(EventData pEventData)
        {
            mHourglassFinish = true;
            var tPropID = PropID.TripleAddTime;
            ModuleManager.Prop.ExpendProp(tPropID);
            var tAddTime = GameMethod.GetPropConfig(tPropID).param1.ToInt();
            var tEventData = EventManager.GetEventData<TripleMath_AddTime>(EventKey.TripleMath_AddTime);
            tEventData.time = tAddTime;
            EventManager.Trigger(tEventData);
        }
        void OnAddTimeFinish(EventData pEventData)
        {
            mHourglassFinish = false;
        }

        #region 旧

        void OnClick_Magnet()
        {
            TripleMath.EventManager.Instance.OnClickMagnet?.Invoke();
        }
        void OnMagnetComplete(EventData pEventData)
        {

        }

        void OnClick_Undo()
        {
            TripleMath.EventManager.Instance.OnClickUndo?.Invoke();
        }
        void OnUndoNoCheck()
        {
            ScrewJam.EventManager.Instance.OnClickAddHoleSlot?.Invoke();
        }
        void OnUndoComplete(EventData pEventData)
        {

        }

        void OnClick_Freeze()
        {
            TripleMath.EventManager.Instance.OnClickFreeze?.Invoke();
        }
        void OnFreezeComplete(EventData pEventData)
        {

        }
        void OnFreezeFinish(EventData pEventData)
        {

        }


        #endregion

        #endregion

        #region 引导
        private void TryShowGuide()
        {

        }

        #endregion
    }
}
