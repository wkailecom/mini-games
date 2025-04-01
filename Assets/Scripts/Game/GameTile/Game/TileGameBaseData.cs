using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileGameBaseData
{
    public void Init()
    {

    }
    public void Reset()
    {

    }

    #region 序列化 & 反序列化
    public string Serialize()
    {
        //StringBuilder tStringBuilder = new StringBuilder();

        string tSerializeString = "";
        return tSerializeString;
    }

    public void Deserialize(string pSerializedString)
    {
        Reset();
        if (string.IsNullOrEmpty(pSerializedString))
        {
            return;
        }

    }


    #endregion
}