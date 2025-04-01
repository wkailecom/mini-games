using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICountDownTMP : MonoBehaviour
{
    public TextMeshProUGUI timeText; // 用于显示倒计时的文本
    public string defaultFinishText = "Time's up!"; // 倒计时结束后的默认文本

    private double remainingSeconds; // 剩余的秒数
    private bool isFinished; // 倒计时是否结束
    private bool forceHourFormat; // 是否强制使用小时格式
    private string customEndText; // 自定义结束时的文本
    private Action onCountDownFinished; // 倒计时结束时的回调

    // 开始倒计时（传入结束时间）
    public void StartCountDown(DateTime endTime, string endText = "", Action onFinish = null, bool pForceHour = false)
    {
        var timeSpan = endTime - DateTime.Now; // 计算剩余时间
        if (timeSpan.TotalSeconds > 0)
        {
            StartCountDown((int)timeSpan.TotalSeconds, endText, onFinish, pForceHour);
        }
        else
        {
            HandleCountDownOver();
        }
    }

    // 开始倒计时（传入秒数）
    public void StartCountDown(int totalSeconds, string endText = "", Action onFinish = null, bool pForceHour = false)
    {
        remainingSeconds = totalSeconds;
        customEndText = endText;
        onCountDownFinished = onFinish;
        forceHourFormat = pForceHour;
        isFinished = false;
        gameObject.SetActive(true); // 显示倒计时UI
    }

    // 停止并隐藏倒计时
    public void StopCountDown(bool pIsClose = false, string pTimeText = null)
    {
        if (!pIsClose && !string.IsNullOrEmpty(pTimeText))
        {
            timeText.text = pTimeText;
        }
        gameObject.SetActive(!pIsClose);
        isFinished = true;
    }

    // 更新倒计时（每帧调用）
    void Update()
    {
        if (isFinished)
        {
            return;
        }

        remainingSeconds -= Time.unscaledDeltaTime; // 使用 unscaledDeltaTime 保持倒计时与游戏暂停无关
        if (remainingSeconds <= 0)
        {
            HandleCountDownOver();
            return;
        }

        UpdateTimeText();
    }

    // 更新UI文本
    private void UpdateTimeText()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(remainingSeconds);
        if (forceHourFormat || timeSpan.TotalDays < 1)
        {
            int totalHours = (int)timeSpan.TotalHours;
            if (totalHours < 1)
            {
                // 小于1小时，显示分钟:秒
                timeText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            }
            else
            { // 时间超过1小时但在1天之内，显示小时:分钟:秒
                timeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", totalHours, timeSpan.Minutes, timeSpan.Seconds);
            }
        }
        else
        {
            // 大于1天时，显示天:小时:分钟
            timeText.text = string.Format("{0}d {1}h", timeSpan.Days, timeSpan.Hours);
        }
    }

    // 处理倒计时结束逻辑
    private void HandleCountDownOver()
    {
        isFinished = true;

        // 设置倒计时结束时显示的文本
        if (!string.IsNullOrEmpty(customEndText))
        {
            timeText.text = customEndText;
        }
        else
        {
            timeText.text = string.IsNullOrEmpty(defaultFinishText) ? "00:00" : defaultFinishText;
        }

        // 执行回调函数
        onCountDownFinished?.Invoke();
    }
}
