using System; 
using UnityEngine;

public sealed class TimerTrigger : TimerBase
{
    private readonly Action<float> onUpdate;

    public TimerTrigger(float InDurationFloat, Action InOnCompletedAction,
        MonoBehaviour InMonoOwner = null)
        : base(InDurationFloat, TimerConfig.DELTA_TIME, InOnCompletedAction, InMonoOwner)
    {
    }

    public TimerTrigger(float InDurationFloat, Action InOnCompletedAction,
        Action<float> InOnUpdate,
        MonoBehaviour InMonoOwner = null)
        : base(InDurationFloat, TimerConfig.DELTA_TIME, InOnCompletedAction, InMonoOwner)
    {
        onUpdate = InOnUpdate;
    }

    public override void Update()
    {
        if (HasMonoOwner && null == MonoOwner)
        {
            LogManager.LogWarning("[TimerBase]    MonoBehaviour owner has been destroyed.");
            IsAutoKilled = true;
        }
        //已完成 || 已取消 || Text脚本已经被销毁
        if (IsDone) return;
        if (IsPaused) return;

        RemainingTimeFloat -= IntervalsTimeFloat;

        if (onUpdate != null) onUpdate.Invoke(RemainingTimeFloat);

        if (RemainingTimeFloat <= 0.00001f)
        {
            if (null != OnCompletedAction) OnCompletedAction.Invoke();
            IsCompleted = true;
        }
    }

    protected override void UpdateTime()
    {
        if (onUpdate != null) onUpdate.Invoke(RemainingTimeFloat);

        if (RemainingTimeFloat <= 0) IsCompleted = true;
    }

    public float getRemainingTimeFloat()
    {
        return RemainingTimeFloat;
    }

}