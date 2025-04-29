using Config;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerConfig", menuName = "GameConfigs/TowerConfig", order = 1)]
public class TowerConfig : ScriptableObject
{
    public List<TowerInfo> Modules;
}

[Serializable]
public class TowerInfo
{
    public int Id;                      // 模块ID
    [Tooltip("开关状态")]
    public bool Status;                 // 模块开关
    [Tooltip("总层数")]
    public int Floor;                   // 总层数
    [Tooltip("可失败次数")]
    public int FailureNumber;           // 可失败次数
    [Tooltip("是否回退至起点")]
    public bool BackToOrigin;           // 是否回退至起点

    public List<TowerReward> Rewards;

    public TowerInfo()
    {
        Id = 1;
        Status = true;
        Floor = 100;
    }
}

[Serializable]
public class TowerReward
{
    public int Floor;                     // 指定奖励层数
    public List<PropData> Items;          // 奖励内容（道具ID和数量）
}
