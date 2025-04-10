using Config;
using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using UnityEngine;

/// <summary>
/// 公共方法
/// </summary>
public static class GameMethod
{
    public static int[] NoRepeatRandom(int pMin, int pMax, int pTimes)
    {
        int tRange = pMax - pMin + 1;
        if (pTimes < 0 || pTimes > tRange)
        {
            LogManager.LogError("NoRepeatRandom: param wrong!");
            return new int[0];
        }

        int[] tResult = new int[pTimes];

        int[] tPool = new int[tRange];
        for (int i = 0; i < tRange; i++)
        {
            tPool[i] = pMin + i;
        }

        for (int i = 0; i < pTimes; i++)
        {
            int tIndex = UnityEngine.Random.Range(0, tRange);
            tResult[i] = tPool[tIndex];

            tPool[tIndex] = tPool[tRange - 1];
            --tRange;
        }

        return tResult;
    }

    public static int GetIndexByWeights(List<int> pWeights)
    {
        int tRandom = UnityEngine.Random.Range(0, pWeights.Sum());
        int tCurrent = 0;
        for (int i = 0; i < pWeights.Count; i++)
        {
            tCurrent += pWeights[i];
            if (tCurrent > tRandom)
            {
                return i;
            }
        }

        return 0;
    }

    public static void SetItemsActive<T>(this IList<T> pItemsPool, int pCount, T pItemAsset, Transform pItemsRoot) where T : Component
    {
        while (pItemsPool.Count < pCount)
        {
            pItemsPool.Add(UnityEngine.Object.Instantiate(pItemAsset, pItemsRoot));
        }

        for (int i = 0; i < pCount; i++)
        {
            pItemsPool[i].gameObject.SetActive(true);
        }
        for (int i = pCount; i < pItemsPool.Count; i++)
        {
            pItemsPool[i].gameObject.SetActive(false);
        }
    }

    public static List<string> GetAllFileWithoutMetaInDic(string path, string searchPattern = "*", string[] filters = null)
    {
        List<string> list = new List<string>();

        string[] files = System.IO.Directory.GetFiles(path, searchPattern, System.IO.SearchOption.AllDirectories);

        foreach (var file in files)
        {
            if (filters != null)
            {
                bool isFilter = false;
                foreach (var filter in filters)
                {
                    if (file.Contains(filter))
                    {
                        isFilter = true;
                        break;
                    }
                }

                if (isFilter) continue;
            }

            if (!file.Contains(".meta") && !file.StartsWith("."))
            {
                list.Add(file.Replace("\\", "/"));
            }
        }

        return list;
    }

    public static List<PropData> ParseProps(int[] pPropsIDs, int[] pPropsCounts)
    {
        var tResult = new List<PropData>();

        if (pPropsIDs.Length != pPropsCounts.Length)
        {
            LogManager.LogError("CommonMethod.ParseProps: pPropsIDs.Length != pPropsCounts.Length");
            return tResult;
        }

        for (int i = 0; i < pPropsIDs.Length; i++)
        {
            tResult.Add(new PropData((PropID)pPropsIDs[i], pPropsCounts[i]));
        }
        return tResult;
    }

    public static bool HasRemoveAD() => ModuleManager.Prop.HasProp(PropID.RemoveAD);
    public static void OpenBanner(bool pIsOpen)
    {
        if (HasRemoveAD()) return;
        if (pIsOpen)
        {
            ADManager.Instance.ShowBanner();
        }
        else
        {
            ADManager.Instance.HideBanner();
        }
    }
    public static bool IsFullEnergy() => ModuleManager.Prop.GetPropCount(PropID.Energy) >= CommonDefine.energyFunllCount;
    public static int GetIAPProductPropCount(IAPProductConfig pConfig, PropID pPropID)
    {
        if (pConfig == null)
        {
            return 0;
        }

        for (int i = 0; i < pConfig.propsID.Length && i < pConfig.propsCount.Length; i++)
        {
            if (pConfig.propsID[i] == (int)pPropID)
            {
                return pConfig.propsCount[i];
            }
        }

        return 0;
    }
    public static bool HasIAPProductProp(IAPProductConfig pConfig, PropID pPropID)
    {
        foreach (var tProp in pConfig.propsID)
        {
            if (tProp == (int)pPropID) return true;
        }
        return false;
    } 

    public static void TriggerUIAction(string pUIName, string pPageName, UIActionType pType, bool pIsReport = true)
    {
        var tEventData = EventManager.GetEventData<UIAction>(EventKey.UIAction);
        tEventData.UIName = pUIName;
        tEventData.UIPageName = pPageName;
        tEventData.actionType = pType;
        tEventData.isReport = pIsReport;
        EventManager.Trigger(tEventData);
    }

    public static void TriggerUIAction(string pUIName, string pPageName, UIActionType pType, ADType pADType)
    {
        var tEventData = EventManager.GetEventData<UIAction>(EventKey.UIAction);
        tEventData.UIName = pUIName;
        tEventData.UIPageName = pPageName;
        tEventData.actionType = pType;
        tEventData.ADType = pADType;
        tEventData.isReport = true;
        EventManager.Trigger(tEventData);
    }

    public static string GetCurrentDataPath() => GetDataPath(AppInfoManager.Instance.UserGroup);

    public static string GetDataPath(UserGroup pUserGroup)
    {
        return pUserGroup switch
        {
            UserGroup.GroupA => GameConst.CONFIG_ROOT_PATH,
            UserGroup.GroupB => GameConst.CONFIG_ROOT_PATHB,
            UserGroup.GroupC => GameConst.CONFIG_ROOT_PATHC,
            UserGroup.GroupD => GameConst.CONFIG_ROOT_PATHD,
            _ => GameConst.CONFIG_ROOT_PATH,
        };
    }

    public static bool IsAllowLogPurchase()
    {
        if (AppInfoManager.Instance.IsDebug)
        {
            return !GameVariable.IsSkipPurchasValidate;
        }
        else
        {
            return !GameVariable.IsTestDevice;
        }
    }

    public static int GetLoopIndex(int pIndex, int pLoopCount)
    {
        return (pIndex + pLoopCount - 1) % pLoopCount;
    }

    public static int GetCharCount(string str, char ch)
    {
        int count = 0;
        int index = 0;
        while ((index = str.IndexOf(ch, index)) != -1)
        {
            count++;
            index++;
        }
        return count;
    }

    public static Vector2 OtherWorldToSelfLocalPos(Transform otherRect, Transform selfRect, Camera otherCamera, Camera selfCamera)
    {
        Vector2 uiScreenPos = RectTransformUtility.WorldToScreenPoint(otherCamera, otherRect.position);
        bool isSucess = RectTransformUtility.ScreenPointToLocalPointInRectangle(selfRect.parent as RectTransform, uiScreenPos, selfCamera, out Vector2 localPos);
        return isSucess ? localPos : Vector2.zero;
    }

    public static Vector2 OtherWorldToSelfLocalPos(Transform otherRect, Transform selfRect, Camera camera)
    {
        return OtherWorldToSelfLocalPos(otherRect, selfRect, camera, camera);
    }

    public static Vector2 OtherWorldToSelfLocalPos<T>(T otherRect, T selfRect) where T : Component
    {
        return OtherWorldToSelfLocalPos(otherRect.transform, selfRect.transform, Camera.main);
    }

    public static bool IsSameDate(DateTime dt1, DateTime dt2)
    {
        return dt1.Date == dt2.Date;
    }

    public static bool IsSameHour(DateTime dt1, DateTime dt2)
    {
        return dt1.Date == dt2.Date && dt1.Hour == dt2.Hour;
    }
}
