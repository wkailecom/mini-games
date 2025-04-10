using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;
using UnityEngine.UI;
using Game.UISystem;
using Game.UI;
using Game;
using System;
using System.Threading;

public class UIBtnHeart : MonoBehaviour
{
    public Canvas canvasRoot;
    public Button buttonRoot;
    public UICountDownTMP countDown;
    public GameObject addRoot;
    public TextMeshProUGUI timeTxt;
    public TextMeshProUGUI countTxt;

    int openCount = 0;
    private void Awake()
    {
        buttonRoot.onClick.AddListener(OpenGetADHealth);
    }

    private void OnEnable()
    {
        openCount = 0;
        canvasRoot.overrideSorting = false;
        GameVariable.CurUIBtnHeart = this;

        EventManager.Register(EventKey.PageBeginOpen, OnPageBeginOpen);
        EventManager.Register(EventKey.PageClosed, OnPageClosed);
        EventManager.Register(EventKey.PropCountChange, OnPropCountChange);

        CheckCountdown();
    }

    private void OnDisable()
    {
        EventManager.Unregister(EventKey.PageBeginOpen, OnPageBeginOpen);
        EventManager.Unregister(EventKey.PageClosed, OnPageClosed);
        EventManager.Unregister(EventKey.PropCountChange, OnPropCountChange);

        openCount = 0;
        canvasRoot.overrideSorting = false;
    }

    void OnPageBeginOpen(EventData pEventData)
    {
        var tEventData = pEventData as PageOperation;
        if (tEventData.pageID == PageID.AdsPropPopup || tEventData.pageID == PageID.SwapEnergyPage)
        {
            openCount++;
            canvasRoot.overrideSorting = true;
            canvasRoot.sortingOrder = 500;
        }
    }

    void OnPageClosed(EventData pEventData)
    {
        var tEventData = pEventData as PageOperation;
        if (tEventData.pageID == PageID.AdsPropPopup || tEventData.pageID == PageID.SwapEnergyPage)
        {
            openCount--;
            canvasRoot.overrideSorting = openCount > 0;
        }
    }

    void OnPropCountChange(EventData pEventData)
    {
        var tEventData = pEventData as PropCountChange;
        if (tEventData.propID == PropID.Energy && tEventData.changedCount > 0)
        {
            CheckCountdown();
        }
    }


    void CheckCountdown()
    { 
        if (GameMethod.IsFullEnergy())
        {
            addRoot.SetActive(false);
            countDown.StopCountDown(false, "FULL"); 
        }
        else
        {
            addRoot.SetActive(true);
            countDown.StartCountDown(ModuleManager.UserInfo.HealthHarvestTime, "Full");
        }
    } 

    public void OpenGetADHealth()
    {
        if (GameMethod.IsFullEnergy())
        {
            MessageHelp.Instance.ShowMessage("You energy are full!");
            return;
        }

        var tCurCount = ModuleManager.Prop.GetPropCount(PropID.Coin);
        if (tCurCount >= CommonDefine.energyCoinCount)
        {
            PageManager.Instance.OpenPage(PageID.SwapEnergyPage);
        }
        else
        {
            PageManager.Instance.OpenPage(PageID.ShopPage, new ShopPageParam(ShopPageParam.ShopGroup.CoinFirst));
        }
    }
}
