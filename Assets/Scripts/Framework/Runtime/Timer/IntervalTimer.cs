using System; 
using UnityEngine;
using UnityEngine.UI;

public class IntervalTimer : TimerBase
{
    private Action<float> intervalAction;
    private float passedTime;

    public IntervalTimer(float InDuration,
        Action InOnCompletedAction,
        Action<float> InIntervalAction,
        float InInterval = TimerConfig.DELTA_TIME,
        MonoBehaviour InMonoOwner = null)
        : base(InDuration, InInterval, InOnCompletedAction, InMonoOwner)
    {
        passedTime = 0;
        intervalAction = InIntervalAction;
    }

    public override void Update()
    {
        if (HasMonoOwner && null == MonoOwner)
        {
            LogManager.LogWarning("[IntervalTimer]    MonoBehaviour owner has been destroyed.");
            IsAutoKilled = true;
        }

        //已完成 || 已取消 || 脚本已经被销毁
        if (IsDone) return;
        if (IsPaused) return;

        passedTime += TimerConfig.DELTA_TIME;
        RemainingTimeFloat -= TimerConfig.DELTA_TIME;

        if (passedTime < IntervalsTimeFloat) return;
        passedTime -= IntervalsTimeFloat;

        intervalAction?.Invoke(RemainingTimeFloat);

        if (RemainingTimeFloat <= 0)
        {
            OnCompletedAction?.Invoke();
            IsCompleted = true;
        }
    }

    protected override void UpdateTime()
    {
    }
}