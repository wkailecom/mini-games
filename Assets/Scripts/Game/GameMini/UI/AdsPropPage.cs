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

            var mConfig = ConfigData.propConfig.GetByPrimary((int)mParam.propID);
            if (mConfig == null)
            {
                return;
            }
            string tPropName = mParam.propID == PropID.Energy ? "ui_heart" : mConfig.icon;
            _imgProp.SetPropIcon(tPropName, false);
            _txtCount.text = $"x{mParam.PropCount}";
            _txtDesc.text = $"Watch a video to Get 1 free Hint.";
        }

        #region UI事件

        void OnPlayAd()
        {
            //GameMethod.TriggerUIAction(UIActionName.Na, UIPageName.PopupAddHeart, UIActionType.Agree, ADType.RewardVideo);
            if (ADManager.Instance.IsRewardVideoReady)
            {
                var tADShowReason = GetADShowReason(mParam.propID);
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

        ADShowReason GetADShowReason(PropID pPropID)
        {
            return pPropID switch
            { 
                PropID.Energy => ADShowReason.Video_GetPropHealth,
                PropID.ScrewExtraSlot => ADShowReason.Video_GetScrewExtraSlot,
                PropID.ScrewHammer => ADShowReason.Video_GetScrewHammer,
                PropID.ScrewExtraBox => ADShowReason.Video_GetScrewExtraBox,
                PropID.Jam3DReplace => ADShowReason.Video_GetJam3DReplace,
                PropID.Jam3DRevert => ADShowReason.Video_GetJam3DRevert,
                PropID.Jam3DShuffle => ADShowReason.Video_GetJam3DShuffle,
                PropID.TileRecall => ADShowReason.Video_GetTileRecall,
                PropID.TileMagnet => ADShowReason.Video_GetTileMagnet,
                PropID.TileShuffle => ADShowReason.Video_GetTileShuffle,
                _ => ADShowReason.Invalid,
            };
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
