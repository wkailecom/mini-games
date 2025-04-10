using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Config;
using Game;

public class MiniFavorModule : ModuleBase
{
    const string RECORD_KEY = "RecordMiniFavorInfo";

    Dictionary<int, int> mFavorData;
    Dictionary<int, int> mStateData;
    protected override void OnInit()
    {
        Deserialize();

        InitStateData();
    }

    void InitStateData()
    {
        mStateData = new Dictionary<int, int>();
        for (int i = 0; i < CommonDefine.miniShowSort.Length; i++)
        {
            mStateData.AddValue(CommonDefine.miniShowSort[i], CommonDefine.miniShowState[i]);
        }
    }

    public int GetTagState(int pId)
    {
        return mStateData.GetValue(pId);
    }

    public bool IsFavor(int pId)
    {
        return mFavorData.GetValue(pId) == 1;
    }

    public void SetFavor(int pId, bool pIsFavor)
    {
        mFavorData.SetValue(pId, pIsFavor ? 1 : 0);
        Serialize();
    }

    public List<int> GetShowSort()
    {
        var tReturn = new List<int>();
        var tReturn1 = new List<int>();
        foreach (var item in CommonDefine.miniShowSort)
        {
            if (IsFavor(item))
            {
                tReturn.Add(item);
            }
            else
            {
                tReturn1.Add(item);
            }
        }
        tReturn.AddRange(tReturn1);
        return tReturn;
    }

    #region 序列化

    void Serialize()
    {
        DataTool.Serialize(RECORD_KEY, mFavorData);
    }

    void Deserialize()
    {
        mFavorData = DataTool.Deserialize<Dictionary<int, int>>(RECORD_KEY);
    }

    #endregion 
}
