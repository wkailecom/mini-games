using AddressableTool;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

namespace HybridCLREditor
{
    public static class HotFixConfig
    {
        #region 临时保存 这些都在HybridCLR设置中添加了
        /// <summary>
        /// 需要在Prefab上挂脚本的热更dll名称列表，不需要挂到Prefab上的脚本可以不放在这里
        /// 但放在这里的dll即使勾选了 AnyPlatform 也会在打包过程中被排除
        /// 
        /// 另外请务必注意： 需要挂脚本的dll的名字最好别改，因为这个列表无法热更（上线后删除或添加某些非挂脚本dll没问题）。
        /// 
        /// 注意：多热更新dll不是必须的！大多数项目完全可以只有HotFix.dll这一个热更新模块,或者使用默认Assembly-CSharp.dll,
        /// 另外，是否热更新跟dll名毫无关系，凡是不打包到主工程的，都可以是热更新dll。
        /// </summary>
        public static List<string> MonoHotUpdateDllNames { get; } = new List<string>()
        {
            "HotFix.dll",
        };

        /// <summary>
        /// 所有热更新dll列表。放到此列表中的dll在打包时OnFilterAssemblies回调中被过滤。
        /// </summary>
        public static List<string> AllHotUpdateDllNames { get; } = MonoHotUpdateDllNames.Concat(new List<string>
        {
            // 这里放除了s_monoHotUpdateDllNames以外的脚本不需要挂到资源上的dll列表
            //"HotFix2.dll",
        }).ToList();

        public static List<string> AOTMetaDlls { get; } = new List<string>()
        {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll", // 如果使用了Linq，需要这个
        };
        #endregion

        public static string ProjectDir => Directory.GetParent(Application.dataPath).ToString();
        public static string IOSBuildPath => ProjectDir + "/iOSBuild";
        public static string IOSLibil2cppPath => IOSBuildPath + "/build/libil2cpp.a";
        public static string IOSBuildLibil2cppPath => IOSBuildPath + "/build_libil2cpp.sh";
        public static string HotFixDllsOutputDir => Application.dataPath + "/HybridCLRGenerate/HotFixDlls";
        public static string AotDllsOutputDir => HotFixDllsOutputDir + "/AotDlls";
        public static string HotDllsOutputDir => HotFixDllsOutputDir + "/HotDlls";

        public static string GetHotFixDllsOutputDirByTarget(BuildTarget target)
        {
            return $"{HotFixDllsOutputDir}/{target}";
        }


    }

    public static class HotFixBuild
    {

        [MenuItem("Build/HotFix相关/GenerateAll", priority = 200)]
        public static void GenerateAll()
        {
            PrebuildCommand.GenerateAll();
        }

        [MenuItem("Build/HotFix相关/CompileAndCopy_AOT&&HotUpdateDlls", priority = 200)]
        public static void CompileAndCopy_AOT_HotUpdateDlls()
        {
            CompileDllCommand.CompileDllActiveBuildTarget();
            CopyAOTAssembliesToTarget();
            CopyHotUpdateAssembliesToTarget();
            AssetDatabase.Refresh();
        }

        [MenuItem("Build/HotFix相关/CompileAndCopy_HotUpdateDlls", priority = 200)]
        public static void CompileAndCopy_HotUpdateDlls()
        {
            CompileDllCommand.CompileDllActiveBuildTarget();
            CopyHotUpdateAssembliesToTarget();
            AssetDatabase.Refresh();
        }


        /*
        * 版本底包打包流程(unity)
        * 首次GenerateAll,生成所有必须文件
        * 复制裁剪的dll(AOT)和热更dll到指定资源目录
        * 打包资源
        * 打包工程
        * 
        * 
        * 版本底包打包流程(导出工程)
        * 运行 HybridCLR/Generate/LinkXml
        * 导出工程
        * 运行 HybridCLR/Generate/Il2cppDef
        * 运行 HybridCLR/Generate/MethodBridge生成桥接函数
        * 运行 HybridCLR/Generate/PReverseInvokeWrapper。 不需要与lua之类交互的项目可跳过此步。
        * 将 {proj}\HybridCLRData\LocalIl2CppData-{platform}\il2cpp\libil2cpp\hybridclr\generated目录 替换导出工程中的此目录。
        * 在导出工程上执行build
        * 
        * 
        * 热更流程
        * 编译生成热更dll
        * 资源更新打包
        * 上传服务器
        */

        public static void Build()
        {
            if (!SettingsUtil.Enable) return;

            //var group = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            //PlayerSettings.SetApiCompatibilityLevel(group, ApiCompatibilityLevel.NET_4_6);
            PrebuildCommand.GenerateAll();
            Debug.Log("====> 复制dll到资源目录");
            CopyAOTAssembliesToTarget();
            CopyHotUpdateAssembliesToTarget();
            //PlayerSettings.SetApiCompatibilityLevel(group, ApiCompatibilityLevel.NET_Standard_2_0);
            AssetDatabase.Refresh();
        }

        [MenuItem("Build/构建代码热更", priority = 200)]
        public static void BuildHotUpdate()
        {
            //var group = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            //PlayerSettings.SetApiCompatibilityLevel(group, ApiCompatibilityLevel.NET_4_6);
            Debug.Log("====> 复制热更dll到工程目录");
            CompileAndCopy_HotUpdateDlls();
            //PlayerSettings.SetApiCompatibilityLevel(group, ApiCompatibilityLevel.NET_Standard_2_0);
            AssetDatabase.Refresh();
        }

        const string kHybridCLREditActive = "Build/HybridCLR Editor Active";

        [MenuItem(kHybridCLREditActive, false, 201)]
        public static void ToggleHybridActive()
        {
            SettingsUtil.Enable = !SettingsUtil.Enable;
        }

        [MenuItem(kHybridCLREditActive, true, 201)]
        public static bool ToggleHybridActiveValidate()
        {
            UnityEditor.Menu.SetChecked(kHybridCLREditActive, SettingsUtil.Enable);
            return true;
        }

        public static void CopyAOTAssembliesToTarget()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
            string aotAssembliesDstDir = HotFixConfig.AotDllsOutputDir;

            foreach (var dll in LoadDll.AOTMetaAssemblyNames)
            {
                string srcDllPath = $"{aotAssembliesSrcDir}/{dll}";
                if (!File.Exists(srcDllPath))
                {
                    Debug.LogError($"ab中添加AOT补充元数据dll:{srcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    continue;
                }
                string dllBytesPath = $"{aotAssembliesDstDir}/{dll}.bytes";
                File.Copy(srcDllPath, dllBytesPath, true);
                Debug.Log($"[CopyAOTAssembliesToTarget] copy AOT dll {srcDllPath} -> {dllBytesPath}");
            }
        }

        public static void CopyHotUpdateAssembliesToTarget()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;

            string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
            string hotfixAssembliesDstDir = HotFixConfig.HotDllsOutputDir;
            foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
            {
                string dllPath = $"{hotfixDllSrcDir}/{dll}";
                string dllBytesPath = $"{hotfixAssembliesDstDir}/{dll}.bytes";
                File.Copy(dllPath, dllBytesPath, true);
                Debug.Log($"[CopyHotUpdateAssembliesToTarget] copy hotfix dll {dllPath} -> {dllBytesPath}");
            }
        }




        #region 测试

        //[MenuItem("Build/HotFix测试/底包构建Win", priority = 200)]
        public static void BuildApp()
        {
            GenerateAll();
            Build_Win64();
            CompileAndCopy_AOT_HotUpdateDlls();
            ResBuild.Build();
            Build_Win64();
            Debug.Log("====> 底包构建完成");
        }

        // [MenuItem("Build/HotFix测试/底包构建APK", priority = 200)]
        public static void BuildAPK()
        {
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_4_6);
            GenerateAll();
            // BuildPlayer.BuildAndroidTest();//Build_Apk();
            Debug.Log("====> 第1次 Build App(为了生成补充AOT元数据dll)");
            BuildAndroidTest();
            Debug.Log("====> 复制AOT和热更dll到工程目录");
            CompileAndCopy_AOT_HotUpdateDlls();
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_Standard_2_0);
            Debug.Log("====> 打包资源");
            ResBuild.Build();
            Debug.Log("====> 第2次 Build App");
            BuildPlayer.ExportAPK();
            Debug.Log("====> 底包构建完成");
        }

        public static void Build_Win64()
        {
            BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
            BuildTarget activeTarget = EditorUserBuildSettings.activeBuildTarget;
            if (activeTarget != BuildTarget.StandaloneWindows64 && activeTarget != BuildTarget.StandaloneWindows)
            {
                Debug.LogError("请先切到Win平台再打包");
                return;
            }

            string outputPath = $"{HotFixConfig.ProjectDir}/Builds/Release-Win64/HybridCLRTrial.exe";

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new string[] { "Assets/Scenes/Entry.unity" };
            buildPlayerOptions.locationPathName = outputPath;
            buildPlayerOptions.options = BuildOptions.CompressWithLz4;
            buildPlayerOptions.target = buildTarget;
            buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;


            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.LogError("打包失败");
                return;
            }
        }
        public static void Build_Apk()
        {
            BuildTarget buildTarget = BuildTarget.Android;
            BuildTarget activeTarget = EditorUserBuildSettings.activeBuildTarget;
            if (activeTarget != BuildTarget.Android)
            {
                Debug.LogError("请先切到Android平台再打包");
                return;
            }

            string outputPath = $"{HotFixConfig.ProjectDir}/Builds/Release-Android/HybridCLRTrial.apk";

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new string[] { "Assets/Scenes/Entry.unity" };
            buildPlayerOptions.locationPathName = outputPath;
            buildPlayerOptions.options = BuildOptions.CompressWithLz4;
            buildPlayerOptions.target = buildTarget;
            buildPlayerOptions.targetGroup = BuildTargetGroup.Android;


            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.LogError("打包失败");
                return;
            }
        }

        public static void BuildAndroidTest()
        {
            BuildTarget target = BuildTarget.Android;
            BuildOptions buildOptions = BuildOptions.None;
            // Get filename.
            DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
            string appName = $"Woody_{Application.version}{".apk"}";
            string outpath = directory.Parent + "/Build/" + appName;// 导出路径   


            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
            {
                scenes = GetBuildScenes(),
                locationPathName = outpath,
                options = buildOptions,
                target = target,
                targetGroup = BuildTargetGroup.Android,
            };

            BuildPipeline.BuildPlayer(buildPlayerOptions);
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

        public static void CheckForUpdateContent()
        {
            //与上次打包做资源对比
            string buildPath = ContentUpdateScript.GetContentStateDataPath(false);//如果想弹出文件选择面板，将参数true
            Debug.Log("localBundelPath:" + buildPath);
            var Settings = AddressableAssetSettingsDefaultObject.Settings;
            List<AddressableAssetEntry> entrys = ContentUpdateScript.GatherModifiedEntries(Settings, buildPath);
            if (entrys.Count == 0) return;
            StringBuilder sbuider = new StringBuilder();
            sbuider.AppendLine("Need Update Assets:");
            foreach (var _ in entrys)
            {
                sbuider.AppendLine(_.address);
            }
            Debug.Log(sbuider.ToString());

            //将被修改过的资源单独分组
            var groupName = string.Format("UpdateGroup_{0}", DateTime.Now.ToString("yyyyMMdd"));
            ContentUpdateScript.CreateContentUpdateGroup(Settings, entrys, groupName);
            ContentUpdateScript.BuildContentUpdate(AddressableAssetSettingsDefaultObject.Settings, buildPath);
        }

    }

    #endregion

}