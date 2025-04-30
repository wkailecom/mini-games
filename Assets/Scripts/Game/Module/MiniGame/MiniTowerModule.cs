using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Config;
using Game;
using Game.UISystem;
using System.Linq;

public enum TowerTopState
{
    Prepare,
    Running,
    End,
}

public class MiniTowerInfo
{
    public int IssueNum;
    public int CurFloor;
    public int RecFloor;
    public int LostNum;
    public List<int> ReceiveFloor;
    public MiniTowerInfo()
    {
        IssueNum = 1;
        CurFloor = 1;
        RecFloor = 1;
        LostNum = 0;
        ReceiveFloor = new List<int>();
    }

    public void SyncFloor(bool pDrop)
    {
        RecFloor = CurFloor;
    }
}


public class MiniTowerModule : ModuleBase
{
    const string RECORD_KEY = "RecordMiniTowerInfo";

    public class TowerActivity
    {
        public int ID;
        public DateTime startTime;
        public DateTime endTime;
        public TowerTopState ActivityState
        {
            get
            {
                if (TimeManager.Instance.ServerTime >= startTime && TimeManager.Instance.ServerTime <= endTime) return TowerTopState.Running;
                if (TimeManager.Instance.ServerTime < startTime) return TowerTopState.Prepare;
                return TowerTopState.End;
            }
        }

        public TimeSpan RemainToStart { get { return startTime - TimeManager.Instance.ServerTime; } }
        public TimeSpan RemainToEnd { get { return endTime - TimeManager.Instance.ServerTime; } }

        public TowerActivity(int pId, DateTime pStart, DateTime pEnd)
        {
            ID = pId; startTime = pStart; endTime = pEnd;
        }
    }


    DateTime firstStart;

    ///////////

    public int CurFloor => mData.CurFloor;
    public int RecFloor => mData.RecFloor;
    public int TotalFloor => mTowerInfo.Floor;
    public TowerInfo TowerInfo => mTowerInfo;

    public Dictionary<int, List<PropData>> nodeRewards;

    int CurLostNum;
    public int CurBalloon => Math.Max(mTowerInfo.FailureNumber - CurLostNum, 0);
    public int RecBalloon => Math.Max(mTowerInfo.FailureNumber - mData.LostNum, 0);

    TowerInfo mTowerInfo;
    TowerConfig mTowerConfig;

    MiniTowerInfo mData;
    protected override void OnInit()
    {
        Deserialize();

        //EventManager.Register(EventKey.MiniGameComplete, OnMiniGameComplete);

        RefreshData();
    }

    void RefreshData()
    {
        firstStart = ModuleManager.UserInfo.FirstLoginTime;
        nodeRewards = new Dictionary<int, List<PropData>>();

        mTowerConfig = ResTool.Load<TowerConfig>(GameConst.CUSTOM_CONFIG_ROOT_PATHD + "/TowerConfig");
        mTowerInfo = mTowerConfig.Modules[0];
        CurLostNum = mData.LostNum;

        foreach (var item in mTowerInfo.Rewards)
        {
            if (nodeRewards.ContainsKey(item.Floor))
            {
                nodeRewards[item.Floor].AddRange(item.Items);
            }
            else
            {
                nodeRewards.Add(item.Floor, item.Items);
            }
        }
    }

    public void TowerUpdate(bool pIsComplete)
    {
        if (pIsComplete)
        {
            mData.CurFloor++;
        }
        else
        {
            CurLostNum++;
            if (CanDrop())
            {
                mData.CurFloor = GetDropFloor(CurFloor);
            }
        }
    }

    public TowerActivity CalcCurrentActivity()
    {
        TimeSpan ts = TimeManager.Instance.ServerTime.Date - firstStart.Date;
        var startDate = new DateTime(firstStart.Year, firstStart.Month, firstStart.Day);
        int curActivityId = ts.Days / 7 + 1;//当前应该第几场(即将开始的或正在进行的)
        DateTime start = curActivityId == 1 ? firstStart : startDate + new TimeSpan(7 * (curActivityId - 1), 10, 0, 0);
        DateTime end = startDate + new TimeSpan(7 * curActivityId, 0, 0, 0);
        return new TowerActivity(curActivityId, start, end); ;//curActivityId 正在进行
    }

    public bool ActivityValid()
    {
        return true;
    }

    bool CanDrop()
    {
        return CurLostNum >= mTowerInfo.FailureNumber;
    }

    int GetDropFloor(int pCurFloor)
    {
        var tRewardFloor = new List<int>(nodeRewards.Keys);
        tRewardFloor.Sort();
        int tR = 1;
        foreach (var tFloor in tRewardFloor)
        {
            if (pCurFloor >= tFloor)
            {
                tR = tFloor;
            }
            else
            {
                return tR;
            }
        }
        return tR;
    }

    public List<PropData> GetNodeReward(int pFloor)
    {
        return nodeRewards.GetValue(pFloor) ?? new List<PropData>();
    }

    public bool HasReward(int pFloor)
    {
        return nodeRewards.ContainsKey(pFloor) && !IsReceive(pFloor);
    }

    public bool IsReceive(int pFloor)
    {
        return mData.ReceiveFloor.Contains(pFloor);
    }

    public void ReceiveReward(int pFloor)
    {
        mData.ReceiveFloor.Add(pFloor);
        Serialize();
    }

    public void SyncFloor(bool pDrop)
    {
        mData.SyncFloor(pDrop);
        if (pDrop)
        {
            mData.LostNum = 0;
            CurLostNum = 0;
        }
        Serialize();
    }

    public void SyncBalloon()
    {
        if (CurBalloon == 0)
        {
            mData.LostNum = 0;
            CurLostNum = 0;
        }
        else
        {
            mData.LostNum = CurLostNum;
        }
        Serialize();
    }



    #region 序列化

    void Serialize()
    {
        DataTool.Serialize(RECORD_KEY, mData);
    }

    void Deserialize()
    {
        mData = DataTool.Deserialize<MiniTowerInfo>(RECORD_KEY);
    }

    #endregion
}
