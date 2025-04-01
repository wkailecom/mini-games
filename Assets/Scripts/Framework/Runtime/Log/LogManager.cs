using UnityEngine;
using System.Text;

public static class LogManager
{
    public static bool EnableLog = false;

    public static void Log(object pMessage, Color pColor = default, Object pContext = null)
    {
        if (!EnableLog)
        {
            return;
        }

        Debug.Log(GetColorString(pMessage.ToString(), pColor), pContext);
    }

    public static void LogWarning(object pMessage, Color pColor = default, Object pContext = null)
    {
        if (!EnableLog)
        {
            return;
        }

        Debug.LogWarning(GetColorString(pMessage.ToString(), pColor), pContext);
    }

    public static void LogError(object pMessage, Color pColor = default, Object pContext = null)
    {
        if (!EnableLog)
        {
            return;
        }

        Debug.LogError(GetColorString(pMessage.ToString(), pColor), pContext);
    }

    public static void LogErrorFormat(string pFormat, params object[] pArgs)
    {
        if (!EnableLog)
        {
            return;
        }

        Debug.LogErrorFormat(pFormat, pArgs);
    }

    static string GetColorString(string pString, Color pColor)
    {
        if (pColor == default)
        {
            pColor = Color.white;
        }
        var tColorCode = ColorUtility.ToHtmlStringRGB(pColor);
        return new StringBuilder("<color=#").Append(tColorCode).Append(">").Append(pString).Append("</color>").ToString();
    }
}