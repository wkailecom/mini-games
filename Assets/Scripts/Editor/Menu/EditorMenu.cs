using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class EditorMenu
{
    [MenuItem("Tools/Clear PlayerData", priority = 30)]
    static void ClearPlayerData()
    {
        PlayerPrefs.DeleteAll();
        FileUtil.DeleteFileOrDirectory(Application.persistentDataPath);
    }

    #region Folder
    [MenuItem("Tools/Folder/Assets", priority = 100)]
    static void OpenAssetsFolder()
    {
        Process.Start(Application.dataPath);
    }

    [MenuItem("Tools/Folder/StreamingAssets", priority = 101)]
    static void OpenStreamingAssetsFolder()
    {
        Process.Start(Application.streamingAssetsPath);
    }

    [MenuItem("Tools/Folder/PersistentData", priority = 102)]
    static void OpenPersistentDataFolder()
    {
        Process.Start(Application.persistentDataPath);
    }

    [MenuItem("Tools/Folder/TemporaryCache", false, 103)]
    public static void OpenFolderTemporaryCachePath()
    {
        Process.Start(Application.temporaryCachePath);
    }
    #endregion

    #region AutoBuild
    [MenuItem("Assets/****** Build Selected Excel ******", priority = 31)]
    static void BuildSelectedExcel()
    {
        ExcelBuilderEditorWindow.BuildExcel(AssetDatabase.GetAssetPath(Selection.activeObject));
    }

    [MenuItem("Assets/****** Build Selected Excel ******", true)]
    static bool CheckBuildSelectedExcel()
    {
        if (Selection.activeObject == null)
        {
            return false;
        }

        return ExcelBuilderEditorWindow.CheckSelected(AssetDatabase.GetAssetPath(Selection.activeObject));
    }

    [MenuItem("Assets/****** Build Selected Excel All Sheet ******", priority = 30)]
    static void BuildSelectedExcelAllSheet()
    {
        ExcelBuilderEditorWindow.BuildExcel(AssetDatabase.GetAssetPath(Selection.activeObject), true);
        Debug.LogWarning("注意多个Sheet的结构要相同，没问题忽略此信息。");
    }

    [MenuItem("Assets/****** Build Selected Excel All Sheet ******", true)]
    static bool CheckBuildSelectedExcelAllSheet()
    {
        if (Selection.activeObject == null)
        {
            return false;
        }

        return ExcelBuilderEditorWindow.CheckSelected(AssetDatabase.GetAssetPath(Selection.activeObject));
    }

    #endregion

    #region Scene
    [MenuItem("Tools/Scene/LauncherGame #%L", priority = 400)]
    static void LauncherGame()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Launcher.unity");
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Tools/Scene/Select LauncherGame", priority = 400)]
    static void SelectScenes()
    {
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Scenes/Launcher.unity");
    }

    [MenuItem("Tools/Scene/Editor #%E", priority = 401)]
    static void EditorScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Editor.unity");
        EditorApplication.isPlaying = true;
    }

    #endregion
}