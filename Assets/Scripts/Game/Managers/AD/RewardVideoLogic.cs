using Config;
using System;

public class RewardVideoLogic
{
    bool mIsShowing;
    Action<bool> mCacheAction;
    public void Init()
    {
        EventManager.Register(EventKey.ADShown, OnADShown);
        EventManager.Register(EventKey.ADClosed, OnADClosed);
        EventManager.Register(EventKey.VideoADRewarded, OnVideoADRewarded);
        EventManager.Register(EventKey.ADShowFailed, OnADShowFailed); 
    }

    public void Uninit()
    {
        EventManager.Unregister(EventKey.ADShown, OnADShown);
        EventManager.Unregister(EventKey.ADClosed, OnADClosed);
        EventManager.Unregister(EventKey.VideoADRewarded, OnVideoADRewarded);
        EventManager.Unregister(EventKey.ADShowFailed, OnADShowFailed); 
    }

    public void TryShowRewardVideo(ADShowReason pReason, Action<bool> pAction)
    {
        if (mIsShowing)
        {
            return;
        }

        if (ADManager.Instance.IsRewardVideoReady)
        {
            mCacheAction = pAction;
            ADManager.Instance.ShowRewardVideo(pReason);
        }
    }

    void OnADShown(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.RewardVideo)
        {
            mIsShowing = true;
        }
    }

    void OnADClosed(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.RewardVideo)
        {
            mIsShowing = false;
        }
    }

    void OnVideoADRewarded(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.RewardVideo)
        {
            mCacheAction?.Invoke(true);
            mCacheAction = null;

            ModuleManager.Statistics.AddValue(StatsID.ADVideo, StatsGroup.Total);
            ModuleManager.Statistics.AddValue(StatsID.ADVideo, StatsGroup.TotalDay);

            if (tEventData.showReason == ADShowReason.Video_GetPropHealth)
            {
                ModuleManager.Statistics.AddValue(StatsID.ADHealthTimes, StatsGroup.Total);
            }
            else if (tEventData.showReason == ADShowReason.Video_GetPropHint)
            { 
            }
        }
    }

    void OnADShowFailed(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.RewardVideo)
        {
            mCacheAction?.Invoke(false);
            mCacheAction = null;
        }
    }
}