using Config;
using System.Collections;
using UnityEngine;

public class AudioPlayer
{
    public void Init()
    {
        //EventManager.Register(EventKey.PurchaseSuccess, OnPurchaseSuccess);
        //EventManager.Register(EventKey.VideoADRewarded, OnGetReward); 
    }

    //void OnPurchaseSuccess(EventData pEventData)
    //{
    //    var tEventData = pEventData as PurchaseSuccess;
    //    if (tEventData.isRestore) return;

    //    AudioManager.Instance.PlaySound(SoundID.GetReward);
    //}

    //void OnGetReward(EventData pEventData)
    //{
    //    var tEventData = pEventData as ADEvent;
    //    if (tEventData.showReason == ADShowReason.Video_ShopFreeRotate)
    //    {
    //        AudioManager.Instance.PlaySound(SoundID.GetReward);
    //    }

    //}

}