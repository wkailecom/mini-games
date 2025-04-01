using UnityEngine;
using System.Collections.Generic;
using LLFramework;

/**
* 没有返回值
* AndroidJavaObject jo = new AndroidJavaObject("android.content.res.Configuration");
* jo.Call("setToDefaults");
*
* 带有返回值
* AndroidJavaObject jo = new AndroidJavaObject("java.lang.String", "some string");
* int hash = jo.Call<int>("hashCode");
*
* 没有返回值的静态方法
* AndroidJavaObject jo = new AndroidJavaObject("android.os.Binder");
* jo.CallStatic("flushPendingCommands");
*
* 带有返回值的静态方法
* AndroidJavaObject jo = new AndroidJavaObject("java.lang.String");
* string valueString = jo.CallStatic<string>("valueOf", 42.0);
*
* 获取属性
* AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity")
**/

public class AdForAndroid : Singleton<AdForAndroid>
{
    private const string JAVA_CLASS_NAME = "com.nutz.game.ad.Mediation";
    private const string JAVA_CLASS_STATIC_FUNC_NAME = "getInstance";

#if UNITY_ANDROID

    #region 原生AndroidJavaClass

    public static void CallJavaFunc(string funcName, params object[] args)
    {
        Debug.Log("Call: " + funcName);
        try
        {
            // 获取类
            using (AndroidJavaClass jc = new AndroidJavaClass(JAVA_CLASS_NAME))
            {
                // 调用类静态方法
                using (AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>(JAVA_CLASS_STATIC_FUNC_NAME))
                {
                    // 调用类实例方法
                    jo.Call(funcName, args);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("callSdk error:" + ex.Message);
        }
    }

    public static int CallJavaFuncWithReturnValue(string funcName, params object[] args)
    {
        Debug.Log("Call: " + funcName);
        int returnValue = 0;
        try
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(JAVA_CLASS_NAME))
            {
                using (AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>(JAVA_CLASS_STATIC_FUNC_NAME))
                {
                    returnValue = jo.Call<int>(funcName, args);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("callSdk error:" + ex.Message);
        }
        return returnValue;
    }

    public static string CallJavaFuncWithReturnString(string funcName, params object[] args)
    {
        Debug.Log("Call: " + funcName);
        string returnValue = "";
        try
        {
            using (AndroidJavaClass jc = new AndroidJavaClass(JAVA_CLASS_NAME))
            {
                using (AndroidJavaObject jo = jc.CallStatic<AndroidJavaObject>(JAVA_CLASS_STATIC_FUNC_NAME))
                {
                    returnValue = jo.Call<string>(funcName, args);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("callSdk error:" + ex.Message);
        }
        return returnValue;
    }

    //------------------------------ plugin ------------------------------//
    private static AndroidJavaObject ConvertDictionaryToJavaHashMap(Dictionary<string, string> parameters)
    {
        var hashMap = new AndroidJavaObject("java.util.HashMap");
        var put = AndroidJNIHelper.GetMethodID(hashMap.GetRawClass(), "put",
                                               "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");

        foreach (var entry in parameters)
        {

            using (var key = new AndroidJavaObject("java.lang.String", entry.Key))
            {
                string val = entry.Value;
                if (string.IsNullOrEmpty(entry.Value)) val = "unknown";
                using (var value = new AndroidJavaObject("java.lang.String", val))
                {
                    AndroidJNI.CallObjectMethod(hashMap.GetRawObject(), put,
                                                AndroidJNIHelper.CreateJNIArgArray(new object[] { key, value }));
                }
            }
        }

        return hashMap;
    }

    //------------------------------ Notification ------------------------------//
    private const string JAVA_CLASS_NAME_NOTIFY = "com.nutz.game.message.notify.SFNotificationUtil";

    public static void SendNotification(double delayTime, int id, string title, string content)
    {
        Debug.Log("Call: " + "SendNotification");
        try
        {
            // 获取类
            using (AndroidJavaClass jc = new AndroidJavaClass(JAVA_CLASS_NAME_NOTIFY))
            {
                using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {

                    using (AndroidJavaObject cls_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        jc.CallStatic("sendNotification", cls_Activity, (long)delayTime, id, title, content);
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("callSdk error:" + ex.Message);
        }
    }

    public static void CancelNotification(int id)
    {
        Debug.Log("Call: " + "CancelNotification");
        try
        {
            // 获取类
            using (AndroidJavaClass jc = new AndroidJavaClass(JAVA_CLASS_NAME_NOTIFY))
            {
                jc.CallStatic("cancelSendMsgHandlerService", id);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("callSdk error:" + ex.Message);
        }
    }

    //------------------------------ Other ------------------------------//

    #endregion

    public bool IsInterstitialReady()
    {
        int ret = CallJavaFuncWithReturnValue("isInterstitialReady");
        return ret == 1;
    }

    public bool IsRewardVideoReady()
    {
        int ret = CallJavaFuncWithReturnValue("isRewardVideoReady");
        return ret == 1;
    }

    public bool IsBannerReady()
    {
        int ret = CallJavaFuncWithReturnValue("isBannerReady");
        return ret == 1;
    }

    public void ShowInterstitial()
    {
        CallJavaFunc("showInterstitial");
    }

    public void ShowRewardVideo()
    {
        CallJavaFunc("showRewardVideo");
    }

    public void ShowBanner()
    {
        CallJavaFunc("showBanner");
    }

    public void HideBanner()
    {
        CallJavaFunc("hideBanner");
    }

    public void ShowTestTool()
    {
        CallJavaFunc("showTestTool");
    }

    public void OpenAppStore(string url)
    {
        CallJavaFunc("openAppStore", url);
    }

    public string getInterstitialEcpm()
    {
        return CallJavaFuncWithReturnString("getCurrentInterstitialEcpm");
    }

    public string getRewardVideoEcpm()
    {
        return CallJavaFuncWithReturnString("getCurrentRewardVideoEcpm");
    }

    public string getBannerEcpm()
    {
        return CallJavaFuncWithReturnString("getCurrentBannerEcpm");
    }

    public string getCountry()
    {
        return CallJavaFuncWithReturnString("getCountry");
    }

    public string getLanguage()
    {
        return CallJavaFuncWithReturnString("getLanguage");
    }

    public string getAdvertisingId()
    {
        return CallJavaFuncWithReturnString("getAdvertisingId");
    }

    private bool IsAndroidPlayer()
    {
        return Application.platform == RuntimePlatform.Android;
    }

    public void Init(Dictionary<string, string> param = null)
    {
        if (!IsAndroidPlayer()) return;

        using (var hashMap = ConvertDictionaryToJavaHashMap(param))
        {
            CallJavaFunc("init", hashMap);
        }
    }

    public void InitAdLib(Dictionary<string, string> param = null, string afCallbackInfo = null)
    {
        if (!IsAndroidPlayer()) return;

        using (var hashMap = ConvertDictionaryToJavaHashMap(param))
        {
            CallJavaFunc("init", hashMap, afCallbackInfo);
        }
    }

    public void SendDataToPlatform(Dictionary<string, string> param = null)
    {
        if (!IsAndroidPlayer()) return;

        using (var hashMap = ConvertDictionaryToJavaHashMap(param))
        {
            CallJavaFunc("sendData", hashMap);
        }
    }



    public void ScheduleLocalNotification(double second, int notiID, string title, string content)
    {
        SendNotification(second, notiID, title, content);
    }

    public void CancelLocalNotification(int id)
    {
        CancelNotification(id);
    }

#endif


}
