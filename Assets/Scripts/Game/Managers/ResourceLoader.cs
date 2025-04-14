
using LLFramework;
using UnityEngine;


public class ResourceLoader : Singleton<ResourceLoader>
{
    public void Init()
    {
        ScrewJam.ResourcesManager.SetResourceLoader(new ScrewResourcesLoader());
        BusOut.ResourcesManager.SetResourceLoader(new BusOutResourcesLoader());
    }
}

public class ScrewResourcesLoader : ScrewJam.IResourceLoader
{
    public T Load<T>(string path) where T : Object
    {
        return AssetManager.Instance.LoadAsset<T>(path);
    }
}

public class BusOutResourcesLoader : BusOut.IResourceLoader
{
    public T Load<T>(string path) where T : Object
    {
        return AssetManager.Instance.LoadAsset<T>(path);
    }
}
