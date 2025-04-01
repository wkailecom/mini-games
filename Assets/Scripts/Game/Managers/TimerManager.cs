using LLFramework;
using System;
using System.Collections;
using UnityEngine;

public class TimerManager : MonoSingleton<TimerManager>
{
    public readonly static WaitForSeconds WaitOneSecond = new(1);
    DateTime cacheTime;
    DateTime cacheHourTime;

    public void Init()
    {
        cacheTime = DateTime.Now;
        cacheHourTime = DateTime.Now;
        StartCoroutine(CheckForNewDay());
    }

    private IEnumerator CheckForNewDay()
    {
        while (true)
        {
            if (!DateTime.Now.Date.Equals(cacheHourTime.Date) || DateTime.Now.Hour != cacheHourTime.Hour)
            {
                cacheHourTime = DateTime.Now;
                EventManager.Trigger(EventKey.StartNewHour);
            }

            if (!DateTime.Now.Date.Equals(cacheTime.Date))
            {
                cacheTime = DateTime.Now;
                EventManager.Trigger(EventKey.StartNewDay);
            }
            yield return WaitOneSecond;
        }
    }

}
