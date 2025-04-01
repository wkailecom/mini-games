using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pool
{
    [Serializable]
    public class ObjectPoolItem
    {
        public GameObject objectToPool;
        public string poolName;
        public int amountToPool;
        public bool shouldExpand = true;
    }

    public class ObjectPool : MonoBehaviour
    {
        private const string DefaultRootObjectPoolName = "Pooled Objects";

        public static ObjectPool Instance;
        public string rootPoolName = DefaultRootObjectPoolName;
        public List<PoolBehaviour> pooledObjects = new List<PoolBehaviour>();
        private List<ObjectPoolItem> itemsToPool;


        private void OnEnable()
        {
            LoadFromScriptable();
            Instance = this;
            if (!Application.isPlaying) return;
            ClearNullElements();

            if (itemsToPool != null)
            {
                foreach (var item in itemsToPool)
                {
                    CreateObject(item);
                }
            }
          
        }

        private void LoadFromScriptable()
        {
            var poolSettings = Resources.Load<PoolScriptable>("PoolSettings");
            itemsToPool = poolSettings.itemsToPool;
            foreach (var item in itemsToPool)
            {
                CreateObject(item);
            }
        }

        private void CreateObject(ObjectPoolItem item)
        {
            if (item == null) return;
            if (item.objectToPool == null) return;
            var pooledCount = pooledObjects.Count(i => i.itemName == item.objectToPool.name);
            for (var i = 0; i < item.amountToPool - pooledCount; i++)
            {
                CreatePooledObject(item);
            }
        }

        private void ClearNullElements()
        {
            pooledObjects.RemoveAll(i => i == null);
        }

        private GameObject GetParentPoolObject(string objectPoolName)
        {
            var parentObject = GameObject.Find(objectPoolName);
            // Create the parent object if necessary
            if (parentObject == null)
            {
                parentObject = new GameObject();
                parentObject.name = objectPoolName;

                // Add sub pools to the root object pool if necessary
                if (objectPoolName != rootPoolName)
                    parentObject.transform.parent = transform;
            }

            return parentObject;
        }

        public void HideObjects(string poolTag)
        {
            var pbs = FindObjectsOfType<PoolBehaviour>();
            var objects = pbs.Where(i => i.itemName == poolTag);
            var parent = GetParentPoolObject(poolTag);
            foreach (var item in objects)
            {
                item.gameObject.SetActive(false);
                item.transform.SetParent(parent.transform);
            }
        }

        public void HideSearchPattern(string pattern)
        {
            var pbs = FindObjectsOfType<PoolBehaviour>();
            var objects = pbs.Where(i => i.itemName.Contains(pattern));
            foreach (var item in objects)
            {
                var parent = GetParentPoolObject(item.itemName);
                item.gameObject.SetActive(false);
                item.transform.SetParent(parent.transform);
            }
        }

        public void PutBack(GameObject obj)
        {
            obj.SetActive(false);
        }

        public GameObject GetPooledObject(string name, Object activatedBy = null, bool active = true,
            bool canBeActive = false)
        {
            ClearNullElements();
            PoolBehaviour obj = null;
            for (var i = 0; i < pooledObjects.Count; i++)
            {
                if (pooledObjects[i] == null) continue;
                if ((!pooledObjects[i].gameObject.activeSelf || canBeActive) && pooledObjects[i].itemName == name)
                {
                    obj = pooledObjects[i];
                    if (obj) break;
                }
            }

            if (itemsToPool == null) LoadFromScriptable();
            if (!obj)
            {
                foreach (var item in itemsToPool)
                {
                    if (item != null && item.objectToPool == null) continue;
                    if (item.objectToPool.name == name)
                    {
                        if (item.shouldExpand)
                        {
                            obj = CreatePooledObject(item);
                            break;
                        }
                    }
                }
            }

            if (obj != null)
            {
                obj.gameObject.SetActive(active);
                return obj.gameObject;
            }

            return null;
        }

        private PoolBehaviour CreatePooledObject(ObjectPoolItem item)
        {            
            var parentPoolObject = GetParentPoolObject(item.poolName);
            var obj = Instantiate(item.objectToPool, parentPoolObject.transform, true);
            obj.name = item.objectToPool.name;
            var poolBehaviour = obj.GetComponent<PoolBehaviour>();
            if (poolBehaviour == null)
                poolBehaviour = obj.AddComponent<PoolBehaviour>();
            poolBehaviour.itemName = item.objectToPool.name;
            if (!string.IsNullOrEmpty(poolBehaviour.parentPath))
            {
                obj.transform.SetParent(GameObject.Find(poolBehaviour.parentPath).transform);
                obj.transform.localScale = item.objectToPool.transform.localScale;
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.PrefabUtility.RevertPrefabInstance(obj, UnityEditor.InteractionMode.AutomatedAction);
            }
#endif

            obj.SetActive(false);
            pooledObjects.Add(poolBehaviour);


            return poolBehaviour;
        }

        public void DestroyObjects(string tag)
        {
            for (var i = 0; i < pooledObjects.Count; i++)
            {
                if (pooledObjects[i].itemName == tag)
                {
                    DestroyImmediate(pooledObjects[i]);
                }
            }

            ClearNullElements();
        }
    }
}