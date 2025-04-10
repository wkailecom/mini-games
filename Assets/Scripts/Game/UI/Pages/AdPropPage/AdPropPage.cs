using Config;
using DG.Tweening;
using Game.UISystem;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class AdPropPage : PageBase
    {
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnAD;
        [SerializeField] private Image _imgProp;

        AdPropPageParam mPageParam;
        protected override void OnInit()
        {
            _btnClose.onClick.AddListener(OnClickClose);
            _btnAD.onClick.AddListener(OnPlayAd);
        }

        protected override void OnBeginOpen()
        {
            mPageParam = PageParam as AdPropPageParam;

        }

        #region UI事件

        void OnPlayAd()
        {
            GameMethod.TriggerUIAction(UIActionName.Na, UIPageName.PopupAddHeart, UIActionType.Agree, ADType.RewardVideo);
            if (ADManager.Instance.IsRewardVideoReady)
            {
                ADManager.Instance.PlayRewardVideo(ADShowReason.Video_GetEnergy, (isSucceed) =>
                {
                    if (GameVariable.CurUIBtnHeart != null)
                    {
                        InputLockManager.Instance.Lock("AddHeart");
                        var tImgProp = Instantiate(_imgProp, GameVariable.CurUIBtnHeart.transform);
                        tImgProp.transform.position = _imgProp.transform.position;
                        var tSeq = DOTween.Sequence();
                        tSeq.Append(tImgProp.transform.DOMove(GameVariable.CurUIBtnHeart.countTxt.transform.position, 0.3f));
                        tSeq.Append(tImgProp.DOFade(0, 0.2f));
                        tSeq.OnComplete(() =>
                        {
                            Destroy(tImgProp.gameObject);
                            InputLockManager.Instance.UnLock("AddHeart");
                            Close();
                            mPageParam?.callBack?.Invoke(isSucceed);
                        });
                    }
                    else
                    {
                        Close();
                        mPageParam?.callBack?.Invoke(isSucceed);
                    }

                });
            }
            else
            {
                MessageHelp.Instance.ShowMessage("no internet connection");
            }
        }

        void OnClickClose()
        {
            GameMethod.TriggerUIAction(UIActionName.Na, UIPageName.PopupAddHeart, UIActionType.Refuse);
            Close();
            mPageParam?.callBack?.Invoke(false);
        }

        #endregion
    }

    public class AdPropPageParam
    {
        public Action<bool> callBack { get; }

        public AdPropPageParam(Action<bool> pCallBack)
        {
            callBack = pCallBack;
        }

    }
}
