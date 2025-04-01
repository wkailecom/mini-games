using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

enum DownLoadType
{
    Downloading = 0,
    DownLoaded = 1,
}


public class GameLoader : MonoBehaviour
{
    const string DOWNLOAD_ASSET_KEY = "DownloadAsset";
    const string DOWNLOAD_RRTRY_COUNT_KEY = "AssetDownloadRetryCount";
    private string gameSceneName = "Launcher";
    private string updateSceneName = "Loader";
    private string backupPath;
    private string sourcePath;

    public LoadDll loadDll;
    public UILoading uiloading;
    public GameObject LogConsole;

    private int downloadAssetType
    {
        get => PlayerPrefs.GetInt(DOWNLOAD_ASSET_KEY, 1);
        set => PlayerPrefs.SetInt(DOWNLOAD_ASSET_KEY, value);
    }
    private int downloadRetryCount
    {
        get => PlayerPrefs.GetInt(DOWNLOAD_RRTRY_COUNT_KEY, 0);
        set => PlayerPrefs.SetInt(DOWNLOAD_RRTRY_COUNT_KEY, value);
    }

    private float waitHandTime = 5;
    private int retryDownloadCount = 3;
    private float InitialProgressPercentage = 0.1f;
    private float hotUpdateProgressPercentage = 0.5f;
    private float loadSceneProgressPercentage = 0.2f;

    private bool waitHanding = true;
    private Coroutine waitHandle;
    private int failedDownloadCount;
    void Start()
    {
#if GM_MODE
        Instantiate(LogConsole);
#endif
        //uiloading.OnLoadingComplete += LoadGameScene;
        uiloading.Init();

        InitAB();
    }

    public void InitAB()
    {
        backupPath = Application.persistentDataPath + "/catalogBackup";
        sourcePath = Application.persistentDataPath + "/com.unity.addressables";
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (downloadAssetType == (int)DownLoadType.Downloading)
            {
                //尝试恢复catalog
                if (RestoreCatalog())
                {
                    downloadAssetType = (int)DownLoadType.DownLoaded;
                }
                else
                {
                    //恢复失败，删除catalog缓存
                    DeleteCatalog();
                }
            }
        }

        waitHandle = StartCoroutine(WaitHandle());
        StartCoroutine(InitAddressables());
    }


    IEnumerator InitAddressables()
    {
        var initHandle = Addressables.InitializeAsync();
        Debug.Log("initAA: initHandle");
        uiloading.SetTargetProgress(InitialProgressPercentage);
        while (!initHandle.IsDone && waitHanding)
        {
            yield return null;
        }
        if (!waitHanding) yield break;

        var checkHandle = Addressables.CheckForCatalogUpdates(false);
        Debug.Log("initAAA:  checkHandle");
        while (!checkHandle.IsDone && waitHanding)
        {
            yield return null;
        }
        if (!waitHanding) yield break;

        if (checkHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("initAAA: check result count: " + checkHandle.Result.Count);
            if (checkHandle.Result.Count > 0)
            {
                Debug.Log("initAAA: 更新catalog");
                //列表有更新，代表可能有新的资源需要下载
                downloadAssetType = (int)DownLoadType.Downloading;
                var updateHandle = Addressables.UpdateCatalogs(checkHandle.Result, false);
                yield return updateHandle;

                Debug.Log("initAAA: start download");
                var locators = updateHandle.Result;
                int tCount = 0;
                int tTotal = locators.Count;
                foreach (var locator in locators)
                {
                    yield return DownloadAsset(locator, (float)(tCount * hotUpdateProgressPercentage + InitialProgressPercentage) / tTotal, hotUpdateProgressPercentage / tTotal);
                    tCount++;
                }
                Addressables.Release(updateHandle);
                if (failedDownloadCount == 0)
                {
                    downloadAssetType = (int)DownLoadType.DownLoaded;
                }
            }
            else
            {
                if (downloadAssetType == (int)DownLoadType.Downloading)
                {
                    Debug.Log("initAAA: 断点续传");
                    yield return DownloadUncompletedAsset(Addressables.ResourceLocators);
                    if (failedDownloadCount == 0)
                    {
                        downloadAssetType = (int)DownLoadType.DownLoaded;
                    }
                }
                else
                {
                    Debug.Log("initAAA: 没有需要加载的内容");
                }
            }
        }
        //Addressables.Release(initHandle);
        Addressables.Release(checkHandle);

        //有文件未更新成功
        if (failedDownloadCount > 0)
        {
            if (downloadRetryCount > retryDownloadCount)
            {
                //重试次数过多，删除缓存的catalog文件
                DeleteCatalog();
                downloadRetryCount = 0;
                downloadAssetType = (int)DownLoadType.DownLoaded;
                //重新加载场景
                yield return SceneManager.LoadSceneAsync(updateSceneName);
                yield break;
            }

            //记录失败次数
            downloadRetryCount += 1;
            //重新加载场景
            yield return SceneManager.LoadSceneAsync(updateSceneName);
            yield break;
        }

        downloadRetryCount = 0;
        BackupCatalog(); //更新成功，备份catalog文件

        LoadGameScene();
    }

    IEnumerator DownloadAsset(IResourceLocator pLocator, float pCurPercent = 0, float selfPercent = 0)
    {
        var sizeHandle = Addressables.GetDownloadSizeAsync(pLocator.Keys);
        yield return sizeHandle;

        var totalDownloadSize = Mathf.Floor(sizeHandle.Result / 1024f);
        if (sizeHandle.Result > 0)
        {
            downloadAssetType = (int)DownLoadType.Downloading;
            var downloadHandle = Addressables.DownloadDependenciesAsync(pLocator.Keys, Addressables.MergeMode.Union);
            while (!downloadHandle.IsDone)
            {
                // 下载进度
                var downloadStatus = downloadHandle.GetDownloadStatus();
                float percentage = (float)Math.Round(downloadStatus.Percent * 100, 2);
                uiloading.SetTargetProgress(pCurPercent + downloadStatus.Percent * selfPercent);
                Debug.Log($"downloadAA: {downloadStatus.DownloadedBytes}/{downloadStatus.TotalBytes} ====> {percentage}%");

                yield return null;
            }

            if (!downloadHandle.IsValid() || downloadHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("downloadAA:下载失败");
            }
            else
            {
                Debug.Log("downloadAA:下载完成");
            }
            Addressables.Release(downloadHandle);
        }
        Addressables.Release(sizeHandle);
    }

    //获取未下载完成的资源
    IEnumerator DownloadUncompletedAsset(IEnumerable<IResourceLocator> locators)
    {
        var tUncompletedLocator = new List<IResourceLocator>();
        //获取需要下载资源的locator
        foreach (var locator in locators)
        {
            var sizeHandle = Addressables.GetDownloadSizeAsync(locator.Keys);
            yield return sizeHandle;
            if (sizeHandle.Result > 0)
            {
                tUncompletedLocator.Add(locator);
            }
        }

        int tCount = 0;
        int tTotal = tUncompletedLocator.Count;
        foreach (var locator in tUncompletedLocator)
        {
            yield return DownloadAsset(locator, (float)(tCount * hotUpdateProgressPercentage + InitialProgressPercentage) / tTotal, hotUpdateProgressPercentage / tTotal);
            tCount++;
        }
    }

    IEnumerator WaitHandle()
    {
        yield return new WaitForSeconds(waitHandTime);
        if (waitHanding)
        {
            if (downloadAssetType == (int)DownLoadType.Downloading) yield break;
            Debug.Log("热更新初始化超过5秒,跳过热更新!");
            waitHanding = false;
            LoadGameScene();
        }
    }

    private void LoadGameScene()
    {
        if (waitHandle != null)
        {
            StopCoroutine(waitHandle);
            waitHandle = null;
        }

        loadDll.Init();

        Debug.Log("加载游戏场景");//带有热更脚本的资源必须通过加载ab方式加载
                            // Addressables.LoadSceneAsync(gameSceneName).WaitForCompletion();
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        var handle = Addressables.LoadSceneAsync(gameSceneName, LoadSceneMode.Single, false);

        var tCurProgress = uiloading.progressBar.value;
        while (!handle.IsDone)
        {
            float progress = tCurProgress + (loadSceneProgressPercentage) * handle.PercentComplete;
            uiloading.SetTargetProgress(progress);
            //Debug.Log("Loading progress: " + progress * 100 + "%");
            yield return null;
        }

        //uiloading.OnLoadingComplete = () =>
        //{
        //    handle.Result.ActivateAsync();
        //};
        handle.Result.ActivateAsync();
    }



    #region catalog文件处理

    void DeleteCatalog()
    {
        if (Directory.Exists(sourcePath))
        {
            CleanDirectory(sourcePath);
            Debug.Log(sourcePath + "删除成功");
        }
        else
        {
            Debug.Log($"文件夹不存在：{sourcePath}");
        }
    }

    void BackupCatalog()
    {
        if (!Directory.Exists(backupPath))
        {
            Directory.CreateDirectory(backupPath);
        }
        CopyFile(sourcePath, backupPath);
        Debug.Log("finish backupCatalog");
    }

    bool RestoreCatalog()
    {
        return CopyFile(backupPath, sourcePath);
    }

    //复制文件夹内容
    bool CopyFile(string oldPath, string newPath)
    {
        if (!Directory.Exists(oldPath) || !Directory.Exists(newPath))
        {
            return false;
        }

        DirectoryInfo dir = new DirectoryInfo(oldPath);
        FileInfo[] allFiles = dir.GetFiles("*", SearchOption.AllDirectories);
        if (allFiles.Length == 0)
        {
            return false;
        }

        foreach (var item in allFiles)
        {
            File.WriteAllBytes($"{newPath}/{item.Name}", File.ReadAllBytes(item.FullName));
        }
        return true;
    }

    void CleanDirectory(string pPath)
    {
        if (Directory.Exists(pPath))
        {
            DirectoryInfo dir = new DirectoryInfo(pPath);
            if (dir != null)
            {
                foreach (var file in dir.GetFiles())
                {
                    file.Delete();
                }
                foreach (var subDir in dir.GetDirectories())
                {
                    subDir.Delete(true);
                }
            }
        }
    }

    #endregion
}


