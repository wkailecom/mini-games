using LLFramework;
using System.Collections.Generic;
using UnityEngine;

public class InputLockManager : Singleton<InputLockManager>
{
    GameObject mLockPanel;
    HashSet<string> mLockNames;

    public void Init()
    {
        mLockPanel = UIRoot.Instance.InputLockPanel;
        mLockNames = new HashSet<string>();
        UpdateLockState();
    }

    public void Lock(string pLockName)
    {
        if (!string.IsNullOrEmpty(pLockName))
        {
            mLockNames.Add(pLockName);
            LogManager.Log("FrameCount: " + Time.frameCount + " Lock <" + pLockName + ">  Lock Count:" + mLockNames.Count);

            UpdateLockState();
        }
    }

    public void UnLock(string pLockName)
    {
        if (!string.IsNullOrEmpty(pLockName) && mLockNames.Contains(pLockName))
        {
            mLockNames.Remove(pLockName);
            LogManager.Log("FrameCount: " + Time.frameCount + " Unlock <" + pLockName + ">  Lock Count:" + mLockNames.Count.ToString());

            UpdateLockState();
        }
    }

    public void UnLockAll()
    {
        mLockNames.Clear();
        UpdateLockState();
    }

    void UpdateLockState()
    {
        mLockPanel.SetActive(mLockNames.Count > 0);
    }
}