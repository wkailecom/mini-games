using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class AdForIOS
{

#if UNITY_IOS

    private const string PluginName = "__Internal";

    //******************* SDK Init && Util Start*******************//
    [DllImport(PluginName)]
    private static extern void CFInitAdLib(string param = null, string afCallbackInfo = null);

    //******************* RewardedVideo API *******************//
    [DllImport(PluginName)]
    private static extern bool CFIsRewardedVideoReady();
    [DllImport(PluginName)]
    private static extern void CFShowRewardedVideo();

    //******************* Interstitial API Start*******************//
    [DllImport(PluginName)]
    private static extern bool CFIsInterstitialReady();
    [DllImport(PluginName)]
    private static extern void CFShowInterstitial();

    //******************* Banner API Start*******************//
    [DllImport(PluginName)]
    private static extern bool CFIsBannerReady();
    [DllImport(PluginName)]
    private static extern void CFShowBanner();
    [DllImport(PluginName)]
    private static extern void CFHideBanner();

    //******************* Ecpm API Start*******************//
    [DllImport(PluginName)]
    private static extern string CFFetchvideoadecpm();
    [DllImport(PluginName)]
    private static extern string CFFetchinteradecpm();
    [DllImport(PluginName)]
    private static extern string CFFetchBannerAdecpm();
    [DllImport(PluginName)]
    private static extern double CFFetchvideoadecpm_double();
    [DllImport(PluginName)]
    private static extern double CFFetchinteradecpm_double();
    [DllImport(PluginName)]
    private static extern double CFFetchBannerAdecpm_double();

    //******************* Notify API Start*******************//
    [DllImport(PluginName)]
    private static extern bool CFPushNotificationState();
    [DllImport(PluginName)]
    private static extern int CFUIUserInterfaceStyle();
    [DllImport(PluginName)]
    private static extern void CFScheduleLocalNotificationWithID(string notiID, string title, string content, double second);
    [DllImport(PluginName)]
    private static extern void CFSetBadgeNumber(string count);
    [DllImport(PluginName)]
    private static extern void CFCancelLocalNotificationWithID(string notiID);

    //******************* Other API Start*******************//
    [DllImport(PluginName)]
    private static extern void showAdTest();
    [DllImport(PluginName)]
    private static extern string CFGetIdfa();
    [DllImport(PluginName)]
    private static extern string CFGetDeviceId();
    [DllImport(PluginName)]
    private static extern int CFGetAuthAdTraceState();
    [DllImport(PluginName)]
    private static extern void CFGetNotificationPermis();
    [DllImport(PluginName)]
    private static extern void ShakeIos();
    [DllImport(PluginName)]
    private static extern void CFUpdateAppInAppStore();




    /// <summary>
    /// Converts the parameters to json.
    /// </summary>
    private static string ConvertParameters(Dictionary<string, string> parameters)
    {
        if (parameters == null)
        {
            return null;
        }
        StringBuilder builder = new StringBuilder();
        builder.Append("{\n");
        if (builder == null)
        {
            return null;
        }
        var first = true;
        foreach (var pair in parameters)
        {
            if (!first)
            {
                builder.Append(',');
            }

            SerializeString(builder, pair.Key);
            builder.Append(":");
            SerializeString(builder, pair.Value);

            first = false;
        }

        builder.Append("}\n");
        return builder.ToString();
    }

    /// <summary>
    /// Serialize string to json string.
    /// </summary>
    private static void SerializeString(StringBuilder builder, string str)
    {
        builder.Append('\"');

        var charArray = str.ToCharArray();
        foreach (var c in charArray)
        {
            switch (c)
            {
                case '"':
                    builder.Append("\\\"");
                    break;
                case '\\':
                    builder.Append("\\\\");
                    break;
                case '\b':
                    builder.Append("\\b");
                    break;
                case '\f':
                    builder.Append("\\f");
                    break;
                case '\n':
                    builder.Append("\\n");
                    break;
                case '\r':
                    builder.Append("\\r");
                    break;
                case '\t':
                    builder.Append("\\t");
                    break;
                default:
                    var codepoint = System.Convert.ToInt32(c);
                    if ((codepoint >= 32) && (codepoint <= 126))
                    {
                        builder.Append(c);
                    }
                    else
                    {
                        builder.Append("\\u" + System.Convert.ToString(codepoint, 16).PadLeft(4, '0'));
                    }
                    break;
            }
        }

        builder.Append('\"');
    }


    //////////////////////////////////////////////
    public static void InitAdLib(Dictionary<string, string> param = null, string afCallbackInfo = null)
    {
        CFInitAdLib(ConvertParameters(param), afCallbackInfo);
    }

    public static void ShowTestTool()
    {
        showAdTest();
    }

    public static bool IsRewardVideoReady()
    {
        return CFIsRewardedVideoReady();
    }
    public static void ShowRewardVideo()
    {
        CFShowRewardedVideo();
    }

    public static bool IsInterstitialReady()
    {
        return CFIsInterstitialReady();
    }
    public static void ShowInterstitial()
    {
        CFShowInterstitial();
    }

    public static bool IsBannerReady()
    {
        return CFIsBannerReady();
    }
    public static void ShowBanner()
    {
        CFShowBanner();
    }
    public static void HideBanner()
    {
        CFHideBanner();
    }

    public static string GetRewardVideoEcpm()
    {
        return CFFetchvideoadecpm();
    }
    public static string GetInterstitialEcpm()
    {
        return CFFetchinteradecpm();
    }
    public static string GetBannerEcpm()
    {
        return CFFetchBannerAdecpm();
    }
    public static double GetRewardVideoEcpm_Double()
    {
        return CFFetchvideoadecpm_double();
    }
    public static double GetInterstitialEcpm__Double()
    {
        return CFFetchinteradecpm_double();
    }
    public static double GetBannerEcpm__Double()
    {
        return CFFetchBannerAdecpm_double();
    }

    public static bool GetPushNotificationState()
    {
        return CFPushNotificationState();
    }

    public static int GetUIUserInterfaceStyle()
    {
        return CFUIUserInterfaceStyle();
    }

    public static void ScheduleLocalNotification(double second, int notiID, string title, string content)
    {
        CFScheduleLocalNotificationWithID(notiID.ToString(), title, content, second);
    }

    public static void SetBadgeNumber(string count)
    {
        CFSetBadgeNumber(count);
    }

    public static void CancelLocalNotification(string notiID)
    {
        CFCancelLocalNotificationWithID(notiID);
    }


    public static void InitATT()
    {
        CFGetIdfa();
    }

    public static string GetDeviceId()
    {
        return CFGetDeviceId();
    }

    public static int GetATTState()
    {
        return CFGetAuthAdTraceState();
    }

    public static void Shake()
    {
        ShakeIos();
    }

    public static void OpenAppStore()
    {
        CFUpdateAppInAppStore();
    }

    public static void RequestNotify()
    {
        CFGetNotificationPermis();
    }


#endif
}
