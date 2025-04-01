using LLFramework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIADLoadingManager : MonoBehaviour
{
    bool isShow;
    public void Init()
    {
        isShow = false;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        if (!isShow)
        {
            isShow = true;
            gameObject.SetActive(true);
            TaskManager.Instance.StartTask(OnLimitTime());
        }
    }

    public void Hide()
    {
        if (isShow)
        {
            isShow = false;
            gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        EventManager.Register(EventKey.ADShown, OnADShown);
        EventManager.Register(EventKey.ADShowFailed, OnADShown);
    }
    void OnDisable()
    {
        EventManager.Unregister(EventKey.ADShown, OnADShown);
        EventManager.Unregister(EventKey.ADShowFailed, OnADShown);
    }

    void OnADShown(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        if (tEventData.ADType == ADType.Interstitial || tEventData.ADType == ADType.RewardVideo)
        {
            Hide();
        }
    }

    IEnumerator OnLimitTime()
    {
        yield return new WaitForSeconds(3);
        Hide();
    }
}
