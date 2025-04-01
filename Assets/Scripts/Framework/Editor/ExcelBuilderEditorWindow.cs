using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ExcelBuilderEditorWindow
{
    const string CONFIGS_ROOT_FOLDER_PATH = "Assets/_OriginalData/Excel";
    const string CONFIGS_ROOT_FOLDER_PATHB = "Assets/_OriginalData/ExcelB";
    const string CONFIGS_ROOT_FOLDER_PATHC = "Assets/_OriginalData/ExcelC";
    const string CONFIGS_ROOT_FOLDER_PATHD = "Assets/_OriginalData/ExcelD";
    static readonly string BUILDER_FOLDER_ROOT_PATH = Application.dataPath + "/../AutoBuilder/";
    static readonly string BUILDER_FOLDER_PATH = BUILDER_FOLDER_ROOT_PATH + "Data/";
    static readonly string BUILDER_FOLDER_PATHB = BUILDER_FOLDER_ROOT_PATH + "DataB/";
    static readonly string BUILDER_FOLDER_PATHC = BUILDER_FOLDER_ROOT_PATH + "DataC/";
    static readonly string BUILDER_FOLDER_PATHD = BUILDER_FOLDER_ROOT_PATH + "DataD/";
    public static event Action OnBuildExcelFinish;
    static string BuilderPath(int pGroupId)
    {
        return pGroupId switch
        {
            1 => BUILDER_FOLDER_PATH,
            2 => BUILDER_FOLDER_PATHB,
            3 => BUILDER_FOLDER_PATHC,
            4 => BUILDER_FOLDER_PATHD,
            _ => BUILDER_FOLDER_PATH,
        };
    }

    static string FolderPath(int pGroupId)
    {
        return pGroupId switch
        {
            1 => CONFIGS_ROOT_FOLDER_PATH,
            2 => CONFIGS_ROOT_FOLDER_PATHB,
            3 => CONFIGS_ROOT_FOLDER_PATHC,
            4 => CONFIGS_ROOT_FOLDER_PATHD,
            _ => CONFIGS_ROOT_FOLDER_PATH,
        };
    }

    public static bool CheckSelected(string pExcelPathOrExcelFolderPath)
    {
        string tAbsolutePath = GetAbsolutePath(pExcelPathOrExcelFolderPath);
        return IsExcelFile(tAbsolutePath) || IsFolderContainsExcel(tAbsolutePath);
    }

    static string GetAbsolutePath(string pRelativePath)
    {
        return Application.dataPath + "/../" + pRelativePath;
    }

    static bool IsExcelFile(string pPath)
    {
        if (!File.Exists(pPath))
        {
            return false;
        }

        string tExtension = Path.GetExtension(pPath);
        return tExtension.Equals(".xls") || tExtension.Equals(".xlsx");
    }

    static bool IsFolderContainsExcel(string pPath)
    {
        if (!Directory.Exists(pPath))
        {
            return false;
        }

        return Directory.GetFiles(pPath, "*.xls?", SearchOption.AllDirectories).Length > 0;
    }

    public static void BuildExcel(string pExcelPathOrExcelFolderPath, bool pIsAllSheet = false)
    {
        if (string.IsNullOrEmpty(pExcelPathOrExcelFolderPath))
        {
            return;
        }

        int tGroupId = 1;
        if (pExcelPathOrExcelFolderPath.Contains(CONFIGS_ROOT_FOLDER_PATHB))
        {
            tGroupId = 2;
        }
        else if (pExcelPathOrExcelFolderPath.Contains(CONFIGS_ROOT_FOLDER_PATHC))
        {
            tGroupId = 3;
        }
        else if (pExcelPathOrExcelFolderPath.Contains(CONFIGS_ROOT_FOLDER_PATHD))
        {
            tGroupId = 4;
        }

        bool tIsBuildAll = pExcelPathOrExcelFolderPath.Equals(FolderPath(tGroupId));
        var tBuilderPath = BuilderPath(tGroupId);
        var tAbsolutePath = GetAbsolutePath(pExcelPathOrExcelFolderPath);

#if UNITY_EDITOR_OSX
        MacBuildExcel(tBuilderPath, pIsAllSheet, tIsBuildAll ? null : tAbsolutePath);
#else
        WinBuildExcel(tBuilderPath, pIsAllSheet, tIsBuildAll ? null : tAbsolutePath);
#endif
        UnityEditor.AssetDatabase.Refresh();
    }

    public static void WinBuildExcel(string pBuildPath, bool pIsAllSheet = false, string pAbsolutePath = null)
    {
        Process tProcess = new Process();
        tProcess.StartInfo.FileName = pBuildPath + "WinBuild.bat";
        tProcess.StartInfo.Arguments = $"{(pIsAllSheet ? 1 : 0)} {pAbsolutePath}"; ;

        tProcess.Start();
        tProcess.WaitForExit();
        if (tProcess.ExitCode == 0) OnBuildExcelFinish?.Invoke();
        tProcess.Close();
    }

    public static void MacBuildExcel(string pBuildPath, bool pIsAllSheet = false, string pAbsolutePath = null)
    {
        Process tProcess = new Process();
        tProcess.StartInfo.FileName = "/bin/bash";
        tProcess.StartInfo.Arguments = $"MacBuild.sh {(pIsAllSheet ? 1 : 0)} {pAbsolutePath}";
        tProcess.StartInfo.WorkingDirectory = pBuildPath;
        tProcess.StartInfo.RedirectStandardError = true;
        tProcess.StartInfo.RedirectStandardOutput = true;
        tProcess.StartInfo.CreateNoWindow = true;
        tProcess.StartInfo.UseShellExecute = false;

        tProcess.Start();
        tProcess.BeginOutputReadLine();
        tProcess.OutputDataReceived += new DataReceivedEventHandler(OnProcessOutput);

        tProcess.BeginErrorReadLine();
        tProcess.ErrorDataReceived += new DataReceivedEventHandler(OnProcessError);

        tProcess.WaitForExit();
        if (tProcess.ExitCode == 0) OnBuildExcelFinish?.Invoke();
        tProcess.Close();
    }

    static void OnProcessOutput(object pSender, DataReceivedEventArgs pEventArgs)
    {
        if (!string.IsNullOrEmpty(pEventArgs.Data))
        {
            Debug.Log(pEventArgs.Data);
        }
    }

    static void OnProcessError(object sender, DataReceivedEventArgs pEventArgs)
    {
        if (!string.IsNullOrEmpty(pEventArgs.Data))
        {
            Debug.LogError(pEventArgs.Data);
        }
    }
}