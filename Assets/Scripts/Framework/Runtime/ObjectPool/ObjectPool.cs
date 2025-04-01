using System;
using System.Collections.Generic;

namespace LLFramework
{
    public class ObjectPool
    {
        Type mPoolType;
        Stack<PoolObjectBase> freeObjects;

        public ObjectPool(Type pPoolType)
        {
            mPoolType = pPoolType;
            freeObjects = new Stack<PoolObjectBase>();
        }

        public void AddPoolObject(int pCount)
        {
            for (int i = 0; i < pCount; i++)
            {
                ReturnPoolObject(Activator.CreateInstance(mPoolType) as PoolObjectBase);
            }
        }

        public PoolObjectBase GetPoolObject()
        {
            if (freeObjects.Count > 0)
            {
                PoolObjectBase tObject = freeObjects.Pop();
                return tObject;
            }
            else
            {
                AddPoolObject(1);
                return GetPoolObject();
            }
        }

        public void ReturnPoolObject(PoolObjectBase pPoolObject)
        {
            pPoolObject.SetObjectFree();
            freeObjects.Push(pPoolObject);
        }

        public void ClearPool()
        {
            freeObjects.Clear();
        }
    }
}