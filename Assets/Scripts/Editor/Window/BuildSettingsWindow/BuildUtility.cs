using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildUtility
{
    //static string BUILD_CONFIG_KEY = "BUILD_CONFIG";
    //public static BuildConfig BuildConfig
    //{
    //    get => DataTool.Deserialize<BuildConfig>(BUILD_CONFIG_KEY);
    //    set => DataTool.Serialize(BUILD_CONFIG_KEY, value);
    //}

    public static bool IsRelease = false;
    public static bool IsBuildRes = true;
}

public enum BuildMode
{
    Debug,
    Release,
}

public enum BuildPlatform
{
    Android,
    IOS,
}

