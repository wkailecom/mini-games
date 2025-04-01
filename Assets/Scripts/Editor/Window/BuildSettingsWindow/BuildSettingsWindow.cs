using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.AddressableAssets;
using System.Collections.Generic;

public static class VisualElementExtend
{
    public static void SetDisplay(this VisualElement visualElement, bool isDisplay)
    {
        visualElement.style.display = isDisplay ? DisplayStyle.Flex : DisplayStyle.None;
    }
}


public class BuildSettingsWindow : EditorWindow, IPostprocessBuildWithReport
{
    [MenuItem("Build/打包设置", priority = 100)]
    public static void ShowExample()
    {
        BuildSettingsWindow window = GetWindow<BuildSettingsWindow>("构建设置", true);
        window.minSize = new Vector2(400, 600);
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        // 加载布局文件
        var visualAsset = WindowHelper.LoadWindowUXML<BuildSettingsWindow>();
        if (visualAsset == null) return;

        visualAsset.CloneTree(root);

        //构建菜单
        var tMenusElement = new VisualElement();
        tMenusElement.AddToClassList("some-styled-menus-bg");

        var tAllResProfile = GetResProfile();
        var tBuildPlatform = new EnumField("构建平台", BuildPlatform.Android);
        var tBuildMode = new EnumField("构建模式", BuildMode.Debug);
        var tResBuildType = new PopupField<string>("资源类型", tAllResProfile, 1);
        var tAppVersion = new TextField("app版本");
        var tVersionCode = new IntegerField("VersionCode");
        var tResVersion = new IntegerField("热更版本");
        var tIsBuildRes = new Toggle("是否构建资源");
        var tIsHotAssets = new Toggle("是否热更资源");
        var tIsUploading = new Toggle("是否上传服务器");

        tMenusElement.Add(tBuildPlatform);
        tMenusElement.Add(tBuildMode);
        tMenusElement.Add(tResBuildType);
        tMenusElement.Add(tAppVersion);
        tMenusElement.Add(tVersionCode);
        tMenusElement.Add(tIsBuildRes);
        tMenusElement.Add(tIsHotAssets);
        tMenusElement.Add(tResVersion);
        tMenusElement.Add(tIsUploading);

        //构建按钮
        var tBuildBut = new Button();
        tBuildBut.AddToClassList("some-styled-build-but");

        root.Add(tMenusElement);
        root.Add(tBuildBut);


        //初始化值
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            tBuildPlatform.SetValueWithoutNotify(BuildPlatform.IOS);
            tVersionCode.label = "Build";
            tVersionCode.SetValueWithoutNotify(PlayerSettings.iOS.buildNumber.ToInt());
        }
        else
        {
            tBuildPlatform.SetValueWithoutNotify(BuildPlatform.Android);
            tVersionCode.label = "Version Code";
            tVersionCode.SetValueWithoutNotify(PlayerSettings.Android.bundleVersionCode);
        }

        tBuildMode.SetValueWithoutNotify(BuildPlayer.IsDebug ? BuildMode.Release : BuildMode.Debug);
        tAppVersion.SetValueWithoutNotify(PlayerSettings.bundleVersion);
        tIsBuildRes.SetValueWithoutNotify(true);
        tIsHotAssets.SetValueWithoutNotify(false);
        tIsUploading.SetValueWithoutNotify(!tIsHotAssets.value);
        tIsBuildRes.SetEnabled(!tIsHotAssets.value);
        tIsUploading.SetEnabled(tIsHotAssets.value);
        tResVersion.SetValueWithoutNotify(0);
        tResVersion.SetDisplay(tIsHotAssets.value);
        tBuildBut.text = !tIsHotAssets.value ? "打包构建" : "热更资源构建";

        tBuildPlatform.RegisterCallback<ChangeEvent<Enum>>((evt) =>
        {
            var isIos = (BuildPlatform)evt.newValue == BuildPlatform.IOS;
            tVersionCode.label = isIos ? "Build" : "Version Code";
            tVersionCode.SetValueWithoutNotify(isIos ? PlayerSettings.iOS.buildNumber.ToInt() : PlayerSettings.Android.bundleVersionCode);
        });
        tBuildMode.RegisterCallback<ChangeEvent<Enum>>((evt) =>
        {
            var isRelease = (BuildMode)evt.newValue == BuildMode.Release;
            var tProfileName = isRelease ? tAllResProfile[2] : tAllResProfile[1];
            tResBuildType.SetValueWithoutNotify(tProfileName);
            SettResProfile(tProfileName);
        });
        tResBuildType.RegisterCallback<ChangeEvent<string>>((evt) =>
        {
            SettResProfile(evt.newValue);
        });
        tIsHotAssets.RegisterCallback<ChangeEvent<bool>>((evt) =>
        {
            tResVersion.SetDisplay(evt.newValue);
            tAppVersion.SetEnabled(!evt.newValue);
            tVersionCode.SetEnabled(!evt.newValue);
            tIsBuildRes.SetEnabled(!evt.newValue);
            tIsUploading.SetValueWithoutNotify(true);
            tIsUploading.SetEnabled(evt.newValue);
            tIsUploading.SetValueWithoutNotify(!evt.newValue);
            tBuildBut.text = !evt.newValue ? "打包构建" : "热更构建";
        });


        tBuildBut.clicked += () =>
        {
            BuildUtility.IsRelease = (BuildMode)tBuildMode.value == BuildMode.Release;
            BuildUtility.IsBuildRes = tIsBuildRes.value;
            PlayerSettings.bundleVersion = tAppVersion.text;
            SettResProfile(tResBuildType.value);

            if ((BuildPlatform)tBuildPlatform.value == BuildPlatform.Android)
            {
                PlayerSettings.Android.bundleVersionCode = tVersionCode.value;
                BuildAPP(false, BuildUtility.IsRelease, tIsHotAssets.value, tResVersion.value);
            }
            else if ((BuildPlatform)tBuildPlatform.value == BuildPlatform.IOS)
            {
                PlayerSettings.iOS.buildNumber = tVersionCode.value.ToString();
                BuildAPP(true, BuildUtility.IsRelease, tIsHotAssets.value, tResVersion.value);
            }

        };

    }

    List<string> GetResProfile()
    {
        var setting = AddressableAssetSettingsDefaultObject.Settings;
        return setting.profileSettings.GetAllProfileNames();
    }

    void SettResProfile(string pProfileName)
    {
        var setting = AddressableAssetSettingsDefaultObject.Settings;
        setting.activeProfileId = setting.profileSettings.GetProfileId(pProfileName);
        AssetDatabase.Refresh();
    }

    void SetResVersion(int pResVersion)
    {
        var tResPath = Application.dataPath + "/HybridCLRGenerate/HotFixDlls/HotDlls/ResVersion.txt";
        File.WriteAllText(tResPath, pResVersion.ToString());
        AssetDatabase.Refresh();
    }

    void BuildAPP(bool pIsIos, bool pIsRelease, bool pIsHot, int pResVersion = 0)
    {
        SetResVersion(pIsHot ? pResVersion : 0);
        if (pIsHot)
        {
            BuildPlayer.ExportHotAssets();
            return;
        }

        if (pIsIos)
        {
            BuildPlayer.Build(BuildTargetGroup.iOS, BuildTarget.iOS, pIsRelease, false);
        }
        else
        {
            BuildPlayer.Build(BuildTargetGroup.Android, BuildTarget.Android, pIsRelease, false);
        }

    }

    #region 构建完成后
    public int callbackOrder => 2;
    public void OnPostprocessBuild(BuildReport report)
    {
        var outputPath = report.summary.outputPath;
        if (!string.IsNullOrEmpty(outputPath) && File.Exists(outputPath))
        {
            EditorUtility.RevealInFinder(outputPath);
        }
    }
    #endregion

}