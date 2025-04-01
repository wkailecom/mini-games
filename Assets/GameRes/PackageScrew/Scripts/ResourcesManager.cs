using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ScrewJam
{
    public interface IResourceLoader
    {
        T Load<T>(string path) where T : Object;
    }

    public class ResourcesManager
    {
        public static IResourceLoader _resourceLoader;

        public static void SetResourceLoader(IResourceLoader resourceLoader)
        {
            _resourceLoader = resourceLoader;
        }

        public static bool LoadAsset(string path, UnityAction<Object> callback)
        {
            Debug.Log($"ScrewJam ResourcesManager loadPath:{path}");
            var obj = _resourceLoader.Load<Object>(path);
            callback?.Invoke(obj);
            return true;
        }

        public static bool LoadAsset<T>(string path, UnityAction<T> callback) where T : Object
        {
            Debug.Log($"ScrewJam ResourcesManager type:{typeof(T).FullName}, loadPath:{path}");
            var obj = _resourceLoader.Load<T>(path);
            callback?.Invoke(obj);
            return true;
        }

        public static T LoadAsset<T>(string path) where T : Object
        {
            //Debug.Log($"ScrewJam ResourcesManager type:{typeof(T).FullName}, loadPath:{path}");
            var obj = _resourceLoader.Load<T>(path);
            return obj;
        }
    }

}
