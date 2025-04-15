using System;
using System.Collections.Generic;
using UnityEngine;
using LLFramework;
public sealed class TimerManager : MonoSingleton<TimerManager>
{
    private List<TimerBase> timers;
    private List<TimerBase> removeTimers;

    protected override void Awake()
    {
        base.Awake();
        timers = new List<TimerBase>(5);
        removeTimers = new List<TimerBase>(1);
    }

    public void AddTimer(TimerBase InTimerBase)
    {
        timers.Add(InTimerBase);
    }

    private float time;

    private void Update()
    {
        foreach (var timer in frameTimer)
        {
            timer.Invoke(Time.deltaTime);
        }

        if (timers == null || timers.Count == 0) return;

        time += Time.deltaTime;

        if (time < TimerConfig.DELTA_TIME) return;
        time -= TimerConfig.DELTA_TIME;

        for (int i = 0; i < timers.Count; i++)
        {
            if (timers[i] != null)
            {
                TimerBase curTimer = timers[i];
                curTimer.Update();
                if (curTimer.IsDone)
                {
                    removeTimers.Add(timers[i]);
                }
            }
        }
        for (int i = 0; i < removeTimers.Count; i++)
        {
            timers.Remove(removeTimers[i]);
        }
        removeTimers.Clear();
    }

    List<Action<float>> frameTimer = new List<Action<float>>();
    public void RegisterFrameTimer(Action<float> action)
    {
        if (frameTimer.Contains(action)) return;
        frameTimer.Add(action);
    }

    public void UnRegisterFrameTimer(Action<float> action)
    {
        frameTimer.Remove(action);
    }

    public TimerBase RunIntervalTimer(float duration, Action complete, Action<float> intervalAction, float interval = TimerConfig.DELTA_TIME, MonoBehaviour monoBehaviour = null)
    {
        TimerBase timer = new IntervalTimer(duration, complete, intervalAction, interval, monoBehaviour);
        timer.Run();
        return timer;
    }
}