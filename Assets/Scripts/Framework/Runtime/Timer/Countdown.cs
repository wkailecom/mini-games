using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class Countdown : TimerBase
{
    private readonly Text timeText;

    private readonly TimeType timeType;

    private readonly Action<int, string> onUpdate;

    private float passedTime;
    private int hour, minute, second;
    private string timeStr;

    public Countdown(int InDurationInt,
        TimeType InTimeType,
        Text InTimeText,
        Action InOnCompletedAction,
        int InIntervalsTimeInt = 1,
        MonoBehaviour InMonoOwner = null)
        : base(InDurationInt, InIntervalsTimeInt, InOnCompletedAction, InMonoOwner)
    {
        timeText = InTimeText;

        timeType = InTimeType;

        passedTime = 0;
    }

    public Countdown(int InDurationInt,
        TimeType InTimeType,
        Action<int, string> InOnUpdate,
        Action InOnCompletedAction,
        int InIntervalsTimeInt = 1,
        MonoBehaviour InMonoOwner = null)
        : base(InDurationInt, InIntervalsTimeInt, InOnCompletedAction, InMonoOwner)
    {
        onUpdate = InOnUpdate;

        timeType = InTimeType;

        passedTime = 0;
    }

    public override void Update()
    {
        //已完成 || 已取消 || Text脚本已经被销毁
        if (IsDone) return;
        if (IsPaused) return;

        passedTime += TimerConfig.DELTA_TIME;
        if (passedTime < IntervalsTimeInt) return;

        passedTime -= IntervalsTimeInt;
        RemainingTimeInt -= IntervalsTimeInt;

        switch (timeType)
        {
            case TimeType.hhmmss:
                hour = RemainingTimeInt / 3600;
                minute = RemainingTimeInt % 3600 / 60;
                second = RemainingTimeInt % 60;
                timeStr = (hour > 9 ? "" : "0") + hour + (minute > 9 ? ":" : ":0") + minute + (second > 9 ? ":" : ":0") + second;
                break;

            case TimeType.mmss:
                minute = RemainingTimeInt % 3600 / 60;
                second = RemainingTimeInt % 60;
                timeStr = (minute > 9 ? "" : "0") + minute + (second > 9 ? ":" : ":0") + second;
                break;

            case TimeType.ss:
                timeStr = RemainingTimeInt > 9 ? RemainingTimeInt.ToString() : "0" + RemainingTimeInt;
                break;

            default:
                timeStr = string.Empty;
                break;
        }

        if (HasMonoOwner && null == MonoOwner)
        {
            LogManager.LogWarning("[TimerBase]    MonoBehaviour owner has been destroyed.");
            IsAutoKilled = true;
        }

        if (null != timeText) timeText.text = timeStr;

        if (onUpdate != null) onUpdate.Invoke(RemainingTimeInt, timeStr);

        if (RemainingTimeInt <= 0)
        {
            if (OnCompletedAction != null) OnCompletedAction.Invoke();
            IsCompleted = true;
        }
    }

    protected override void UpdateTime()
    {
        switch (timeType)
        {
            case TimeType.hhmmss:
                hour = RemainingTimeInt / 3600;
                minute = RemainingTimeInt % 3600 / 60;
                second = RemainingTimeInt % 60;
                timeStr = (hour > 9 ? "" : "0") + hour + (minute > 9 ? ":" : ":0") + minute + (second > 9 ? ":" : ":0") + second;
                break;

            case TimeType.mmss:
                minute = RemainingTimeInt % 3600 / 60;
                second = RemainingTimeInt % 60;
                timeStr = (minute > 9 ? "" : "0") + minute + (second > 9 ? ":" : ":0") + second;
                break;

            case TimeType.ss:
                timeStr = RemainingTimeInt > 9 ? RemainingTimeInt.ToString() : "0" + RemainingTimeInt;
                break;

            default:
                timeStr = string.Empty;
                break;
        }

        if (null != timeText) timeText.text = timeStr;

        if (onUpdate != null) onUpdate.Invoke(RemainingTimeInt, timeStr);

        if (RemainingTimeInt <= 0)
        {
            if (OnCompletedAction != null) OnCompletedAction.Invoke();
            IsCompleted = true;
        }
    }
}