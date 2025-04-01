
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

public struct TileSlotData
{
    public int Floor;
    public int Index;
    public int IconIndex;
}


public class SlotViewData
{
    private Stack<TileSlotData> queueData;
    public List<TileSlotData> slotsData;

    public void Init()
    {
        queueData = new Stack<TileSlotData>();
        slotsData = new List<TileSlotData>();
        Reset();

        //EventManager.Register(EventDataType.TileCellClick, OnTileCellClick);
    }
    public void Reset()
    {
        queueData.Clear();
        slotsData.Clear();
    }

    void OnTileCellClick(EventData pEventData)
    {
       // var tEventData = pEventData as TileCellClick;

        if (!IsCanPlace()) return;

        //tEventData.cellData?.SetState(TileCellState.Hide);
    }


    public bool IsCanPlace()
    {
        return slotsData.Count < TileGameDefine.SLOT_MAX_COUNT;
    }

    public int AddSlotData(TileCellData pCellData)
    {
        if (!IsCanPlace()) return -1;

        int tInsertIndex = slotsData.Count;
        for (int i = slotsData.Count - 1; i >= 0; i--)
        {
            if (slotsData[i].IconIndex == pCellData.IconIndex)
            {
                tInsertIndex = i + 1;
                break;
            }
        }

        var tSlotData = new TileSlotData();
        tSlotData.Floor = pCellData.Floor;
        tSlotData.Index = pCellData.Index;
        tSlotData.IconIndex = pCellData.IconIndex;
        slotsData.Insert(tInsertIndex, tSlotData);
        queueData.Push(tSlotData);
        return tInsertIndex;
    }

    public (int Index, TileSlotData? SlotData) GetRecallIndex()
    {
        if (slotsData.Count <= 0 || queueData.Count <= 0)
        {
            return (-1, null);
        }
        var tInsertIndex = slotsData.LastIndexOf(queueData.Pop());
        return (tInsertIndex, slotsData[tInsertIndex]);
    }

    public bool RemoveSlotData(int pInsertIndex)
    {
        if (pInsertIndex < 0 || pInsertIndex >= slotsData.Count)
        {
            return false;
        }
        else
        {
            slotsData.RemoveAt(pInsertIndex);
            return true;
        }
    }

    public List<int> EraseSlotData()
    {
        var tSlotGroupData = new Dictionary<int, List<TileSlotData>>();
        foreach (var tData in slotsData)
        {
            int iconIndex = tData.IconIndex;
            List<TileSlotData> tSlotDataList;
            if (!tSlotGroupData.TryGetValue(iconIndex, out tSlotDataList))
            {
                tSlotDataList = new List<TileSlotData>();
                tSlotGroupData[iconIndex] = tSlotDataList;
            }
            // 将新牌添加到相应的集合中
            tSlotDataList.Add(tData);
        }

        int tCurIndex = 0;
        List<int> tEraseIndexList = new List<int>();
        foreach (var tData in tSlotGroupData)
        {
            if (tData.Value.Count >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    slotsData.Remove(tData.Value[i]);
                    tEraseIndexList.Add(tCurIndex + i);
                }
            }
            tCurIndex += tData.Value.Count;
        }

        //存在消除则清空顺序队列
        if (tEraseIndexList.Count != 0)
        {
            queueData.Clear();
        }
        return tEraseIndexList;
    }

    public void ClearQueueData()
    {
        queueData.Clear();
    }


    #region 序列化 & 反序列化
    public string Serialize()
    {
        StringBuilder tStringBuilder = new StringBuilder();
        string tQueueDataStr = JsonConvert.SerializeObject(queueData);
        string tSlotsDataStr = JsonConvert.SerializeObject(slotsData);

        tStringBuilder.Append(tQueueDataStr).Append('&').Append(tSlotsDataStr);

        return tStringBuilder.ToString();
    }

    public void Deserialize(string pSerializedString)
    {
        Reset();
        if (string.IsNullOrEmpty(pSerializedString))
        {
            return;
        }

        var tSplitedStr = pSerializedString.Split('&');
        if (tSplitedStr.Length < 2)
        {
            LogManager.LogError($"SlotViewData.Deserialize: Invalid string ");
            return;
        }

        queueData = JsonConvert.DeserializeObject<Stack<TileSlotData>>(tSplitedStr[0]);
        slotsData = JsonConvert.DeserializeObject<List<TileSlotData>>(tSplitedStr[1]);
    }

    #endregion
}