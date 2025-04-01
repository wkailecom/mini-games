using Config;
using DG.Tweening;
using Game.UISystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopFree : MonoBehaviour, IShopItem
{
    public Image imgIcon;
    public TextMeshProUGUI txtName;
    public Button btnFree;
    public Button btnVideo;
    public GameObject btnDisabled;

    public UICountDownTMP txtCountdown;

    //public Animator animVideoIcon;

    Transform mTranCountdown;
    public void Init(IAPProductConfig tProductConfig, Action<string> pOnClickBuy)
    {
        //imgIcon.SetIcon(CommonDefine.freeItemIcon, false);
        //txtName.text = "x 1";
        btnFree.onClick.AddListener(OnClickBtnFree);
        btnVideo.onClick.AddListener(OnClickBtnWatch);
    }

    public void OnShow()
    {
        //EventManager.Register(EventKey.ChangeShopFreeRewards, OnChangeShopFreeRewards);
        EventManager.Register(EventKey.VideoADLoaded, OnVideoADLoaded);
        EventManager.Register(EventKey.VideoADRewarded, OnVideoADRewarded);

        RefreshUIState();
    }

    public void OnHide()
    {
        //EventManager.Unregister(EventDataType.ChangeShopFreeRewards, OnChangeShopFreeRewards);
        EventManager.Unregister(EventKey.VideoADLoaded, OnVideoADLoaded);
        EventManager.Unregister(EventKey.VideoADRewarded, OnVideoADRewarded);
    }

    void RefreshUIState()
    {
        var hasShopFree = true;
        //hasShopFree = ModuleManager.Activity.HasShopFree();
        //string tRemainCount = ADManager.Instance.ShopFreePropRemainTimes.ToString();
        //txtRemainFree.text = tRemainCount;
        //txtRemainVideo.text = tRemainCount;
        if (hasShopFree)
        {
            btnFree.gameObject.SetActive(true);
            btnVideo.gameObject.SetActive(false);
            mTranCountdown.gameObject.SetActive(false);
        }
        else
        {
            //btnFree.gameObject.SetActive(false);
            //mTranCountdown.gameObject.SetActive(true);
            //animVideoIcon.Play(ADManager.Instance.IsShopFreeVideoValid ? "loading" : "inactivation");

            //if (ADManager.Instance.IsShopFreeValid)
            //{
            //    btnVideo.gameObject.SetActive(true);
            //    ModuleManager.Activity.StartShopFreeCountdow();
            //}
            //else
            //{
            //    btnVideo.gameObject.SetActive(false);
            //}

            //txtCountdown.StartCountDown(tFinishTime, () =>
            //{
            //    RefreshUIState();
            //}, endStr: "00:00:00");
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
        //if (ADManager.Instance.IsShopFreeVideoValid)
        //{
        //    ADManager.Instance.ShowRewardVideo(ADShowReason.Video_ShopFreeRotate);
        //}
        //else
        //{
        //    PageManager.Instance.OpenPage(PageID.HintNetPage, new HintNetPageParam(TextTool.GetText("Item_VideoFailed"), TextTool.GetText("Txt_NoNetwork")));
        //}
    }

    void OnClickBtnFree()
    {
        //ModuleManager.Activity.ReceiveShopFree();
        RefreshUIState();
    }

    #endregion
}