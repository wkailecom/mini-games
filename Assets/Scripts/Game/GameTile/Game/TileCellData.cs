using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileCellData
{
    public int Floor;//层
    public int Index;//索引对应物品 0开始
    public int IconIndex;//索引对应物品 1开始
    public TileCellState State;
    public event Action OnStateChange;

    public TileCellData() { }

    public TileCellData(int pFloor, int pIndex, int pIconIndex = 0)
    {
        Floor = pFloor;
        Index = pIndex;
        IconIndex = pIconIndex;
        State = pIconIndex > 0 ? TileCellState.Show : TileCellState.Hide;
    }

    public TileCellData(TileCellData pTileCellData)
    {
        Floor = pTileCellData.Floor;
        Index = pTileCellData.Index;
        IconIndex = pTileCellData.IconIndex;
        State = pTileCellData.State;
    }

    public void SetState(TileCellState pState)
    {
        if (State == pState)
        {
            return;
        }

        State = pState;
        OnStateChange?.Invoke();
    }

    public void SetState(TileCellState pState, int pIconIndex)
    {
        if (State == pState && IconIndex == pIconIndex)
        {
            return;
        }

        if (pIconIndex > 0)
        {
            IconIndex = pIconIndex;
        }

        State = pState;
        OnStateChange?.Invoke();
    }

    public int GetPosX()
    {
        return Index % TileGameDefine.EACH_LAYER_COLUMNL_COUNT;
    }

    public int GetPosY()
    {
        return Index / TileGameDefine.EACH_LAYER_COLUMNL_COUNT;
    }

    public static int GetIndex(int tPosX, int tPosY)
    {
        return tPosX + tPosY * TileGameDefine.EACH_LAYER_COLUMNL_COUNT;
    }
}

public enum TileCellState
{
    Hide = 0,
    Show = 1,
    Freeze = 2,
}