using Config;
using Game.UISystem;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MiniGame
{
    public class AdsPropPage : PageBase
    {
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnAD;
        [SerializeField] private Image _imgProp;
        [SerializeField] private Text _txtCount;
        [SerializeField] private Text _txtDesc;

        AdsPropPageParam mParam;
        protected override void OnInit()
        {
            _btnClose.onClick.AddListener(OnClickClose);
            _btnAD.onClick.AddListener(OnPlayAd);
        }

        protected override void OnBeginOpen()
        {
            mParam = PageParam as AdsPropPageParam;
            if (mParam == null)
            {
                LogManager.LogError("AdsPropPage.OnBeginOpen PageParam is null!!!");
                return;
            }

            _imgProp.SetPropIcon(mParam.propID, false);
            _txtCount.text = $"x{mParam.PropCount}";
            _txtDesc.text = $"Watch a video to Get 1 free Hint.";
        }

        #region UI事件

        void OnPlayAd()
        {
            //GameMethod.TriggerUIAction(UIActionName.Na, UIPageName.PopupAddHeart, UIActionType.Agree, ADType.RewardVideo);
            if (ADManager.Instance.IsRewardVideoReady)
            {
                var tADShowReason = DataConvert.GetADShowReason(mParam.propID);
                if (tADShowReason == ADShowReason.Invalid)
                {
                    MessageHelp.Instance.ShowMessage("no internet connection");
                    return;
                }
                ADManager.Instance.PlayRewardVideo(tADShowReason, (isSucceed) =>
                {
                    Close();
                    mParam?.callBack?.Invoke(isSucceed);
                });
            }
            else
            {
                MessageHelp.Instance.ShowMessage("no internet connection");
            }
        }



        void OnClickClose()
        {
            //GameMethod.TriggerUIAction(UIActionName.Na, UIPageName.PopupAddHeart, UIActionType.Refuse);
            Close();
            mParam?.callBack?.Invoke(false);
        }

        #endregion
    }

    public class AdsPropPageParam
    {
        public PropID propID { get; }
        public int PropCount { get; }
        public Action<bool> callBack { get; }

        public AdsPropPageParam(PropID pPropID, int pCount, Action<bool> pCallBack)
        {
            propID = pPropID;
            PropCount = pCount;
            callBack = pCallBack;
        }

        public AdsPropPageParam(PropID pPropID, Action<bool> pCallBack) : this(pPropID, 1, pCallBack) { }

        public AdsPropPageParam(PropData pPropData, Action<bool> pCallBack) : this(pPropData.ID, pPropData.Count, pCallBack) { }

    }
}
