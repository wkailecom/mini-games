using Config;
using Game;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public static class ResTool
{
    public static T CreatePrefab<T>(string pPrefabPath, Transform pParent) where T : Component
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

    public static T CreatePrefab<T>(string pPrefabName, string pRootPath, Transform pParent) where T : Component
    {
        return CreatePrefab<T>(Path.Combine(pRootPath, pPrefabName), pParent);
    }

    public static Sprite LoadIcon(string pIconName, string pAtlasPath)
    {
        var tAtlas = AssetManager.Instance.LoadAsset<SpriteAtlas>(pAtlasPath);
        return tAtlas.GetSprite(pIconName);
    }

    public static Sprite LoadPropIcon(string pIconName)
    {
        return LoadIcon(pIconName, GameConst.ATLAS_PROPS_PATH);
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
