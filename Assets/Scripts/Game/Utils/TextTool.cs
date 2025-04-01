using Config;
using System;

public static class TextTool
{
    //public static string GetText(string pTextID, params object[] pArgs)
    //{
    //    return LanguageManager.GetFormatText(pTextID, pArgs);
    //}

    public static string FormatText(string pFormat, params object[] pArgs)
    {
        if (pArgs == null || pArgs.Length == 0)
        {
            return pFormat;
        }

        return string.Format(pFormat, pArgs);
    }

    //public static string Progress(string pNumerator, string pDenominator) => GetText("Com_Progress", pNumerator, pDenominator); // {0}/{1}

    //public static string TimeCountdown(int pSeconds) => GetText("Com_TimeStyle", (pSeconds / 60).ToString("D2"), (pSeconds % 60).ToString("D2")); //{0}:{1}

    public static string GetPercent(string pValue) => $"{pValue}%";

    public static string Percent(float pValue) => ValueToText(pValue, ValueShowType.FloatToPercent2);

    public static string ValueToText(float pValue, ValueShowType pType)
    {
        switch (pType)
        {
            case ValueShowType.Int:
                return ((int)pValue).ToString();
            case ValueShowType.FloatToPercent2:
                return GetPercent(Math.Round(pValue * 100, 2).ToString());
            case ValueShowType.IntToPercent:
                return GetPercent(((int)pValue).ToString());
            case ValueShowType.Float2:
                return Math.Round(pValue, 2).ToString();
        }

        LogManager.LogError($"ValueToString.GetValueString: Invalid type: {pType}");
        return string.Empty;
    }

    public enum ValueShowType
    {
        Int,                // 整数
        FloatToPercent2,    // float转换为百分比，保留两位小数，例如：0.256789 => 25.68%
        IntToPercent,       // int转换为百分比，例如：25 => 25%
        Float2,             // float保留两位小数
    }
     
}