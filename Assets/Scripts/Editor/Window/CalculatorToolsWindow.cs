using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using Game;

#if UNITY_EDITOR
public class CalculatorToolsWindow : EditorWindow
{

    [MenuItem("Tools/计算工具")]
    static void Init()
    {
        var showOnly = GetWindow<CalculatorToolsWindow>(true, "计算工具");
        //showOnly.minSize = new Vector2(800, 600);
        showOnly.Show();
    }

    UserGroup mUserGroup;

    private void OnGUI()
    {
        //*****************************检查关卡是否有效************************************
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("检查关卡配置"))
        {
            ReadDataConfig();
            IsValidLevelConfig();
        }
        mUserGroup = (UserGroup)EditorGUILayout.EnumPopup(mUserGroup, GUILayout.Width(150));


        EditorGUILayout.EndHorizontal();
        //*****************************   *************************************
    }


    void ReadDataConfig()
    {
        ConfigData.levelConfig.LoadData(AppInfoManager.Instance.GetDataPath(mUserGroup));
    }

    static void IsValidLevelConfig()
    {
        var originalTextMap = new Dictionary<string, int>();
        StringBuilder strBuiler = new StringBuilder();
        
        GUIUtility.systemCopyBuffer = strBuiler.ToString();
        Debug.LogError("完成并复制");
    }
}
#endif