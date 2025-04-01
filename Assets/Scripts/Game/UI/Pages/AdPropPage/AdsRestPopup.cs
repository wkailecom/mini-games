using Config;
using Game.UISystem;
using LLFramework;
using System;
using System.Collections;
using UnityEngine;

namespace Game.UI
{
    public class AdsRestPopup : PageBase
    {

        protected override void OnBeginOpen()
        {
            StartCoroutine(PlayAD());
        }

        IEnumerator PlayAD()
        {
            yield return new WaitForSeconds(0.8f);
            ADManager.Instance.PlayInterstitial(ADShowReason.Interstitial_CumulativeDuration);
            yield return new WaitForSeconds(0.1f);
            Close();
        }

        protected override void RegisterEvents()
        {
            EventManager.Register(EventKey.ADShown, OnADShown);
            EventManager.Register(EventKey.ADShowFailed, OnADShown);
        }

        protected override void UnregisterEvents()
        {
            EventManager.Unregister(EventKey.ADShown, OnADShown);
            EventManager.Unregister(EventKey.ADShowFailed, OnADShown);
        }

        void OnADShown(EventData pEventData)
        {
            var tEventData = pEventData as ADEvent;
            if (tEventData.ADType == ADType.Interstitial || tEventData.ADType == ADType.RewardVideo)
            {
                Close();
            }
        }
    }

}
