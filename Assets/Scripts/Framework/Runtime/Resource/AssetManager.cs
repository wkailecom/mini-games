using LLFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets; 
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement; 
using Object = UnityEngine.Object;

public class AssetHandle
{
    public AsyncOperationHandle Handle { get; private set; }
    public uint Count;

    public AssetHandle(AsyncOperationHandle pHandle)
    {
        Handle = pHandle;
        Count = 1;
    }
}

/// <summary>
/// 资源管理器，用于加载和管理资源
/// </summary>
public class AssetManager : BaseManager<AssetManager>
{
    private readonly Dictionary<string, AssetHandle> _assetDic = new();
    private readonly object lockObject = new();

    /// <summary>
    /// 同步加载资源
    /// </summary>
    public T LoadAsset<T>(string pAssetPath) where T : Object
    {
        var tHandle = GetOrCreateHandle<T>(pAssetPath);
        tHandle.WaitForCompletion();

        if (tHandle.Status == AsyncOperationStatus.Succeeded)
        {
            return tHandle.Result;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    public void LoadAssetAsync<T>(string pAssetPath, UnityAction<T> pCallback) where T : Object
    {
        var tHandle = GetOrCreateHandle<T>(pAssetPath);

        if (tHandle.IsDone)
        {
            pCallback(tHandle.Result);
        }
        else
        {
            tHandle.Completed += h =>
            {
                if (h.Status == AsyncOperationStatus.Succeeded)
                {
                    pCallback(h.Result);
                }
                else
                {
                    LogManager.LogError($"异步加载资源失败: {pAssetPath}");
                }
            };
        }
    }

    private AsyncOperationHandle<T> GetOrCreateHandle<T>(string pAssetPath) where T : Object
    {
        string tKey = GenerateKey(pAssetPath, typeof(T).Name);
        lock (lockObject)
        {
            if (_assetDic.ContainsKey(tKey))
            {
                var tInfo = _assetDic[tKey];
                tInfo.Count++;
                return tInfo.Handle.Convert<T>();
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<T>(pAssetPath);
                if (handle.IsValid())
                {
                    _assetDic.Add(tKey, new AssetHandle(handle));
                }
                else
                {
                    Debug.LogError($"资源加载失败: {pAssetPath}");
                }
                return handle;
            }
        }
    }

    private string GenerateKey(string pAssetPath, string typeName)
    {
        var keyBuilder = new StringBuilder();
        keyBuilder.Append(pAssetPath);
        keyBuilder.Append("_");
        keyBuilder.Append(typeName);
        return keyBuilder.ToString();
    }

    /// <summary>
    /// 释放已加载的资源
    /// </summary>
    public void ReleaseAsset<T>(string pAssetPath) where T : Object
    {
        string tKey = GenerateKey(pAssetPath, typeof(T).Name);
        lock (lockObject)
        {
            if (_assetDic.ContainsKey(tKey))
            {
                var info = _assetDic[tKey];
                info.Count--;
                if (info.Count == 0)
                {
                    Addressables.Release(info.Handle);
                    _assetDic.Remove(tKey);
                }
            }
            else
            {
                LogManager.LogWarning($"尝试释放不存在的资源: {pAssetPath}");
            }
        }
    }


    public GameObject LoadPrefab(string pPrefabPath, Transform pParent)
    {
        var tAsset = LoadAsset<GameObject>(pPrefabPath);
        if (tAsset == null)
        {
            LogManager.LogError("LoadPrefab Fail! Path : " + pPrefabPath);
            return null;
        }

        return Object.Instantiate(tAsset, pParent);
    }

    IEnumerator LoadSceneAsyncCoroutine(string pSceneName, LoadSceneMode pLoadSceneMode, UnityAction<bool> pCallback)
    {
        var tHandle = Addressables.LoadSceneAsync(pSceneName, pLoadSceneMode, false);
        yield return tHandle;
        if (tHandle.Status == AsyncOperationStatus.Succeeded)
        {
            AsyncOperation activateOperation = tHandle.Result.ActivateAsync();
            yield return activateOperation;  // 等待场景激活完成
            pCallback?.Invoke(true);
        }
        else
        {
            // 如果场景加载失败
            pCallback?.Invoke(false);
        }
    }

    /// <summary>
    /// 异步加载场景
    /// </summary> 
    public void LoadSceneAsync(string pSceneName, LoadSceneMode pLoadSceneMode, UnityAction<bool> pCallback)
    {
        TaskManager.Instance.StartTask(LoadSceneAsyncCoroutine(pSceneName, pLoadSceneMode, pCallback));
    }

    /// <summary>
    /// 异步加载场景
    /// </summary> 
    public void LoadSceneAsync(string pSceneName, LoadSceneMode pLoadSceneMode, UnityAction pCallback)
    {
        SceneManager.LoadSceneAsync(pSceneName, LoadSceneMode.Additive).completed += (asyncOperation) =>
        {
            pCallback.Invoke();
        };

    }

    public void UnloadSceneAsync(string pSceneName, UnityAction pCallback)
    {
        Scene scene = SceneManager.GetSceneByName(pSceneName);
        if (scene != null && scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(pSceneName, UnloadSceneOptions.None).completed += (a) =>
            {
                pCallback?.Invoke();
            };
        }
    }

    /// <summary>
    /// 卸载场景
    /// </summary> 
    public void UnloadScene(string sceneName)
    {
        if (IsLoadScene(sceneName))
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
        else
        {
            Debug.LogWarning($"场景 {sceneName} 尚未加载，无法卸载。");
        }
    }

    public bool IsLoadScene(string sceneName)
    {
        return SceneManager.GetSceneByName(sceneName).isLoaded;
    }
}
