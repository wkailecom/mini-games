using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoPool<T> where T : MonoBehaviour
{
    private bool mIsAutoActive = false;            // 自动Active， 获取true，释放false

    // 注： 大量SetParent会导致性能降低
    private Transform mTransNewParent = null;      // 创建的时候 挂载到目标节点下作为子节点
    private Transform mTransFreeParent = null;     // 回收的时候 挂载到目标节点下作为子节点

    private T mPrefabObj = null;
    private Queue<T> mQueueObjs = new Queue<T>();  // 池 被回收
    private List<T> mListUsing = new List<T>();    // 池 使用中

    public List<T> ListData => mListUsing;


    /// <summary>
    /// 构造初始化
    /// </summary>
    /// <param name="prefabObj">实例化对象Prefab</param>
    /// <param name="transNewParent">创建父节点</param>
    /// <param name="transFreeParent">销毁父节点</param>
    /// <param name="isAutoActive">自动Active</param>
    public MonoPool(T prefabObj, Transform transNewParent = null, Transform transFreeParent = null, bool isAutoActive = false)
    {
        mPrefabObj = prefabObj;
        mTransNewParent = transNewParent;
        mTransFreeParent = transFreeParent;
        mIsAutoActive = isAutoActive;
    }

    public T GetOne()
    {
        return GetOne(null);
    }

    public T GetOne(Transform parent)
    {
        Transform transParent = parent ?? mTransNewParent;
        T result;
        if (mQueueObjs.Count > 0)
        {
            result = mQueueObjs.Dequeue();
            if (transParent != null && result.transform.parent != transParent)
            {
                result.transform.SetParent(transParent);
            }
            result.transform.localPosition = Vector3.zero;
        }
        else
        {
            if (transParent == null)
            {
                result = Object.Instantiate(mPrefabObj);
            }
            else
            {
                result = Object.Instantiate(mPrefabObj, transParent, false);
            }
        }

        if (mIsAutoActive)
        {
            result.gameObject.SetActive(true);
        }
        mListUsing.Add(result);

        return result;
    }

    public void FreeOne(T obj)
    {
        if (mListUsing.Contains(obj))
        {
            FreeLogicStep(obj);
            mListUsing.Remove(obj);
        }
    }

    /// <summary>
    ///  回收所有
    /// </summary>
    /// <param name="isDestory">true=删除所有对象， false=只回收,不删除</param>
    public void FreeAll(bool isDestory = false)
    {
        if (isDestory)
        {
            for (int i = 0; i < mListUsing.Count; i++)
            {
                Object.Destroy(mListUsing[i].gameObject);
            }
            foreach (var item in mQueueObjs)
            {
                Object.Destroy(item.gameObject);
            }
            mQueueObjs.Clear();
        }
        else
        {
            for (int i = 0; i < mListUsing.Count; i++)
            {
                FreeLogicStep(mListUsing[i]);
            }
        }
        mListUsing.Clear();
    }

    private void FreeLogicStep(T obj)
    {
        if (mIsAutoActive)
        {
            obj.gameObject.SetActive(false);
        }
        if (mTransFreeParent != null && obj.transform.parent != mTransFreeParent)
        {
            obj.transform.SetParent(mTransFreeParent);
        }
        mQueueObjs.Enqueue(obj);
    }
}
