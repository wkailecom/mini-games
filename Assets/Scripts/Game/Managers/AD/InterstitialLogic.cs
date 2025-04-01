using Config;
using Game;
using Game.UISystem;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class InterstitialLogic
{
    readonly Stopwatch RestAdStopwatch = new();
    int mRestAdAccumulatedTime;
    int RestAdAccumulatedTime
    {
        get => mRestAdAccumulatedTime;
        set => DataTool.SetInt(DataKey.RestAdAccumulatedTime, mRestAdAccumulatedTime = value);
    }

    bool mIsShowing;
    Action<bool> mCacheAction;
    public void Init()
    {
        if (GameMethod.HasRemoveAD())
        {
            return;
        }
        mRestAdAccumulatedTime = DataTool.GetInt(DataKey.RestAdAccumulatedTime, 0);
        EventManager.Register(EventKey.ADShown, OnADShown);
        EventManager.Register(EventKey.ADClosed, OnADClosed);
        EventManager.Register(EventKey.ADShowFailed, OnADShowFailed);
        EventManager.Register(EventKey.PurchaseSuccess, OnPurchaseSuccess);
        EventManager.Register(EventKey.ShowInterstitial, OnShowInterstitial);

        EventManager.Register(EventKey.ApplicationFocus, OnApplicationFocus); 
        EventManager.Register(EventKey.PageBeginOpen, OnPageBeginOpen);
        EventManager.Register(EventKey.PageClosed, OnPageClosed);

        EventManager.Register(EventKey.GameOver, OnGameOver);
    }

    void Uninit()
    {
        EventManager.Unregister(EventKey.ADShown, OnADShown);
        EventManager.Unregister(EventKey.ADClosed, OnADClosed);
        EventManager.Unregister(EventKey.ADShowFailed, OnADShowFailed);
        EventManager.Unregister(EventKey.PurchaseSuccess, OnPurchaseSuccess);
        EventManager.Unregister(EventKey.ShowInterstitial, OnShowInterstitial);

        EventManager.Unregister(EventKey.ApplicationFocus, OnApplicationFocus); 
        EventManager.Unregister(EventKey.PageBeginOpen, OnPageBeginOpen);
        EventManager.Unregister(EventKey.PageClosed, OnPageClosed);

        EventManager.Register(EventKey.GameOver, OnGameOver);
    }

    void TryShowInterstitial(ADShowReason pReason)
    {
        if (mIsShowing)
        {
            return;
        }

        if (ADManager.Instance.IsInterstitialReady)
        {
            ADManager.Instance.ShowInterstitial(pReason);
        }
    }

    public void TryShowInterstitial(ADShowReason pReason, Action<bool> pAction = null)
    {
        if (mIsShowing)
        {
            pAction?.Invoke(false);
            return;
        }

        mCacheAction = pAction;
        ADManager.Instance.ShowInterstitial(pReason);
    }

    void OnADShown(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.Interstitial)
        {
            mIsShowing = true;

            ModuleManager.Statistics.AddValue(StatsID.ADInterstitial, StatsGroup.Total);
            ModuleManager.Statistics.AddValue(StatsID.ADInterstitial, StatsGroup.TotalDay);
        }
    }

    void OnADClosed(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.Interstitial)
        {
            mIsShowing = false;
            mCacheAction?.Invoke(true);
            mCacheAction = null;
        }
    }

    void OnADShowFailed(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.Interstitial)
        {
            mCacheAction?.Invoke(false);
            mCacheAction = null;
        }
    }

    void OnPurchaseSuccess(EventData pEventData)
    {
        var tEventData = pEventData as PurchaseSuccess;
        foreach (var tPropID in tEventData.productConfig.propsID)
        {
            if (tPropID == (int)PropID.RemoveAD)
            {
                Uninit();
                return;
            }
        }
    }

    void OnShowInterstitial(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.Interstitial)
        {
            TryShowInterstitial(tEventData.showReason);
        }
    }

    void OnGameStart(EventData pEventData)
    {
        var tEventData = pEventData as GameStart;
        TryShowInterstitial(ADShowReason.Interstitial_GameStart);
    }

    void OnGameOver(EventData pEventData)
    {
        var tEventData = pEventData as GameOver;

    }

    void OnPageBeginOpen(EventData pEventData)
    {
        var tEventData = pEventData as PageOperation;
        if (tEventData.pageID == PageID.GamePage)
        {
            StartAccumulatedTime();
        }
    }

    void OnPageClosed(EventData pEventData)
    {
        var tEventData = pEventData as PageOperation;
        if (tEventData.pageID == PageID.GamePage)
        {
            StopAccumulatedTime();
        }
    }

    void OnApplicationFocus(EventData pEventData)
    {
        var tEventData = pEventData as ApplicationFocus;
        if (!tEventData.focus)
        {
            StopAccumulatedTime();
        }
        else
        {
            if (GameManager.Instance.GameStart && PageManager.Instance.GetGamePage().IsOpen)
            {
                StartAccumulatedTime();
            }
        }
    }

    void StartAccumulatedTime()
    {
        RestAdStopwatch.Restart();
    }
    void StopAccumulatedTime()
    {
        RestAdStopwatch.Stop();
        RestAdAccumulatedTime += (int)RestAdStopwatch.Elapsed.TotalSeconds;
        RestAdStopwatch.Reset();
    }
     
}