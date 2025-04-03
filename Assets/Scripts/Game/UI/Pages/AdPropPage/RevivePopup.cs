using Config;
using Game.UISystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class RevivePopup : PageBase
    {

        [SerializeField] private GuideMask _guideMask;
        [SerializeField] private RectTransform _panelRoot;
        [SerializeField] private TextMeshProUGUI _txtDescription;
        [SerializeField] private Button _btnHome;
        [SerializeField] private Button _btnRestart; 

        bool mIsFreeRevive;
        RevivePopupParam mPageParam;
        protected override void OnInit()
        {
            _guideMask.Init();

            _btnHome.onClick.AddListener(OnClickHome);
            _btnRestart.onClick.AddListener(OnClickRestart); 
        }

        protected override void OnBeginOpen()
        {
            mPageParam = PageParam as RevivePopupParam;
            if (mPageParam == null)
            {
                LogManager.LogError(" RevivePopup: invalid param");
                return;
            }

            ModuleManager.UserInfo.IsKillEnd = true;
            var mGamePage = PageManager.Instance.GetGamePage();
           
        }

        protected override void OnClosed()
        {
            ModuleManager.UserInfo.IsKillEnd = false;
        }

        void SetUIItem(UIItem pItemToken)
        {
             
        }


        #region UI事件

        void OnClickHome()
        {
            GameMethod.TriggerUIAction(UIActionName.ReturnHome, UIPageName.PopupRevive, UIActionType.Click, ADType.Interstitial);
            GameManager.Instance.FinishGame(false);
            ModuleManager.Prop.ExpendProp(PropID.Energy);
            ADManager.Instance.PlayInterstitial(ADShowReason.Interstitial_ReturnHome, (isSucceed) =>
            {
                PageManager.Instance.OpenPage(PageID.HomePage);
            });
        }

        void GameRetry()
        {
            GameMethod.TriggerUIAction(UIActionName.Restart, UIPageName.PopupRevive, UIActionType.Click, ADType.Interstitial);
            ModuleManager.Prop.ExpendProp(PropID.Energy);
            ADManager.Instance.PlayInterstitial(ADShowReason.Interstitial_GameRetry, (isSucceed) =>
            {
                GameManager.Instance.RetryGame();
            });
        }

        void GameRevive()
        {
            GameMethod.TriggerUIAction(UIActionName.Revive, UIPageName.PopupRevive, UIActionType.Click, ADType.RewardVideo);
            ModuleManager.Analyse.ReviveTimes++;
            ModuleManager.Statistics.AddValue(StatsID.ReviveTimes, StatsGroup.Total);
            Close();
            mPageParam?.reviveCallBack?.Invoke(true);
        }

        void OnClickRestart()
        {
            if (ModuleManager.Prop.GetPropCount(PropID.Energy) > 1)
            {
                GameRetry();
            }
            else
            {
                PageManager.Instance.OpenPage(PageID.AdPropPopup, new AdPropPageParam((isSucceed) =>
                {
                    if (isSucceed)
                    {
                        GameRetry();
                    }
                }));
            }
        }

        void OnClickRevive()
        {
            if (mIsFreeRevive)
            {
                GameRevive();
            }
            else
            {
                ADManager.Instance.PlayRewardVideo(ADShowReason.Video_GameRevive, (isSucceed) =>
                {
                    if (isSucceed)
                    {
                        GameRevive();
                    }
                });
            }
        }

        #endregion
    }

    public class RevivePopupParam
    {
        public Action<bool> reviveCallBack { get; }

        public RevivePopupParam(Action<bool> pAction)
        {
            reviveCallBack = pAction;
        }

    }
}
