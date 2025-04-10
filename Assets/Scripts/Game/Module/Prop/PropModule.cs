using Config;
using Game;
using Game.UISystem;
using System.Collections.Generic;

public class PropModule : ModuleBase
{
    const string RECORD_KEY = "RecordPropsKey";

    Dictionary<int, PropData> mData;

    protected override void OnInit()
    {
        base.OnInit();
        Deserialize();

        EventManager.Register(EventKey.PurchaseSuccess, OnPurchaseSuccess);
        EventManager.Register(EventKey.VideoADRewarded, OnVideoAdRewarded);
    }

    protected override void OnUninit()
    {
        base.OnUninit();

        EventManager.Unregister(EventKey.PurchaseSuccess, OnPurchaseSuccess);
        EventManager.Unregister(EventKey.VideoADRewarded, OnVideoAdRewarded);

        Serialize();
        DataTool.Save();
    }

    #region 序列化 & 反序列化

    void Serialize()
    {
        DataTool.Serialize(RECORD_KEY, mData);
    }

    void Deserialize()
    {
        mData = DataTool.Deserialize<Dictionary<int, PropData>>(RECORD_KEY);
    }

    #endregion

    void RecordData()
    {
        Serialize();
    }

    public void AddProp(PropID pPropID, int pCount, PropSource pSource)
    {
        AddProp(new PropData(pPropID, pCount), pSource);
    }

    public void AddProp(PropData pPropData, PropSource pSource)
    {
        if (pPropData.Count <= 0) return;

        AddProps(new List<PropData> { pPropData }, pSource);
    }

    public void AddProps(int[] pPropIDs, int[] pPropCounts, PropSource pSource)
    {
        AddProps(GameMethod.ParseProps(pPropIDs, pPropCounts), pSource);
    }

    public void AddProps(List<PropData> pPropsData, PropSource pSource)
    {
        foreach (var tProp in pPropsData)
        {
            AddProp(tProp.ID, tProp.Count, false);
        }

        RecordData();

        TriggerGetRewards(pPropsData, pSource);
    }

    void AddProp(PropID pPropID, int pCount, bool pRecordData = true)
    {
        if (!mData.TryGetValue((int)pPropID, out var tPropData))
        {
            tPropData = new PropData(pPropID);
            mData.Add((int)pPropID, tPropData);
        }
        tPropData.AddCount(pCount);
        TriggerPropCountChange(pPropID, pCount, tPropData.Count);

        if (pRecordData)
        {
            RecordData();
        }
    }

    public bool ExpendProp(PropID pPropID, int pCount = 1, bool pRecordData = true)
    {
        if (mData.TryGetValue((int)pPropID, out var tPropData) && tPropData.Expend(pCount))
        {
            TriggerPropCountChange(pPropID, -pCount, tPropData.Count);

            if (pRecordData)
            {
                RecordData();
            }

            return true;
        }

        return false;
    }

    public int GetPropCount(PropID pPropID)
    {
        if (mData.TryGetValue((int)pPropID, out var tPropData))
        {
            return tPropData.Count;
        }

        return 0;
    }

    public bool HasProp(PropID pPropID)
    {
        return GetPropCount(pPropID) > 0;
    }

    void TriggerGetRewards(List<PropData> pPropsData, PropSource pSource)
    {
        var tEventData = EventManager.GetEventData<GetRewards>(EventKey.GetRewards);
        tEventData.rewards = pPropsData;
        tEventData.source = pSource;
        EventManager.Trigger(tEventData);
    }

    void TriggerPropCountChange(PropID pPropID, int pChangedCount, int pCurrentCount)
    {
        var tEventData = EventManager.GetEventData<PropCountChange>(EventKey.PropCountChange);
        tEventData.propID = pPropID;
        tEventData.changedCount = pChangedCount;
        tEventData.currentCount = pCurrentCount;
        EventManager.Trigger(tEventData);
    }

    void OnPurchaseSuccess(EventData pEventData)
    {
        var tEventData = pEventData as PurchaseSuccess;
        var tConfig = tEventData.productConfig;

        var tSource = PropSource.Shop;
        if (GameVariable.IngamePurchase)
        {
            GameVariable.IngamePurchase = false;
            tSource = PropSource.IngamePurchase;
        }

        AddProps(tConfig.propsID, tConfig.propsCount, tSource);
    }

    void OnVideoAdRewarded(EventData pEventData)
    {
        var tEventData = pEventData as ADEvent;
        PropID tPropID = DataConvert.GetADPropID(tEventData.showReason);

        if (tPropID != PropID.Invalid)
        {
            AddProp(tPropID, 1, PropSource.Rrewarded);
        }
    }

}