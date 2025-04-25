using LLFramework;
using System;
using System.Collections;
using UnityEngine;

public class TimeManager : MonoSingleton<TimeManager>
{
    public readonly static WaitForSeconds WaitOneSecond = new(1);
    public DateTime ServerTime => DateTime.Now;

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
