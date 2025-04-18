using Config;
using DG.Tweening;
using Game;
using Game.UISystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopFree : ShopBaseItem
{
    public Image imgIcon;
    public TextMeshProUGUI txtName;
    public Button btnFree;
    public Button btnVideo;
    public GameObject btnDisabled;

    public UICountDownTMP txtCountdown;

    //public Animator animVideoIcon;
    PropData mPropData = new PropData(PropID.Coin, CommonDefine.shopFreeCoinCount);
    Transform mTranCountdown;

    #region 广告记录
    //string LastWatchTimeKey = "LastWatchTimeKey";
    //public DateTime LastWatchTime
    //{
    //    get => new DateTime(DataTool.GetString(LastWatchTimeKey, "0").ToLong());
    //    set => DataTool.SetString(LastWatchTimeKey, value.Ticks.ToString());
    //}
    //public static int CoolingTime => 5 * 60;
    //public static int MaxWatchTimes => 3;
    public static int TodayWatchTimes => ModuleManager.Statistics.GetValue(StatsID.ADShopFreeTimes, StatsGroup.TotalDay);
    public static bool TodayWatchValid => TodayWatchTimes <= CommonDefine.shopFreeCount;
    #endregion

    public override void Init(IAPProductConfig tProductConfig, Action<string> pOnClickBuy)
    {
        imgIcon.SetPropIcon(mPropData.ID);
        txtName.text = $"x {mPropData.Count}";
        btnFree.onClick.AddListener(OnClickBtnFree);
        btnVideo.onClick.AddListener(OnClickBtnWatch);
    }

    public override void OnShow()
    {
        EventManager.Register(EventKey.VideoADLoaded, OnVideoADLoaded);
        EventManager.Register(EventKey.VideoADRewarded, OnVideoADRewarded);

        RefreshUIState();
    }

    public override void OnHide()
    {
        EventManager.Unregister(EventKey.VideoADLoaded, OnVideoADLoaded);
        EventManager.Unregister(EventKey.VideoADRewarded, OnVideoADRewarded);
    }

    void RefreshUIState()
    {
        var tTimeValid = ModuleManager.UserInfo.HasShopFree;//冷却完成
        var tIsFree = TodayWatchTimes == 0;                 //一次未点击时免费,免费点击也当一次广告点击记录
        var tTodayValid = TodayWatchValid;                  //今天次数是否用完

        if (tTodayValid)
        {
            if (tIsFree)
            {
                btnFree.gameObject.SetActive(true);
                btnVideo.gameObject.SetActive(false);
                btnDisabled.gameObject.SetActive(false);
            }
            else
            {
                if (tTimeValid)
                {
                    btnFree.gameObject.SetActive(false);
                    btnVideo.gameObject.SetActive(true);
                    btnDisabled.gameObject.SetActive(false);
                }
                else
                {
                    btnFree.gameObject.SetActive(false);
                    btnVideo.gameObject.SetActive(false);
                    btnDisabled.gameObject.SetActive(true);

                    txtCountdown.StartCountDown(ModuleManager.UserInfo.ShopFreeHarvestTime, "00:00", RefreshUIState);
                }
            }
        }
        else
        {
            btnFree.gameObject.SetActive(false);
            btnVideo.gameObject.SetActive(false);
            btnDisabled.gameObject.SetActive(true);

            var tFinishTime = DateTime.Now.Date.AddDays(1);
            txtCountdown.StartCountDown(tFinishTime, "00:00", RefreshUIState);
        }
    }


    void OnChangeShopFreeRewards(EventData pEventData)
    {
        RefreshUIState();
    }

    void OnVideoADLoaded(EventData pEventData)
    {
        RefreshUIState();
    }

    void OnVideoADRewarded(EventData pEventData)
    {
        RefreshUIState();
    }

    #region UI事件

    void OnClickBtnWatch()
    {
        if (!ADManager.Instance.IsRewardVideoReady)
        {
            MessageHelp.Instance.ShowMessage("The current network environment is unstable. Please try again later.");
            return;
        }
        ADManager.Instance.PlayRewardVideo(ADShowReason.Video_GetCoin, (isf) =>
        {
            if (isf)
            {
                OnClickBtnFree();
            }
        });
    }

    void OnClickBtnFree()
    {
        ModuleManager.Prop.AddProp(mPropData, PropSource.ShopFree);
        ModuleManager.Statistics.AddValue(StatsID.ADShopFreeTimes, StatsGroup.TotalDay);
        PageManager.Instance.OpenPage(PageID.RewardPage, new RewardPageParam(mPropData, PropSource.ShopFree));
        ModuleManager.UserInfo.GatherShopFree();
        RefreshUIState();
    }

    #endregion
}