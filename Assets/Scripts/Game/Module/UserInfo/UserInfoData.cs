using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UserInfoData
{  
    public long FirstLoginTime { get; set; }
    public long LastLoginTime { get; set; }

    public long HealthHarvestTime { get; set; }
    public long ShopFreeHarvestTime { get; set; }

    public UserInfoData()
    {  
        FirstLoginTime = DateTime.MinValue.Ticks;
        LastLoginTime = DateTime.Now.Ticks;
        HealthHarvestTime = DateTime.Now.Ticks;
        ShopFreeHarvestTime = DateTime.Now.Ticks;
    }
}
