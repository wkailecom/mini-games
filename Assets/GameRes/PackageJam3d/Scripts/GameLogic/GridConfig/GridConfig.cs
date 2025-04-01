using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

[Serializable]
public class CellInfo
{
    public int index;
    public bool walkable;
}

[Serializable]
public class GridConfig 
{
    public CellInfo[] cellInfo;
    public int uniqueColorsCount;

    public string ToJson()
    {
        var jObject = new JObject
        {
            ["uniqueColorsCount"] = uniqueColorsCount,
        };
        var cellArray = new JArray();
        for (var i = 0; i < cellInfo.Length; i++)
        {
            var info = cellInfo[i];
            var cell = new JObject
            {
                ["index"] = info.index,
                ["walkable"] =  info.walkable,
            };
            cellArray.Add(cell);
        }
        jObject["cellInfo"] = cellArray;
        var jsonStr = jObject.ToString();
        return jsonStr;
    }
}