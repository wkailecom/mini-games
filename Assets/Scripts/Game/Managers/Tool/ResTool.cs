using Config;
using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static AdEvents;

public static class ResTool
{
    public static T CreatePrefab<T>(string pPrefabPath, Transform pParent) where T : MonoBehaviour
    {
        GameObject tGameObject = AssetManager.Instance.LoadPrefab(pPrefabPath, pParent);
        if (tGameObject == null)
        {
            LogManager.LogError("CreatePrefab Fail! Path : " + pPrefabPath);
            return null;
        }

        if (!tGameObject.TryGetComponent<T>(out var tComponent))
        {
            LogManager.LogError("Prefab Missing Component : " + typeof(T).ToString());
            return null;
        }

        return tComponent;
    }

    public static Sprite LoadPropIcon(string pIconName)
    {
        var mPropAtlas = AssetManager.Instance.LoadAsset<SpriteAtlas>(GameConst.ATLAS_PROPS_PATH);
        return mPropAtlas.GetSprite(pIconName);
    }

    public static void SetPropIcon(this Image pImage, string pIconName, bool pSetNativeSize = false)
    {
        pImage.sprite = LoadPropIcon(pIconName);
        if (pSetNativeSize) pImage.SetNativeSize();
    }

    public static void SetPropIcon(this Image pImage, int pPropID, bool pSetNativeSize = false)
    {
        var mConfig = ConfigData.propConfig.GetByPrimary(pPropID);
        if (mConfig == null)
        {
            LogManager.LogError("SetPropIcon PropID null! ");
            return;
        }
        pImage.sprite = LoadPropIcon(mConfig.icon);
        if (pSetNativeSize) pImage.SetNativeSize();
    }

    public static void SetPropIcon(this Image pImage, PropID pPropID, bool pSetNativeSize = false)
    {
        SetPropIcon(pImage, (int)pPropID, pSetNativeSize);
    }

}
