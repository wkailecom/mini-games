using AddressableTool;
using HybridCLREditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

#if UNITY_EDITOR
public class BuildPlayer
{
    const string BUNDLE_DEBUG_MODE_MENU_NAME = "Build/模式切换/Debug";
    const string BUNDLE_RELEASE_MODE_MENU_NAME = "Build/模式切换/Release";
    const string SYMBOLS_DEBUG = "DOTWEEN; GM_MODE";
    const string SYMBOLS_RELEASE = "DOTWEEN";

#if GM_MODE
    public const bool IsDebug = true;
#else
    public const bool IsDebug = false;
#endif

    [MenuItem(BUNDLE_DEBUG_MODE_MENU_NAME, IsDebug, priority = 2020)]
    public static void ToggleDebugMode()
    {
        BuildUtility.IsRelease = false;
        ResBuild.SetAddressablesProfile(false);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, SYMBOLS_DEBUG);
        AssetDatabase.Refresh();
    }

    [MenuItem(BUNDLE_RELEASE_MODE_MENU_NAME, !IsDebug, priority = 2020)]
    public static void ToggleReleaseMode()
    {
        BuildUtility.IsRelease = true;
        ResBuild.SetAddressablesProfile(true);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, SYMBOLS_RELEASE);
        AssetDatabase.Refresh();
    }

    [MenuItem("Build/Android/APK（Debug）", priority = 2030)]
    public static void ExportAPK()
    {
        Build(BuildTargetGroup.Android, BuildTarget.Android, false, true);
    }

    [MenuItem("Build/Android/AAB（Release）", priority = 2031)]
    public static void ExportAAB()
    {
        Build(BuildTargetGroup.Android, BuildTarget.Android, true, true);
    }

    [MenuItem("Build/iOS/IPA（Debug）", priority = 2040)]
    public static void ExportIosDebug()
    {
        Build(BuildTargetGroup.iOS, BuildTarget.iOS, false);
    }

    [MenuItem("Build/iOS/IPA（Release）", priority = 2041)]
    public static void ExportIosRelease()
    {
        Build(BuildTargetGroup.iOS, BuildTarget.iOS, true);
    }

    [MenuItem("Build/资源更新", priority = 2050)]
    public static void ExportHotAssets()
    {
        bool tIsRelease = BuildUtility.IsRelease;
        ResBuild.SetAddressablesProfile(tIsRelease);
        HotFixBuild.BuildHotUpdate();
        ResBuild.BuildUpdate();
        ResBuild.ZipRemoteData();
        AssetDatabase.Refresh();
        Debug.Log("热更更新构建完成，同步服务器即可");
    }

    public static void Build(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, bool isRelease, bool pIsAuto = true)
    {
        AssetDatabase.SaveAssets();
        //切换对应平台
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        PlayerSettings.SplashScreen.showUnityLogo = false;
        //宏定义修改
        string symbols = isRelease ? SYMBOLS_RELEASE : SYMBOLS_DEBUG;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
        //LunarConsole
        LunarConsoleEditorInternal.Installer.SetLunarConsoleEnabled(!isRelease);

        //热更代码构建
        HotFixBuild.Build();

        //ab资源构建
        ResBuild.Build();
        AssetDatabase.Refresh();

        string appName = string.Empty; //应用名称
        string locationPathName = string.Empty; //导出路径
        var rootDir = new DirectoryInfo(Application.dataPath).Parent; //项目根目录
        PlayerSettings.SplashScreen.showUnityLogo = false;
        if (buildTarget == BuildTarget.Android)
        {
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            EditorUserBuildSettings.buildAppBundle = isRelease;
            string tAppName = Application.productName.Replace(" ", "");
            if (pIsAuto)
            {
                appName = $"{tAppName}{(isRelease ? ".aab" : ".apk")}";
            }
            else
            {
                appName = $"{tAppName}_{Application.version}{(isRelease ? ".aab" : $"_{DateTime.Now:yyyyMMddHHmm}.apk")}";
            }
            locationPathName = rootDir + "/Build/Android/" + appName;
        }
        else if (buildTarget == BuildTarget.iOS)
        {
            EditorUserBuildSettings.iOSXcodeBuildConfig = isRelease ? XcodeBuildConfig.Release : XcodeBuildConfig.Debug;
            appName = "Crypto_IOS";
            locationPathName = rootDir + "/Build/iOS";
            CleanDirectory(locationPathName);
        }

        BuildPlayerOptions buildOptions = new BuildPlayerOptions();
        buildOptions.scenes = GetBuildScenes();
        buildOptions.locationPathName = locationPathName;
        buildOptions.target = buildTarget;
        buildOptions.options = BuildOptions.None;
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build {appName} successded :{report.summary.totalSize / (1024 * 1024)}M" + "\n" + report.summary.outputPath);
        }
        else if (report.summary.result == BuildResult.Failed)
        {
            Debug.LogError($"Build {appName} failed!!!");
        }
    }

    public static bool CheckPlatform(BuildTarget target)
    {
        if (EditorUserBuildSettings.activeBuildTarget != target)
        {
            EditorUtility.DisplayDialog("目标平台与当前平台不一致，请先进行平台转换",
                "当前平台：" + EditorUserBuildSettings.activeBuildTarget + "\n目标平台：" + target, "OK");
            return false;
        }
        return true;
    }

    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
                names.Add(e.path);
        }
        return names.ToArray();
    }

    static void CleanDirectory(string pPath)
    {
        if (Directory.Exists(pPath))
        {
            DirectoryInfo dir = new DirectoryInfo(pPath);
            if (dir != null)
            {
                foreach (var file in dir.GetFiles())
                {
                    file.Delete();
                }
                foreach (var subDir in dir.GetDirectories())
                {
                    subDir.Delete(true);
                }
            }
        }
    }

}
#endif