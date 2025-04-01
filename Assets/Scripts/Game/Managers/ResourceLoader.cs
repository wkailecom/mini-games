
using ScrewJam;
using UnityEngine;

public class ScrewResourceLoader : IResourceLoader
{
    public T Load<T>(string path) where T : Object
    {
        return AssetManager.Instance.LoadAsset<T>(path);
    }
}
