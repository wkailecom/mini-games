using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System.Data;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace AddressableTool
{
    public static class AddressableUtil
    {
        static AddressableAssetSettings Settings => AddressableAssetSettingsDefaultObject.Settings;
        public static AddressableAssetGroup CreateGroup(string groupName)
        {
            return CreateDefaultAssetGroup<SchemaType>(Settings, groupName);
        }

        public static AddressableAssetGroup CreateDefaultAssetGroup<T>(AddressableAssetSettings settings, string groupName)
        {
            AddressableAssetGroup group = settings.FindGroup(groupName);
            if (group == null)
            {
                group = settings.CreateGroup(groupName, false, false, false, GetTemplateGroup(settings, 1), typeof(T));
            }
            return group;
        }

        public static AddressableAssetGroup CreateUpdateAssetGroup<T>(AddressableAssetSettings settings, string groupName)
        {
            AddressableAssetGroup group = settings.FindGroup(groupName);
            if (group == null)
            {
                group = settings.CreateGroup(groupName, false, false, false, GetTemplateGroup(settings, 2), typeof(T));
                group.GetSchema<BundledAssetGroupSchema>().Compression = BundledAssetGroupSchema.BundleCompressionMode.LZMA;
            }
            return group;
        }

        private static List<AddressableAssetGroupSchema> GetTemplateGroup(AddressableAssetSettings settings, int templateIndex)
        {
            var tGroupObjs = settings.GroupTemplateObjects;
            var tGroupObj = tGroupObjs[templateIndex];
            var template = tGroupObj as AddressableAssetGroupTemplate;
            if (template == null)
            {
                Debug.LogError("未找到对应的模板");
                return null;
            }
            return template.SchemaObjects;
        }

        public static void AddAssetToGroup(AddressableAssetGroup pGroup, string assetPath, string labelName, bool simplifyName = false)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            var entry = Settings.CreateOrMoveEntry(guid, pGroup);
            if (simplifyName)
            {
                entry.address = GetPathWithoutExtension(assetPath);
            }
            else
            {
                entry.address = assetPath;
            }
            if (!string.IsNullOrEmpty(labelName))
                entry.SetLabel(labelName, true, true);
        }

        private static string GetPathWithoutExtension(string path)
        {
            if (path != null)
            {
                int length = path.LastIndexOf('.');
                if (length == -1)
                {
                    return path.Replace("Assets/GameRes/", "");
                }
                return path.Substring(0, length).Replace("Assets/GameRes/", "");
            }
            return null;
        }

        public static void MoveAssetToGroup(string assetPath, string groupName)
        {
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            AddressableAssetGroup group = Settings.FindGroup(groupName);
            Settings.CreateOrMoveEntry(guid, group);
        }
    }

}
