using System.Collections.Generic;
using UnityEngine;

public class NativeUtil
{
    public static Dictionary<string, string> ADInfo = new Dictionary<string, string>
    {
        { "AppKey","hBXivm5wUowe1VgTGK0ycdpVQzliyRmIlKTiMt2o4ST8zfgSEgqDDayTVadc1Cvrf8UlImzRuBDQcSahO8e7Tx"},
        { "BannerUnitId","9b43a5ad6191b2ce"},
        { "InterstitialUnitId","13d10a8525859bf7"},
        { "RewardVideoUnitId","7010683de5d8169d"}
    };

    #region Init
    public static void InitAdlib(Dictionary<string, string> adInfo = null, string afCallbackInfo = null)
    {
#if UNITY_EDITOR

#elif UNITY_IOS
        Debug.Log("InitAD ios");
        AdForIOS.InitAdLib(ADInfo, afCallbackInfo);
#elif UNITY_ANDROID
        //AdForAndroid.Instance.InitAdLib(ADInfo, afCallbackInfo);
        //AdForAndroid.Instance.Init(adInfo);
#endif
    }
    #endregion

    #region Interstitial
    public static bool IsInterstitialReady()
    {
#if UNITY_EDITOR
        return true;
#elif UNITY_IOS
        return AdForIOS.IsInterstitialReady();
#elif UNITY_ANDROID
        return AdForAndroid.Instance.IsInterstitialReady();
#else
        return false;
#endif
    }

    public static void ShowInterstitial()
    {
#if UNITY_EDITOR

#elif UNITY_IOS
        AdForIOS.ShowInterstitial();
#elif UNITY_ANDROID
        AdForAndroid.Instance.ShowInterstitial();
#else
        
#endif
    }

    public static string GetInterstitialEcpm()
    {
#if UNITY_EDITOR
        return "0.0";
#elif UNITY_IOS
        return AdForIOS.GetInterstitialEcpm();
#elif UNITY_ANDROID
        return AdForAndroid.Instance.getInterstitialEcpm();
#endif
    }
    #endregion

    #region Reward Video
    public static bool IsRewardVideoReady()
    {
#if UNITY_EDITOR
        return true;
#elif UNITY_IOS
        return AdForIOS.IsRewardVideoReady();
#elif UNITY_ANDROID
        return AdForAndroid.Instance.IsRewardVideoReady();
#else
        return false;
#endif
    }

    public static void ShowRewardVideo()
    {
#if UNITY_EDITOR

#elif UNITY_IOS
        AdForIOS.ShowRewardVideo();
#elif UNITY_ANDROID
        AdForAndroid.Instance.ShowRewardVideo();
#else
        
#endif
    }

    public static string GetRewardVideoEcpm()
    {
#if UNITY_EDITOR
        return "0.0";
#elif UNITY_IOS
        return AdForIOS.GetRewardVideoEcpm();
#elif UNITY_ANDROID
        return AdForAndroid.Instance.getRewardVideoEcpm();
#endif
    }
    #endregion

    #region Banner
    public static bool IsBannerReady()
    {
#if UNITY_EDITOR
        return true;
#elif UNITY_IOS
        return AdForIOS.IsBannerReady();
#elif UNITY_ANDROID
        return AdForAndroid.Instance.IsBannerReady();
#else
        return false;
#endif
    }

    public static void ShowBanner()
    {
#if UNITY_EDITOR

#elif UNITY_IOS
        AdForIOS.ShowBanner();
#elif UNITY_ANDROID
        AdForAndroid.Instance.ShowBanner();
#else
        
#endif
    }

    public static void HideBanner()
    {
#if UNITY_EDITOR

#elif UNITY_IOS
        AdForIOS.HideBanner();
#elif UNITY_ANDROID
        AdForAndroid.Instance.HideBanner();
#else
        
#endif
    }
    public static string GetBannerEcpm()
    {
#if UNITY_EDITOR
        return "0.0";
#elif UNITY_IOS
        return AdForIOS.GetBannerEcpm();
#elif UNITY_ANDROID
        return AdForAndroid.Instance.getBannerEcpm();
#endif
    }
    #endregion

    #region Notification
    public static bool GetPushNotificationState()
    {
#if UNITY_EDITOR
        return true;
#elif UNITY_IOS        
        return AdForIOS.GetPushNotificationState();
#elif UNITY_ANDROID
        return true;
#else
        return false;
#endif
    }

    public static int GetUIUserInterfaceStyle()
    {
#if UNITY_EDITOR
        return 0;
#elif UNITY_IOS
        return AdForIOS.GetUIUserInterfaceStyle();//0 淺色, 1深色
#elif UNITY_ANDROID
        return 0;
#else
        return 0;
#endif
    }

    public static void CancelLocalNotification(int id)
    {
#if UNITY_EDITOR

#elif UNITY_IOS
        AdForIOS.CancelLocalNotification(id.ToString());
#elif UNITY_ANDROID
        AdForAndroid.Instance.CancelLocalNotification(id);
#else

#endif
    }

    public static void ScheduleLocalNotification(double second, int notiID, string title, string content)
    {
#if UNITY_EDITOR

#elif UNITY_IOS
        AdForIOS.ScheduleLocalNotification(second, notiID, title, content);
#elif UNITY_ANDROID
        AdForAndroid.Instance.ScheduleLocalNotification(second, notiID, title, content);
#else

#endif
    }

    public static void SetBadgeNumber(int count)
    {
#if UNITY_EDITOR

#elif UNITY_IOS
        AdForIOS.SetBadgeNumber(count.ToString());
#elif UNITY_ANDROID

#else

#endif
    }


    #endregion

    #region Others

    public static void ShowTestTool()
    {
#if UNITY_IOS
        AdForIOS.ShowTestTool();
#elif UNITY_ANDROID
        AdForAndroid.Instance.ShowTestTool();
#endif
    }

    public static string GetCountry()
    {
#if UNITY_EDITOR
        return System.Globalization.RegionInfo.CurrentRegion.ToString();
#elif UNITY_IOS
        return System.Globalization.RegionInfo.CurrentRegion.ToString(); 
#elif UNITY_ANDROID
        return AdForAndroid.Instance.getCountry();
#endif
    }

    public static string GetLanguage()
    {
#if UNITY_EDITOR
        return Application.systemLanguage.ToString();
#elif UNITY_IOS
        return Application.systemLanguage.ToString();
#elif UNITY_ANDROID
        return AdForAndroid.Instance.getLanguage();
#endif
    }

    public static string GetAdvertisingId()
    {
#if UNITY_EDITOR
        return "";
#elif UNITY_IOS
        return "";
#elif UNITY_ANDROID
        return AdForAndroid.Instance.getAdvertisingId();
#endif
    }

    /*
     * Warning: 
     * 1、ios14 以上系统，调用之前 未弹过授权弹窗，则调用会触发授权弹窗
     * 2、在用户拒绝授权授权的状态下，调用该函数 返回的结果是 全0 无效值字符串
     */
    public static string GetIDFA()
    {
#if UNITY_IOS
        //UnityEngine.iOS.Device.advertisingTrackingEnabled
        if (GetATTState() == 3)
        {
            return UnityEngine.iOS.Device.advertisingIdentifier;
        }
        else
        {
            return "00000000-0000-0000-0000-000000000000";
        }
#else
        return string.Empty;
#endif
    }

    public static string GetIDFV()
    {
#if UNITY_IOS
        return UnityEngine.iOS.Device.vendorIdentifier;
#else 
        return "";
#endif
    }

    public static string GetDeviceId()
    {
#if UNITY_EDITOR
        return SystemInfo.deviceUniqueIdentifier;
#elif UNITY_IOS
        return AdForIOS.GetDeviceId();
#elif UNITY_ANDROID
        return SystemInfo.deviceUniqueIdentifier;
#else
        return "0";
#endif
    }

    public static void InitATT()
    {
#if UNITY_EDITOR

#elif UNITY_IOS
        Debug.Log("InitATT ios");
        AdForIOS.InitATT();
#elif UNITY_ANDROID

#endif
    }

    /*
     * 0 用户未做选择
     * 1 系统层级关闭 == 用户拒绝
     * 2 用户拒绝
     * 3 用户同意 
     */
    public static int GetATTState()
    {
#if UNITY_EDITOR
        return 0;
#elif UNITY_IOS
        return AdForIOS.GetATTState(); 
#elif UNITY_ANDROID
        return 0;
#else
        return 0;
#endif
    }

    public static void RequestNotify()
    {
#if UNITY_EDITOR

#elif UNITY_IOS
         Debug.Log("RequestNotify ori ios");
         AdForIOS.RequestNotify();
#elif UNITY_ANDROID

#endif
    }

    public static void Shake()
    {
#if UNITY_EDITOR

#elif UNITY_IOS
        Debug.Log("AdForIOS, Shake");
        AdForIOS.Shake();
#elif UNITY_ANDROID

#endif
    }

    public static void OpenAppStore(string url)
    {
#if UNITY_EDITOR

#elif UNITY_IOS
        AdForIOS.OpenAppStore();
#elif UNITY_ANDROID
        AdForAndroid.Instance.OpenAppStore(url);
#endif
    }
    #endregion

}
