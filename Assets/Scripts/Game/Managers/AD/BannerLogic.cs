using Config;
using System;

public class BannerLogic
{
    bool mIsShowing;

    public void Init()
    {
        if (GameMethod.HasRemoveAD())
        {
            return;
        }

        EventManager.Register(EventKey.PurchaseSuccess, OnPurchaseSuccess);
        EventManager.Register(EventKey.PropCountChange, OnPropCountChange);

        TryShowBanner();
    }

    void Uninit()
    {
        EventManager.Unregister(EventKey.PurchaseSuccess, OnPurchaseSuccess);
        EventManager.Unregister(EventKey.PropCountChange, OnPropCountChange);
    }

    void TryShowBanner()
    {
        ADManager.Instance.ShowBanner();
        mIsShowing = true;
    }

    void TryHideBanner()
    {
        ADManager.Instance.HideBanner();
        mIsShowing = false;
    }

    void OnPurchaseSuccess(EventData pEventData)
    {
        var tEventData = pEventData as PurchaseSuccess;
        foreach (var tPropID in tEventData.productConfig.propsID)
        {
            if (tPropID == (int)PropID.RemoveAD)
            {
                ADManager.Instance.HideBanner();
                mIsShowing = false;

                Uninit();
                return;
            }
        }
    }

    void OnPropCountChange(EventData pEventData)
    {
        //if (!AppInfoManager.Instance.IsDebug) return;

        var tEventData = pEventData as PropCountChange;
        if (tEventData.propID == PropID.RemoveAD)
        {
            if (GameMethod.HasRemoveAD())
            {
                ADManager.Instance.HideBanner();
                mIsShowing = false;
            }
            else
            {
                ADManager.Instance.ShowBanner();
                mIsShowing = true;
            }
        }
    }
}