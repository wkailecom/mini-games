using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;


public static class DataTool
{
    #region PlayerPrefs

    public static void SetString(string pKey, string pValue)
    {
        PlayerPrefs.SetString(pKey, pValue);
    }

    public static void SetInt(string pKey, int pValue)
    {
        PlayerPrefs.SetInt(pKey, pValue);
    }

    public static void SetBool(string pKey, bool pValue)
    {
        PlayerPrefs.SetInt(pKey, pValue ? 1 : 0);
    }

    public static string GetString(string pKey, string pDefaultValue = null)
    {
        return PlayerPrefs.GetString(pKey, pDefaultValue);
    }

    public static int GetInt(string pKey, int pDefaultValue = 0)
    {
        return PlayerPrefs.GetInt(pKey, pDefaultValue);
    }

    public static bool GetBool(string pKey, bool pValue = false)
    {
        var tValue = PlayerPrefs.GetInt(pKey, pValue ? 1 : 0);
        return tValue == 1;
    }

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public static void DeleteKey(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
        }
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }

    #endregion

    public static string DataToString<T>(T pData, Formatting pFormatting = Formatting.None)
    {
        return JsonConvert.SerializeObject(pData, pFormatting);
    }

    public static T StringToData<T>(string pDataString) where T : new()
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(pDataString) ?? new T();
        }
        catch (JsonException)
        {
            // JSON 解析错误，使用默认值或空对象
            return new T();
        }
    }

    public static void Serialize<T>(string pName, T pData, Formatting pFormatting = Formatting.None) where T : new()
    {
        string tSerializeString = DataToString(pData, pFormatting);
        PlayerPrefs.SetString(pName, tSerializeString);
    }

    public static T Deserialize<T>(string pName) where T : new()
    {
        string tDeserializeString = PlayerPrefs.GetString(pName);
        return StringToData<T>(tDeserializeString);
    }

    public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> pData, TKey pKey)
    {
        if (pData.TryGetValue(pKey, out TValue tValue))
        {
            return tValue;
        }
        return tValue;
    }

    public static void SetValue<TKey, TValue>(this IDictionary<TKey, TValue> pData, TKey pKey, TValue pValue)
    {
        if (pData.ContainsKey(pKey))
        {
            pData[pKey] = pValue;
        }
        else
        {
            pData.Add(pKey, pValue);
        }
    }

    public static int AddValue<TKey>(this IDictionary<TKey, int> pData, TKey pKey, int tValue = 1)
    {
        if (pData.ContainsKey(pKey))
        {
            pData[pKey] += tValue;
        }
        else
        {
            pData.Add(pKey, tValue);
        }
        return pData[pKey];
    }
}