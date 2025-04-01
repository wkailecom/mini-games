using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class WindowHelper
{
    private readonly static Dictionary<System.Type, string> _uxmlDic = new();

    static WindowHelper()
    {
        // 构建设置窗口
        _uxmlDic.Add(typeof(BuildSettingsWindow), "4a750449ce31ee74781ccc282243602a"); 


    }

    /// <summary>
    /// 加载窗口的布局文件
    /// </summary>
    public static UnityEngine.UIElements.VisualTreeAsset LoadWindowUXML<TWindow>() where TWindow : class
    {
        var windowType = typeof(TWindow);
        if (_uxmlDic.TryGetValue(windowType, out string uxmlGUID))
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(uxmlGUID);
            if (string.IsNullOrEmpty(assetPath))
                throw new System.Exception($"Invalid YooAsset uxml guid : {uxmlGUID}");
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.VisualTreeAsset>(assetPath);
            if (visualTreeAsset == null)
                throw new System.Exception($"Failed to load {windowType}.uxml");
            return visualTreeAsset;
        }
        else
        {
            throw new System.Exception($"Invalid YooAsset window type : {windowType}");
        }
    }


    /// <summary>
    /// 加载相关的配置文件
    /// </summary>
    public static TSetting LoadSettingData<TSetting>() where TSetting : ScriptableObject
    {
        var settingType = typeof(TSetting);
        var guids = AssetDatabase.FindAssets($"t:{settingType.Name}");
        if (guids.Length == 0)
        {
            Debug.LogWarning($"Create new {settingType.Name}.asset");
            var setting = ScriptableObject.CreateInstance<TSetting>();
            string filePath = $"Assets/{settingType.Name}.asset";
            AssetDatabase.CreateAsset(setting, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return setting;
        }
        else
        {
            if (guids.Length != 1)
            {
                foreach (var guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    Debug.LogWarning($"Found multiple file : {path}");
                }
                throw new System.Exception($"Found multiple {settingType.Name} files !");
            }

            string filePath = AssetDatabase.GUIDToAssetPath(guids[0]);
            var setting = AssetDatabase.LoadAssetAtPath<TSetting>(filePath);
            return setting;
        }
    }
}
#endif