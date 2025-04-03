using Config;
using Game;
using LLFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UserInfoModule : ModuleBase
{
    const string RECORD_KEY = "RecordUserInfoKey";
    const string RECORD_PAY_KEY = "RecordUserPayOrdersKey";

    public DateTime FirstLoginTime;
    public DateTime LastLoginTime;
    public UserInfoData Data => mData;
    public int InstallDaysCount => (int)(DateTime.Now.Date - FirstLoginTime.Date).TotalDays + 1;
    public int InstallHoursCount => (int)(DateTime.Now - FirstLoginTime).TotalHours + 1;
    public int PayTimes => mUserPayOrders.Count;
    public DateTime LastPayTime => mUserPayOrders.Count == 0 ? DateTime.MinValue : mUserPayOrders.Last().OrderCreateTime;
    public int LastPayDayInterval => mUserPayOrders.Count < 2 ? 0 : (int)(LastPayTime - mUserPayOrders[^2].OrderCreateTime).TotalDays;

    public bool IsKillEnd
    {
        get => PlayerPrefs.GetInt(DataKey.IS_KILL_END_KEY, 0) == 1;
        set => PlayerPrefs.SetInt(DataKey.IS_KILL_END_KEY, value ? 1 : 0);
    }
    public DateTime HealthHarvestTime;
    public DateTime ShopFreeHarvestTime;
    public bool HasShopFree => DateTime.Now >= ShopFreeHarvestTime;
    public bool HasHealth => DateTime.Now >= HealthHarvestTime;

    List<OrderInfo> mUserPayOrders;
    UserInfoData mData;
    protected override void OnInit()
    {
        base.OnInit();
        Deserialize();
        DeserializeUserPay();

        EventManager.Register(EventKey.ApplicationFocus, OnApplicationFocus);
        EventManager.Register(EventKey.PropCountChange, OnPropCountChange);
        EventManager.Register(EventKey.PurchaseSuccess, OnPurchaseSuccess);

        FirstLoginTime = new DateTime(Data.FirstLoginTime);
        LastLoginTime = new DateTime(Data.LastLoginTime);
        HealthHarvestTime = new DateTime(Data.HealthHarvestTime);
        ShopFreeHarvestTime = new DateTime(Data.ShopFreeHarvestTime);
        Data.LastLoginTime = DateTime.Now.Ticks;
        Refresh();
    }

    protected override void OnUninit()
    {
        base.OnUninit();
        Serialize();
        SerializeUserPay();

        EventManager.Unregister(EventKey.ApplicationFocus, OnApplicationFocus);
        EventManager.Unregister(EventKey.PropCountChange, OnPropCountChange);
        EventManager.Unregister(EventKey.PurchaseSuccess, OnPurchaseSuccess);
    }

    public void Refresh()
    {
        if (FirstLoginTime == DateTime.MinValue)
        {
            FirstLoginTime = DateTime.Now;
            Data.FirstLoginTime = FirstLoginTime.Ticks;
            //临时初始生命体力
            if (!ModuleManager.Prop.HasProp(PropID.Energy))
            {
                ModuleManager.Prop.AddProp(PropID.Energy, CommonDefine.energyFunllCount, PropSource.Dfault);
            }
            Serialize();
        }

        if (IsKillEnd)//杀端重进检查扣除体力
        {
            //ModuleManager.Prop.ExpendProp(PropID.Health);
            IsKillEnd = false;
        }

        HarvestHealth();
    }

    void OnApplicationFocus(EventData pEventData)
    {
        var tEventData = pEventData as ApplicationFocus;
        if (!tEventData.focus)
        {
            Serialize();
        }
    }

    void OnPurchaseSuccess(EventData pEventData)
    {
        var tEventData = pEventData as PurchaseSuccess;
        mUserPayOrders.Add(tEventData.orderInfo);
        SerializeUserPay();
    }

    void OnPropCountChange(EventData pEventData)
    {
        var tEventData = pEventData as PropCountChange;
        if (tEventData.propID == PropID.Energy && tEventData.changedCount < 0)
        {
            //改变前满血(即没有恢复计时存在)
            if ((tEventData.currentCount + Mathf.Abs(tEventData.changedCount)) == CommonDefine.energyFunllCount)
            {
                ScheduleNextHealthHarvest(CommonDefine.energyHarvestInterval);
            }
        }
    }

    void HarvestHealth()
    {
        if (GameMethod.IsFullEnergy()) return;

        var tHarvestTime = HealthHarvestTime;
        var tCurHealth = ModuleManager.Prop.GetPropCount(PropID.Energy);
        var tTimeSpan = DateTime.Now - tHarvestTime;

        if (tTimeSpan.TotalSeconds < 0)//未到收获时间
        {
            TaskManager.Instance.StartTask(UpdateCountdown(tHarvestTime));
        }
        else
        {
            int tAddHealth = CalculateHealthToAdd(tTimeSpan.TotalMinutes, tCurHealth);
            if (tAddHealth + tCurHealth < CommonDefine.energyFunllCount)
            {
                var tResidueMinutes = CommonDefine.energyHarvestInterval - tTimeSpan.TotalMinutes % CommonDefine.energyHarvestInterval;
                ScheduleNextHealthHarvest(tResidueMinutes);
            }
            ModuleManager.Prop.AddProp(PropID.Energy, tAddHealth, PropSource.TimeRecover);
        }
    }

    int CalculateHealthToAdd(double totalMinutes, int currentHealth)
    {
        int tAddHealth = (int)(totalMinutes / CommonDefine.energyHarvestInterval) + 1;
        if (tAddHealth + currentHealth >= CommonDefine.energyFunllCount)
        {
            tAddHealth = CommonDefine.energyFunllCount - currentHealth;
        }
        return tAddHealth;
    }

    IEnumerator UpdateCountdown(DateTime pHarvestTime)
    {
        while (DateTime.Now < pHarvestTime)
        {
            // TimeSpan timeSpan = pHarvestTime - DateTime.Now;
            yield return TimerManager.WaitOneSecond;
        }

        if (ModuleManager.Prop.GetPropCount(PropID.Energy) < CommonDefine.energyFunllCount - 1)
        {
            ScheduleNextHealthHarvest(CommonDefine.energyHarvestInterval);
        }
        if (!GameMethod.IsFullEnergy())
        {
            ModuleManager.Prop.AddProp(PropID.Energy, 1, PropSource.TimeRecover);
        }
    }

    void ScheduleNextHealthHarvest(double tResiduetTime)
    {
        var tHarvestTime = DateTime.Now.AddMinutes(tResiduetTime);
        UpdateHealthHarvestTime(tHarvestTime);
        TaskManager.Instance.StartTask(UpdateCountdown(tHarvestTime));
    }

    public void UpdateHealthHarvestTime(DateTime pHarvestTime)
    {
        if (pHarvestTime > DateTime.Now)
        {
            HealthHarvestTime = pHarvestTime;
            Data.HealthHarvestTime = HealthHarvestTime.Ticks;
            Serialize();
        }
    }

    public void UpdateShopFreeHarvestTime()
    {
        ShopFreeHarvestTime = DateTime.Now.AddMinutes(CommonDefine.shopFreeTimeInterval);
        Data.ShopFreeHarvestTime = ShopFreeHarvestTime.Ticks;
        Serialize();
    }

    public OrderInfo GetOrderInfo(string pProductID)
    {
        foreach (var tOrder in mUserPayOrders)
        {
            if (tOrder.ProductID.Equals(pProductID))
            {
                return tOrder;
            }
        }
        return null;
    }

    #region 序列化 & 反序列化
    void Serialize()
    {
        DataTool.Serialize(RECORD_KEY, mData);
    }

    void Deserialize()
    {
        mData = DataTool.Deserialize<UserInfoData>(RECORD_KEY);
    }

    void SerializeUserPay()
    {
        StringBuilder tResult = new StringBuilder();
        foreach (var tOrderInfo in mUserPayOrders)
        {
            if (tResult.Length > 0)
            {
                tResult.Append('&');
            }

            tResult.Append(tOrderInfo.Serialize());
        }

        DataTool.SetString(RECORD_PAY_KEY, tResult.ToString());
    }

    void DeserializeUserPay()
    {
        mUserPayOrders = new List<OrderInfo>();
        var tResult = DataTool.GetString(RECORD_PAY_KEY);

        if (!string.IsNullOrEmpty(tResult))
        {
            foreach (var tOrderInfoString in tResult.Split('&'))
            {
                var tOrderInfo = new OrderInfo(tOrderInfoString);
                if (tOrderInfo?.ProductConfig != null) mUserPayOrders.Add(tOrderInfo);
            }
        }
    }

    #endregion



}
