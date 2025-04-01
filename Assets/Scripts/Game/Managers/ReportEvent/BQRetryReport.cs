using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LLFramework;

public class BQRetryReport : Singleton<BQRetryReport>
{
    public Queue<(Dictionary<string, object>, Action<bool>)> bqRetryQueue = new();

    bool bqReport = true;
    WaitForSeconds waitTime = new WaitForSeconds(0.1f);
    public void Init()
    {
        TaskManager.Instance.StartCoroutine(HandlePostFormReport());
    }

    public void AddPostFormReport(Dictionary<string, object> pData, Action<bool> pCallback = null)
    {
        bqRetryQueue.Enqueue((pData, pCallback));
    }

    IEnumerator HandlePostFormReport()
    {
        while (true)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable && bqReport && bqRetryQueue.Count > 0)
            {
                bqReport = false;
                var tRetry = bqRetryQueue.Dequeue();
                BQReportManager.Report(tRetry.Item1, tRetry.Item2, () => bqReport = true);
            }
            yield return waitTime;
        }
    }

}
