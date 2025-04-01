using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
//using Unity.Mathematics;
using UnityEngine;

public class FloorViewData
{
    public int level;
    public List<FloorData> floorsData;

    public void Init()
    {
        floorsData = new List<FloorData>();

        Reset();
    }
    public void Reset()
    {
        floorsData.Clear();
    }

    public void RefreshFloorsData()
    {
        for (int j = 0; j < floorsData.Count; j++)
        {
            for (int i = 0; i < floorsData[j].floorData.Count; i++)
            {
                var tCellData = floorsData[j].floorData[i];
                if (tCellData.State != TileCellState.Hide)
                {
                    if (IsFreeze(tCellData))
                    {
                        tCellData.SetState(TileCellState.Freeze);
                    }
                    else
                    {
                        tCellData.SetState(TileCellState.Show);
                    }
                }
            }
        }
    }

    public List<FloorData> GetFloorsData(int pLeveIndex)
    {
        //var tFloorsData = ModuleManager.TileGame.GetLevelData(pLeveIndex);
        //foreach (var tFloorData in tFloorsData)
        //{
        //    List<TileCellData> tCellDataList = new List<TileCellData>();
        //    foreach (var tCellData in tFloorData.floorData)
        //    {
        //        tCellDataList.Add(new TileCellData(tCellData));
        //    }

        //    FloorData floorData = new FloorData();
        //    floorData.floor = tFloorData.floor;
        //    floorData.floorData = tCellDataList;
        //    floorsData.Add(floorData);
        //}

        RefreshFloorsData();
        return floorsData;
    }

    //洗牌数据
    public void ShuffleFloorData()
    {
        foreach (var tFloorData in floorsData)
        {
            FloorDataShuffle(tFloorData.floorData);
        }
        RefreshFloorsData();
    }
    /// <summary>
    /// 是否冷却
    /// </summary>
    public bool IsFreeze(TileCellData pTileCellData)
    {
        int pFloor = pTileCellData.Floor;
        var tPosX = pTileCellData.GetPosX();
        var tPosY = pTileCellData.GetPosY();
        for (int i = pFloor + 1; i < floorsData.Count; i += 2)//遍历所有可能覆盖当前块的数据
        {
            var itemList = floorsData[i].floorData;
            TileCellData liftTopData;
            TileCellData rightTopData;
            TileCellData liftBottomData;
            TileCellData rightBottomData;

            if (pFloor % 2 != 0)//非偶数层，
            {
                liftTopData = GetTileCellData(itemList, tPosX, tPosY + 1);
                rightTopData = GetTileCellData(itemList, tPosX - 1, tPosY + 1);
                liftBottomData = GetTileCellData(itemList, tPosX, tPosY);
                rightBottomData = GetTileCellData(itemList, tPosX - 1, tPosY);
            }
            else
            {
                liftTopData = GetTileCellData(itemList, tPosX + 1, tPosY);
                rightTopData = GetTileCellData(itemList, tPosX, tPosY);
                liftBottomData = GetTileCellData(itemList, tPosX + 1, tPosY - 1);
                rightBottomData = GetTileCellData(itemList, tPosX, tPosY - 1);
            }

            if ((liftTopData != null && liftTopData.State != TileCellState.Hide)
                  || (rightTopData != null && rightTopData.State != TileCellState.Hide)
                  || (liftBottomData != null && liftBottomData.State != TileCellState.Hide)
                  || (rightBottomData != null && rightBottomData.State != TileCellState.Hide))
            {
                return true;
            }

            if ((i + 1) < floorsData.Count)
            {
                var itemList2 = floorsData[i + 1].floorData;
                if (itemList2 != null)
                {
                    var tData = GetTileCellData(itemList2, tPosX, tPosY);
                    if (tData != null && tData.State != TileCellState.Hide)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void FloorDataShuffle(List<TileCellData> pFloorData)
    {
        System.Random rng = new System.Random();
        int tIconIndex;
        TileCellState tCellState;
        //for (int i = 49; i >= 5; i--)
        //{
        //    int j = rng.Next(i + 1);
        //    tIconIndex = pFloorData[i].IconIndex;
        //    tCellState = pFloorData[i].State;
        //    pFloorData[i].SetState(pFloorData[j].State, pFloorData[j].IconIndex);
        //    pFloorData[j].SetState(tCellState, tIconIndex);
        //}

        //int4 tLimit = ModuleManager.TileGame.GetCurrentLimitData();
        //int x = (tLimit.y - tLimit.w) + 1;//横向多少列
        //int y = (tLimit.z - tLimit.x) + 1;//纵向多少行
        //int total = x * y;//总共多少个
        ////Debug.Log($"===== tLimit:{tLimit},x:{x},y:{y},total:{x * y}");

        //int tCount = pFloorData.Count - 1;
        //for (int i = tCount; i >= 0; i--)
        //{
        //    if (pFloorData[i].IconIndex <= 0) continue;
        //    int r = UnityEngine.Random.Range(0, total);
        //    int tx = r % x;
        //    int ty = r / y;
        //    int j = TileCellData.GetIndex((tLimit.w + tx), (tLimit.x + ty));
        //    tIconIndex = pFloorData[i].IconIndex;
        //    tCellState = pFloorData[i].State;
        //    pFloorData[i].SetState(pFloorData[j].State, pFloorData[j].IconIndex);
        //    pFloorData[j].SetState(tCellState, tIconIndex);
        //    //Debug.Log($"===== r:{r}, j:{j}");
        //}
    }

    TileCellData GetTileCellData(List<TileCellData> pTileCellsData, int pX, int pY)
    {
        if (pX < 0 || pX >= TileGameDefine.EACH_LAYER_COLUMNL_COUNT ||
            pY < 0 || pY >= TileGameDefine.EACH_LAYER_ROW_COUNT)
        {
            return null;
        }
        return pTileCellsData[TileCellData.GetIndex(pX, pY)];
    }

    #region 序列化 & 反序列化
    public string Serialize()
    {
        //StringBuilder tStringBuilder = new StringBuilder();

        string tSerializeString = JsonConvert.SerializeObject(floorsData);
        return tSerializeString;
    }

    public void Deserialize(string pSerializedString)
    {
        Reset();
        if (string.IsNullOrEmpty(pSerializedString))
        {
            return;
        }

        floorsData = JsonConvert.DeserializeObject<List<FloorData>>(pSerializedString);
    }

    #endregion
}

public class TileLevelData
{
    public int level;
    public List<FloorData> floorsData;
}

public class FloorData
{
    public int floor;
    public List<TileCellData> floorData;

    public FloorData() { }
    public FloorData(int pFloor)
    {
        floor = pFloor;
        floorData = new List<TileCellData>();
    }

};