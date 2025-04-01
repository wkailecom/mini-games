using System;
using System.Collections.Generic;
using System.Text;

public static class SafeConverter
{
    public static bool ToBool(this string pString)
    {
        if (!bool.TryParse(pString, out bool tResult))
        {
            LogManager.LogError($"SafeConverter.ToBool Failed! String: {pString}");
        }
        return tResult;
    }

    public static int ToInt(this string pString)
    {
        if (!int.TryParse(pString, out int tResult))
        {
            LogManager.LogError($"SafeConverter.ToInt Failed! String: {pString}");
        }
        return tResult;
    }

    public static long ToLong(this string pString)
    {
        if (!long.TryParse(pString, out long tResult))
        {
            LogManager.LogError($"SafeConverter.ToLong Failed! String: {pString}");
        }
        return tResult;
    }

    public static float ToFloat(this string pString)
    {
        if (!float.TryParse(pString, out float tResult))
        {
            LogManager.LogError($"SafeConverter ToFloat Failed! String: {pString}");
        }
        return tResult;
    }

    public static double ToDouble(this string pString)
    {
        if (!double.TryParse(pString, out double tResult))
        {
            LogManager.LogError($"SafeConverter ToDouble Failed! String: {pString}");
        }
        return tResult;
    }

    public static int[] ToArrInt(this string pString, char pSplit = ',')
    {
        if (string.IsNullOrEmpty(pString))
        {
            return new int[0];
        }

        string[] tSplitedStrings = pString.Split(pSplit);
        int[] tResult = new int[tSplitedStrings.Length];
        for (int i = 0; i < tResult.Length; i++)
        {
            tResult[i] = tSplitedStrings[i].ToInt();
        }
        return tResult;
    }

    public static List<int> ToListInt(this string pString, char pSplit = ',')
    {
        if (string.IsNullOrEmpty(pString))
        {
            return new List<int>();
        }

        string[] tSplitedStrings = pString.Split(pSplit);
        List<int> tResult = new List<int>(tSplitedStrings.Length);
        foreach (var tSplitedString in tSplitedStrings)
        {
            tResult.Add(tSplitedString.ToInt());
        }
        return tResult;
    }

    public static string ListToString<T>(this IEnumerable<T> pList, char pSplit = ',')
    {
        StringBuilder tResult = new StringBuilder();
        foreach (var tItem in pList)
        {
            if (tResult.Length > 0)
            {
                tResult.Append(pSplit);
            }

            tResult.Append(tItem.ToString());
        }
        return tResult.ToString();
    }

    // 格式：1,3,5,7-20,22
    public static List<int> StringWithRangeToListInt(this string pString, char pSplit = ',', char pRangeSplit = '-')
    {
        List<int> tResult = new List<int>();
        if (string.IsNullOrEmpty(pString))
        {
            return tResult;
        }

        string[] tSplitedStrings = pString.Split(pSplit);
        foreach (var tSplitedString in tSplitedStrings)
        {
            if (tSplitedString.Contains(pRangeSplit.ToString()))
            {
                var tSplitedRange = tSplitedString.Split(pRangeSplit);
                int tStart = tSplitedRange[0].ToInt();
                int tEnd = tSplitedRange[1].ToInt();

                for (int i = tStart; i <= tEnd; i++)
                {
                    tResult.Add(i);
                }
            }
            else
            {
                tResult.Add(tSplitedString.ToInt());
            }
        }

        return tResult;
    }

    public static int ToDayInt(this DateTime pDateTime)
    {
        return pDateTime.Year * 10000 + pDateTime.Month * 100 + pDateTime.Day;
    }

}