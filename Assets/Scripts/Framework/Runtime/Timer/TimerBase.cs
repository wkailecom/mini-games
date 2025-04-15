using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class TimerBase
{
    protected float DurationFloat;
    protected float RemainingTimeFloat;

    protected int DurationInt;
    protected int RemainingTimeInt;

    protected bool HasMonoOwner;
    protected MonoBehaviour MonoOwner;

    protected Action OnCompletedAction;

    protected bool IsPaused;

    protected bool IsCompleted;
    protected bool IsCancelled;
    protected bool IsAutoKilled;

    public bool IsDone
    {
        get { return IsCompleted || IsCancelled || IsAutoKilled; }
    }

    protected readonly float IntervalsTimeFloat;
    protected readonly int IntervalsTimeInt;

    protected TimerBase(float InDurationFloat, float InIntervalsTimeFloat,
        Action InOnCompletedAction,
        MonoBehaviour InMonoOwner)
    {
        DurationFloat = InDurationFloat;
        RemainingTimeFloat = InDurationFloat;

        IntervalsTimeFloat = InIntervalsTimeFloat;

        MonoOwner = InMonoOwner;
        HasMonoOwner = MonoOwner != null;

        OnCompletedAction = InOnCompletedAction;
    }

    protected TimerBase(int InDurationInt, int InIntervalsTimeInt,
        Action InOnCompletedAction,
        MonoBehaviour InMonoOwner)
    {
        DurationInt = InDurationInt;
        RemainingTimeInt = DurationInt;

        IntervalsTimeInt = InIntervalsTimeInt;

        MonoOwner = InMonoOwner;
        HasMonoOwner = MonoOwner != null;

        OnCompletedAction = InOnCompletedAction;
    }

    public void Run()
    {
        TimerManager.Instance.AddTimer(this);
    }

    public void Pause()
    {
        IsPaused = true;
    }

    public void Resume()
    {
        IsPaused = false;
    }

    public void Cancel()
    {
        IsCancelled = true;
    }

    public void AddTime(int InAddTimeInt)
    {
        if (IsDone)
        {
            LogManager.LogWarning("TimerBase already completed or cancelled.");
            return;
        }

        if (DurationInt == 0)
        {
            DurationFloat += InAddTimeInt;
            RemainingTimeFloat += InAddTimeInt;
        }
        else
        {
            DurationInt += InAddTimeInt;
            RemainingTimeInt += InAddTimeInt;
        }

        UpdateTime();
    }

    public void AddTime(float InAddTimeFloat)
    {
        if (IsDone)
        {
            LogManager.LogWarning("TimerBase already completed or cancelled.");
            return;
        }

        if (DurationInt == 0)
        {
            DurationFloat += InAddTimeFloat;
            RemainingTimeFloat += InAddTimeFloat;
        }
        else
        {
            int timeInt = (int)InAddTimeFloat;
            DurationInt += timeInt;
            RemainingTimeInt += timeInt;
        }

        UpdateTime();
    }

    public void SubTimer(int InTimeInt)
    {
        if (IsDone)
        {
            LogManager.LogWarning("TimerBase already completed or cancelled.");
            return;
        }

        if (DurationInt == 0)
        {
            RemainingTimeFloat = RemainingTimeFloat > InTimeInt ? RemainingTimeFloat - InTimeInt : 0;
            DurationFloat -= InTimeInt;
        }
        else
        {
            RemainingTimeInt = RemainingTimeInt > InTimeInt ? RemainingTimeInt - InTimeInt : 0;
            DurationInt -= InTimeInt;
        }

        UpdateTime();
    }

    public void SubTimer(float InTimeFloat)
    {
        if (IsDone)
        {
            LogManager.LogWarning("TimerBase already completed or cancelled.");
            return;
        }

        if (DurationInt == 0)
        {
            RemainingTimeFloat = RemainingTimeFloat > InTimeFloat ? RemainingTimeFloat - InTimeFloat : 0;
            DurationFloat = DurationFloat > InTimeFloat ? DurationFloat - InTimeFloat : 0;
        }
        else
        {
            int timeInt = (int)InTimeFloat;
            RemainingTimeInt = RemainingTimeInt > timeInt ? RemainingTimeInt - timeInt : 0;
            DurationInt = DurationInt > timeInt ? DurationInt - timeInt : 0;
        }

        UpdateTime();
    }

    public abstract void Update();

    protected abstract void UpdateTime();

    public static TimerTrigger RegisterTimerTrigger(float InDurationFloat,
        Action InOnCompletedAction,
        float InIntervalsTime = .1f,
        MonoBehaviour InMonoOwner = null)
    {
        return new TimerTrigger(InDurationFloat, InOnCompletedAction, InMonoOwner);
    }

    public static Countdown RegisterCountdown(int InDurationInt,
        TimeType InTimeType,
        Text InTimeText,
        Action InOnCompletedAction,
        int InIntervalsTimeInt = 1,
        MonoBehaviour InMonoOwner = null)
    {
        return new Countdown(InDurationInt,
            InTimeType, InTimeText,
            InOnCompletedAction,
            InIntervalsTimeInt,
            InMonoOwner);
    }

    public static Countdown RegisterCountdown(int InDurationInt,
        TimeType InTimeType,
        Action<int, string> InOnUpdate,
        Action InOnCompletedAction,
        int InIntervalsTimeInt = 1,
        MonoBehaviour InMonoOwner = null)
    {
        return new Countdown(InDurationInt,
            InTimeType, InOnUpdate,
            InOnCompletedAction,
            InIntervalsTimeInt,
            InMonoOwner);
    }
}