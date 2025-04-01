using Config;
using LLFramework;
using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class NetAPI
{
    public const string PARAM_NAME_OS = "os";                   // 操作系统
    public const string PARAM_NAME_IS_RELEASE = "release";      // 是否为正式包
    public const string PARAM_NAME_USER_ID = "userId";
    public const string PARAM_NAME_APP_VERSION = "version";     // 版本号
    public const string PARAM_NAME_RESOURCE_ID = "resources";   // 热更资源ID
    public const string PARAM_NAME_DEVICEID = "deviceId";
    private const string ApiKey = "BE39AFC9462243B31E9CDB10B5C58765";

#if UNITY_ANDROID
    public const int PARAM_VALUE_OS = 0;
#elif UNITY_IOS
    public const int PARAM_VALUE_OS = 1;
#else
    public const int PARAM_VALUE_OS = 0;
#endif

    public static void PostForm(string pURL, WWWForm pForm, UnityAction<bool, string> pCallback, int pTimeout = 0)
    {
        TaskManager.Instance.StartTask(PostFormTask(pURL, pForm, pCallback, pTimeout));
    }

    public static IEnumerator PostFormTask(string pURL, WWWForm pForm, UnityAction<bool, string> pCallback, int pTimeout = 0)
    {
        UnityWebRequest tRequest = UnityWebRequest.Post(pURL, pForm);
        yield return RequestTask(tRequest, pCallback, pTimeout);
    }

    public static void PostJSON<T>(string pURL, T pJsonString, UnityAction<bool, string> pCallback, int pTimeout = 0) where T : new()
    {
        PostJSON(pURL, Newtonsoft.Json.JsonConvert.SerializeObject(pJsonString), pCallback, pTimeout);
    }

    public static void PostJSON(string pURL, string pJsonString, UnityAction<bool, string> pCallback, int pTimeout = 0)
    {
        TaskManager.Instance.StartTask(PostJSONTask(pURL, pJsonString, pCallback, pTimeout));
    }

    public static IEnumerator PostJSONTask(string pURL, string pJsonString, UnityAction<bool, string> pCallback, int pTimeout = 0)
    {
        UnityWebRequest tRequest = new UnityWebRequest(pURL, "POST");
        byte[] tCompressAfterByte = Compress(Encoding.UTF8.GetBytes(pJsonString));
        tRequest.uploadHandler = new UploadHandlerRaw(tCompressAfterByte);
        tRequest.downloadHandler = new DownloadHandlerBuffer();
        tRequest.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
        tRequest.SetRequestHeader("Authorization", $"Api-Key {ApiKey}");
        tRequest.SetRequestHeader("Content-Encoding", "gzip");
        tRequest.SetRequestHeader("Accept-Encoding", "gzip");

        yield return RequestTask(tRequest, pCallback, pTimeout);
    }

    public static byte[] Compress(byte[] data)
    {
        try
        {
            using var ms = new MemoryStream();
            using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(data, 0, data.Length);
            }
            return ms.ToArray();
        }
        catch (Exception e)
        {
            throw new Exception("Compression failed: " + e.Message);
        }
    }

    /// <summary>
    /// 字符串解压缩
    /// </summary>
    public static string DecompressString(string str)
    {
        try
        {
            byte[] compressBeforeByte = Convert.FromBase64String(str);
            byte[] compressAfterByte = Decompress(compressBeforeByte);
            return Encoding.UTF8.GetString(compressAfterByte);
        }
        catch (Exception e)
        {
            throw new Exception("String decompression failed: " + e.Message);
        }
    }

    public static byte[] Decompress(byte[] data)
    {
        try
        {
            using var ms = new MemoryStream(data);
            using var zip = new GZipStream(ms, CompressionMode.Decompress);
            using var msreader = new MemoryStream();
            zip.CopyTo(msreader);
            return msreader.ToArray();
        }
        catch (Exception e)
        {
            throw new Exception("Decompression failed: " + e.Message);
        }
    }

    static IEnumerator RequestTask(UnityWebRequest tRequest, UnityAction<bool, string> pCallback, int pTimeout = 0)
    {
        if (pTimeout > 0)
        {
            tRequest.timeout = pTimeout;
        }

        yield return tRequest.SendWebRequest();
        if (tRequest.IsSuccess())
        {
            pCallback?.Invoke(true, tRequest.downloadHandler.text);
        }
        else
        {
            pCallback?.Invoke(false, tRequest.error);
        }
    }

    public static void DownLoadFile(string pURL, UnityAction<bool, byte[]> pCallback)
    {
        TaskManager.Instance.StartTask(DownLoadFileTask(pURL, pCallback));
    }

    public static IEnumerator DownLoadFileTask(string pURL, UnityAction<bool, byte[]> pCallback)
    {
        using (UnityWebRequest tRequest = UnityWebRequest.Get(pURL))
        {
            yield return tRequest.SendWebRequest();
            if (tRequest.IsSuccess())
            {
                pCallback?.Invoke(true, tRequest.downloadHandler.data);
            }
            else
            {
                pCallback?.Invoke(false, null);
            }
        }
    }

    public static bool IsSuccess(this UnityWebRequest pRequest)
    {
        return pRequest.result != UnityWebRequest.Result.ConnectionError
            && pRequest.result != UnityWebRequest.Result.ProtocolError
            && pRequest.result != UnityWebRequest.Result.DataProcessingError;
    }

    public static void SetRequestBaseMembers(CSRequestBase pRequestData)
    {
        pRequestData.uuid = AppInfoManager.Instance.UserID;
        pRequestData.version = AppInfoManager.Instance.AppVersion;
        pRequestData.os = PARAM_VALUE_OS;
        pRequestData.platform = 0;
        //pRequestData.clientTime =TimeTool.GetTimeStampLong();
    }

    public static string FunctionURL(string pFunctionName)
    {
        return APPDefine.serverFunctionURL + pFunctionName;
    }
}

public class CSRequestBase
{
    public string uuid;
    public string version;
    public int os;
    public int platform;
    //public long clientTime;
}

