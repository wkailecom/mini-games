using System;
using System.Collections.Generic;

namespace LLFramework
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        public Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

        public void AddPoolObject<T>(int pCount) where T : PoolObjectBase, new()
        {
            Type tPoolType = typeof(T);
            if (!pools.ContainsKey(tPoolType.FullName))
            {
                ObjectPool pool = new ObjectPool(tPoolType);
                pools[tPoolType.FullName] = pool;
            }

            pools[tPoolType.FullName].AddPoolObject(pCount);
        }

        public T GetPoolObject<T>() where T : PoolObjectBase, new()
        {
            if (pools.TryGetValue(typeof(T).FullName, out ObjectPool tPool))
            {
                return tPool.GetPoolObject() as T;
            }
            else
            {
                AddPoolObject<T>(1);
                return GetPoolObject<T>();
            }
        }

        public void ReturnPoolObject<T>(ref T pPoolObject) where T : PoolObjectBase
        {
            if (pPoolObject == null)
            {
                LogManager.LogError("ObjectPoolManager.ReturnPoolObject param is null");
                return;
            }

            if (pools.TryGetValue(pPoolObject.GetType().FullName, out ObjectPool tPool))
            {
                tPool.ReturnPoolObject(pPoolObject);
            }
            else
            {
                LogManager.LogError("ObjectPoolManager.ReturnPoolObject no such pool : " + pPoolObject.GetType().FullName);
            }
            pPoolObject = null;
        }

        public void ReleasePools()
        {
            foreach (var tPool in pools.Values)
            {
                tPool.ClearPool();
            }
            pools.Clear();
        }
    }
}