using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 加载热更新Dll
/// </summary>
public class LoadDll : MonoBehaviour
{
    public static bool IsOpenHybridCLR = true;

    /// <summary>
    /// 泛型元数据补充
    /// </summary> 
    public static readonly List<string> AOTMetaAssemblyNames = new List<string>()
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll", // 如果使用了Linq，需要这个
        "UnityEngine.CoreModule.dll",
        
        "LLFramework.dll",
        "Newtonsoft.Json.dll",
        "Unity.Usercentrics.dll",
        
        //"DOTween.dll",
        //"Unity.Addressables.dll",
        //"Unity.ResourceManager.dll",
        //"UnityEngine.AndroidJNIModule.dll", 
        //"UnityEngine.Purchasing.dll",
        //"UnityEngine.UI.dll",
        
     };

    void Start()
    {
        //DontDestroyOnLoad(gameObject);
    }

    public void Init()
    {
        Debug.Log($"LoadDll.Init HybridCLR => {IsOpenHybridCLR}");
        if (IsOpenHybridCLR)
        {
            LoadGameDll();
        }
    }

    void LoadGameDll()
    {
        LoadMetadataForAOTAssembly();
#if  !UNITY_EDITOR
        TextAsset hotfixDll = Addressables.LoadAssetAsync<TextAsset>("GameAssembly.dll").WaitForCompletion();
        Assembly hotUpdateAss = System.Reflection.Assembly.Load(hotfixDll.bytes);
#else
        Assembly hotUpdateAss = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "GameAssembly");
#endif
    }


    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    private static void LoadMetadataForAOTAssembly()
    {
        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMetaAssemblyNames)
        {
            var tAotDllName = "AotDlls/" + aotDllName + ".bytes";
            var dllBytes = Addressables.LoadAssetAsync<TextAsset>(tAotDllName).WaitForCompletion();
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes.bytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
        }
    }

}