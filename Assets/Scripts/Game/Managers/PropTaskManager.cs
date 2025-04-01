using Config;
using Game.UISystem;
using LLFramework;
using System;
using System.Collections;
using System.Collections.Generic;

public class TaskInfo
{
    public string taskName;
    public Action Execute;
    //public Func<bool> StartCondition;
    //public Func<bool> EndCondition;
}

public partial class PopConfig
{
    /// <summary>
    /// 弹出唯一ID
    /// </summary>
    public PageID Id;
    /// <summary>
    /// 权重，权重越大，排序越靠前
    /// </summary>
    public int Weight;

    public static PopConfig[] Datas = new PopConfig[]{
        new () { Id =PageID.EventChestPage, Weight=100},
        new () { Id=PageID.EventPuzzlePage, Weight=200},
        new () { Id=PageID.EventTangramPage, Weight=300},
    };
}

public class PopTaskInfo
{
    public PageID PageID;
    public Action Execute;
    //public PopConfig Config;

    public PopTaskInfo(PageID pPageID, Action pExecute)
    {
        PageID = pPageID;
        Execute = pExecute;

        //Config = new PopConfig();
    }

    public void RunExecute()
    {
        Execute?.Invoke();
    }

    public void Clear()
    {
        Execute = null;
    }

}

public class PopTaskManager : Singleton<PopTaskManager>
{
    List<PopTaskInfo> mTaskList = new(); //正在执行的弹出序列
    PopTaskInfo mCurTask = null;//当前正在执行的弹出
    bool mIsExcuting = false;

    public void Init()
    {
        EventManager.Register(EventKey.PageClosed, OnPageClosed);
    }

    public void Uninit()
    {
        EventManager.Unregister(EventKey.PageClosed, OnPageClosed);
    }

    void OnPageClosed(EventData pEventData)
    {
        var tEventData = pEventData as PageOperation;

        if (tEventData.pageID == PageID.HomePage)
        {
            EndExecute();
        }

        if (mIsExcuting && mCurTask != null && mCurTask.PageID == tEventData.pageID)
        {
            RunNextTask();
        }
    }

    public void AddPop(PageID pPageID, object pPageParam = null)
    {
        AddTask(pPageID, () => { PageManager.Instance.OpenPage(pPageID, pPageParam); });
    }

    public void AddTask(PageID pPageID, Action pExecute)
    {
        var tTask = new PopTaskInfo(pPageID, pExecute);
        mTaskList.Add(tTask);

        if (!mIsExcuting)
        {
            RunNextTask();
        }
    }

    //结束执行弹出逻辑
    public void EndExecute()
    {
        mIsExcuting = false;
        foreach (var item in mTaskList)
        {
            item.Clear();
        }
        mCurTask = null;
        mTaskList.Clear();
    }

    public void RunNextTask()
    {
        if (mTaskList.Count == 0)
        {
            EndExecute();
            return;
        }

        //mTaskList.Sort(Sort);
        // 从队列中取出下一个任务
        mCurTask = mTaskList[0];
        mTaskList.RemoveAt(0);

        mIsExcuting = true;
        mCurTask.RunExecute();
    }

    /*
    int Sort(PopTaskInfo data1, PopTaskInfo data2)
    {
        if (data1.Config.Weight > data2.Config.Weight)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
    */
}
