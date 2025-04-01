using AppsFlyerSDK;
using Game;
using LLFramework;
using System;
using System.Text;
using UnityEngine;

public class AppInfoManager : Singleton<AppInfoManager>
{
    public string AppIdentifier { get; private set; }
    public string AppName { get; private set; }
    public string AppVersion { get; private set; }
    public string ResVersion { get; private set; }
    public string FirstVersion { get; private set; }
    public string AppOS { get; private set; }
    public string AppBuildType { get; private set; }

    public string DeviceOS { get; private set; }
    public string DeviceModel { get; private set; }
    public string DeviceCountry { get; private set; }
    public string DeviceLanguage { get; private set; }

    public string UserID { get; private set; }
    public UserGroup UserGroup { get; private set; }
    public string UserGroupName { get; private set; }

    public string AppsflyerID => AppsFlyer.getAppsFlyerId();
    public string FirebaseUserID => string.Empty;
    public string IDFA => NativeUtil.GetIDFA();
    public string IDFV => NativeUtil.GetIDFV();
    public string MADID => NativeUtil.GetAdvertisingId();

    public bool IsDebug { get; private set; }
    public bool IsIOS { get; private set; }


    public void Init()
    {
        AppIdentifier = Application.identifier;
        AppName = Application.productName;
        AppVersion = GetAppVersion();
        ResVersion = GetResVersion();
        AppOS = GetAppOS();
        AppBuildType = GetBuildType();

        DeviceOS = SystemInfo.operatingSystem;
        DeviceModel = SystemInfo.deviceModel;
        DeviceCountry = GetCountry();
        DeviceLanguage = GetLanguage();

        UserID = GetDeviceId();
        UserGroup = GetUserGroup(UserID);
        UserGroupName = GetUserGroupName(UserGroup);

        IsDebug = GetIsDebug();
        IsIOS = GetIsIOS();

        FirstVersion = DataTool.GetString(DataKey.FIRST_VERSIOM_KEY, Application.version);
    }

    string GetAppVersion()
    {
        StringBuilder tStringBuilder = new StringBuilder("version_");

#if UNITY_ANDROID
        tStringBuilder.Append("android_");
#elif UNITY_IOS
        tStringBuilder.Append("ios_");
#endif
        tStringBuilder.Append(Application.version);
#if GM_MODE
        tStringBuilder.Append("_debug");
#endif

        return tStringBuilder.ToString();
    }

    string GetResVersion()
    {
        return ResVersion ??= AssetManager.Instance.LoadAsset<TextAsset>("ResVersion").text;
    }

    string GetAppOS()
    {
#if UNITY_ANDROID
        return "android";
#elif UNITY_IOS
        return "IOS";
#else
        return "Unknow";
#endif
    }

    string GetBuildType()
    {
        return GetIsDebug() ? "debug" : "release";
    }

    bool GetIsDebug()
    {
#if GM_MODE
        return true;
#else
        return false;
#endif
    }

    bool GetIsIOS()
    {
#if UNITY_IOS
        return true;
#else
        return false;
#endif
    }

    string GetCountry()
    {
        return System.Globalization.RegionInfo.CurrentRegion.ToString();
    }

    string GetLanguage()
    {
        return Application.systemLanguage.ToString();
    }

    string GetDeviceId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    UserGroup GetUserGroup(string pUserID)
    {
        if (string.IsNullOrEmpty(pUserID))
        {
            return UserGroup.GroupA;
        }
        return pUserID[0] % 2 == 0 ? UserGroup.GroupA : UserGroup.GroupB;
    }

    string GetUserGroupName(UserGroup pUserGroup)
    {
        return pUserGroup switch
        {
            UserGroup.GroupA => "U_A",
            UserGroup.GroupB => "U_B",
            UserGroup.GroupC => "U_C",
            UserGroup.GroupD => "U_D",
            _ => "U_A",
        };
    }


    public string GetCurDataPath() => GetDataPath(UserGroup);

    public string GetDataPath(UserGroup pUserGroup)
    {
        return pUserGroup switch
        {
            UserGroup.GroupA => GameConst.CONFIG_ROOT_PATH,
            UserGroup.GroupB => GameConst.CONFIG_ROOT_PATHB,
            UserGroup.GroupC => GameConst.CONFIG_ROOT_PATHC,
            UserGroup.GroupD => GameConst.CONFIG_ROOT_PATHD,
            _ => GameConst.CONFIG_ROOT_PATH,
        };
    }

    public void SwitchCountry(string pCountry)
    {
        DeviceCountry = pCountry;
    }

    public void SwitchUserGroup()
    {
        SetUserGroup(UserGroup == UserGroup.GroupA ? UserGroup.GroupB : UserGroup.GroupA);
        EventManager.Trigger(EventKey.SwitchUserGroup);
    }

    public void SetUserGroup(UserGroup pUserGroup)
    {
        UserGroup = pUserGroup;
        UserGroupName = GetUserGroupName(UserGroup);
    }
}

public enum UserGroup
{
    GroupA,
    GroupB,
    GroupC,
    GroupD,
}