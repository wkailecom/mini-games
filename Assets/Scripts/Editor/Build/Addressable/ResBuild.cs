using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
namespace AddressableTool
{
    public class ResBuild
    {
        public const string RULE_CONFIG_PATH = "Assets/Editor/GroupRulesConfig.asset";

        static AddressableAssetSettings AssetSettings => AddressableAssetSettingsDefaultObject.Settings;
        static List<AddressableGroupData> mGroupDatas = new List<AddressableGroupData>();

        [MenuItem("Build/AA相关/构建底包资源", priority = 300)]
        public static void Build()
        {
            //清理无效分组
            ClearGroupFile();
            SetStaticContentGroup();
            //构建新AA资源
            BuildNewContent();
            AssetDatabase.Refresh();
            Debug.Log("底包资源构建完成");
        }

        [MenuItem("Build/AA相关/构建热更资源", priority = 301)]
        public static void UpdateBuildResource()
        {
            BuildUpdate();
            AssetDatabase.Refresh();
            Debug.Log("热更资源构建完成 注意此构建不包含代码热更");
        }


        [MenuItem("Build/AA相关/按规则创建组", priority = 302)]
        public static void CreateGroupByRuleFile()
        {
            GetRules();

            if (mGroupDatas.Count <= 0) return;
            foreach (var tGroupData in mGroupDatas)
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(tGroupData.searchAsset, out string tGUID, out long tLocalID);
                CreateGroupByFolder(new string[] { tGUID }, tGroupData);
            }

            ClearGroupFile();
            SetStaticContentGroup();
            Debug.Log("完成");
        }

        [MenuItem("Build/AA相关/清理无效分组", priority = 303)]
        public static void ClearGroupFile()
        {
            var tGroups = AssetSettings.groups;
            var tRemoveGroup = new List<AddressableAssetGroup>();
            var tMissingGroupsIndex = new List<int>();
            for (int i = 0; i < tGroups.Count; i++)
            {
                var tGroup = tGroups[i];
                if (tGroup == null)
                {
                    tMissingGroupsIndex.Add(i);
                    continue;
                }

                if (tGroup.entries != null && tGroup.entries.Count <= 0)
                {
                    tRemoveGroup.Add(tGroup);
                }
            }
            //移除 Missing References
            if (tMissingGroupsIndex.Count > 0)
            {
                for (int i = tMissingGroupsIndex.Count - 1; i >= 0; i--)
                {
                    tGroups.RemoveAt(tMissingGroupsIndex[i]);
                }
            }
            //移除空分组
            foreach (var item in tRemoveGroup)
            {
                AssetSettings.RemoveGroup(item);
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 获取Rule文件
        /// </summary>
        static void GetRules()
        {
            if (!File.Exists(RULE_CONFIG_PATH))
            {
                Debug.Log($"{RULE_CONFIG_PATH} 目录不存在或未找到文件");
                return;
            }

            var tRulesConfig = AssetDatabase.LoadAssetAtPath<GroupRulesConfig>(RULE_CONFIG_PATH);
            if (tRulesConfig != null)
            {
                mGroupDatas.Clear();
                mGroupDatas.AddRange(tRulesConfig.groupData);
            }
        }

        /// <summary>
        /// 根据文件夹创建分组
        /// </summary>
        /// <param name="guids"></param>
        /// <param name="pSeparately">分开打bundle</param>
        public static void CreateGroupByFolder(string[] guids, AddressableGroupData pGroupData)
        {
            if (guids == null) return;

            for (int i = 0; i < guids.Length; i++)
            {
                var tFolderPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                DirectoryInfo folderInfo = new DirectoryInfo(tFolderPath);

                var tGroupName = !string.IsNullOrEmpty(pGroupData.groupName) ? pGroupData.groupName : folderInfo.Name;
                var tGroup = AddressableUtil.CreateGroup(tGroupName);
                var tSchema = tGroup.GetSchema<BundledAssetGroupSchema>();

                tSchema.BundleMode = pGroupData.groupType switch
                {
                    GroupType.withFileName => BundledAssetGroupSchema.BundlePackingMode.PackSeparately,
                    _ => tSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether,
                };

                if (pGroupData.searchOption == SearchOption.TopDirectoryOnly)
                {
                    var tDirs = folderInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
                    foreach (var tDir in tDirs)
                    {
                        if (tDir.Extension == ".meta") continue;

                        string tDirPath = CombinStr(tDir.FullName);
                        AddressableUtil.AddAssetToGroup(tGroup, tDirPath, null, true);
                    }
                }
                else
                {
                    var tFiles = folderInfo.GetFiles("*", SearchOption.AllDirectories);
                    foreach (var tFile in tFiles)
                    {
                        if (tFile.Extension == ".meta") continue;

                        string tFilePath = CombinStr(tFile.FullName);
                        AddressableUtil.AddAssetToGroup(tGroup, tFilePath, null, true);
                    }
                }
            }
        }

        static string CombinStr(string pFileFullPath)
        {
            string groupAssetPath = pFileFullPath.Replace(@"\", "/").Replace(Application.dataPath, "");
            string a = $"Assets{groupAssetPath}";
            return a;
        }

        public static void BuildNewContent()
        {
            AddressableAssetSettings.CleanPlayerContent(null);
            BuildCache.PurgeCache(false);
            AddressableAssetSettings.BuildPlayerContent();
        }

        public static void BuildUpdate()
        {
            //与上次打包做资源对比
            string buildPath = ContentUpdateScript.GetContentStateDataPath(false);
            List<AddressableAssetEntry> entrys = ContentUpdateScript.GatherModifiedEntries(AssetSettings, buildPath);
            if (entrys.Count == 0) return;

            //需要更新的资源
            StringBuilder sbuider = new StringBuilder();
            sbuider.AppendLine("Need Update Assets:");
            foreach (var _ in entrys)
            {
                sbuider.AppendLine(_.address);
            }
            Debug.Log(sbuider.ToString());

            //将被修改过的资源单独分组
            ContentUpdateScript.CreateContentUpdateGroup(AssetSettings, entrys, "UpdateGroup");
            AddressablesPlayerBuildResult result = ContentUpdateScript.BuildContentUpdate(AssetSettings, buildPath);
        }

        public static void SetAddressablesProfile(bool pIsRelease)
        {
            var tProfileName = pIsRelease ? "Release" : "Debug";
            var setting = AddressableAssetSettingsDefaultObject.Settings;
            setting.activeProfileId = setting.profileSettings.GetProfileId(tProfileName);
            AssetDatabase.Refresh();
            Debug.Log("aa资源地址:" + setting.activeProfileId);
        }

        public static void SetStaticContentGroup(bool pIsStatic = true)
        {
            var tBuildPath = pIsStatic ? AddressableAssetSettings.kLocalBuildPath : AddressableAssetSettings.kRemoteBuildPath;
            var tLoadPath = pIsStatic ? AddressableAssetSettings.kLocalLoadPath : AddressableAssetSettings.kRemoteLoadPath;
            foreach (var group in AssetSettings.groups)
            {
                if (group == null) continue;
                foreach (var schema in group.Schemas)
                {
                    if (schema is ContentUpdateGroupSchema updateGroupSchema)
                    {
                        updateGroupSchema.StaticContent = pIsStatic;
                    }
                    else if (schema is BundledAssetGroupSchema bundledAssetGroupSchema)
                    {
                        bundledAssetGroupSchema.BuildPath.SetVariableByName(group.Settings, tBuildPath);
                        bundledAssetGroupSchema.LoadPath.SetVariableByName(group.Settings, tLoadPath);
                    }
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        public static void ZipRemoteData()
        {
            //压缩文件
            var tSettings = AddressableAssetSettingsDefaultObject.Settings;
            string zipRootPath = tSettings.RemoteCatalogBuildPath.GetValue(tSettings);
            List<string> bundles = GameMethod.GetAllFileWithoutMetaInDic(zipRootPath);
            string zipPath = $"{zipRootPath}_{DateTime.Now:yyyyMMdd}.zip";
            ZipUtil.Zip(false, zipPath, bundles.ToArray());
        }
    }
}
#endif