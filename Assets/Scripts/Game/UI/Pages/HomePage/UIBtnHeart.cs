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

public class UIBtnHeart : MonoBehaviour
{
    public Canvas canvasRoot;
    public Button buttonRoot;
    public Transform timeRoot;
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
        StopAllCoroutines();
        if (GameMethod.IsFullEnergy())
        {
            timeTxt.text = "FULL";
        }
        else
        {
            StartCoroutine(UpdateTimeTxt(ModuleManager.UserInfo.HealthHarvestTime));
        }
    }

    IEnumerator UpdateTimeTxt(DateTime pHarvestTime)
    {
        while (DateTime.Now < pHarvestTime)
        {
            TimeSpan timeSpan = pHarvestTime - DateTime.Now;
            timeTxt.text = string.Format("{0:D2}m {1:D2}s", (int)timeSpan.TotalMinutes, timeSpan.Seconds);
            yield return TimerManager.WaitOneSecond;
        }
    }



    public void OpenGetADHealth()
    {
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
