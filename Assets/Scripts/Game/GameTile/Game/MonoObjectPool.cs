using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoObjectPool : MonoBehaviour
{

    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private int initialCount;

    private Queue<GameObject> pooledInstanceQueue = new Queue<GameObject>();

    public void Init()
    {
        if (pooledInstanceQueue.Count < initialCount)
        {
            for (int i = 0; i < initialCount; i++)
            {
                pooledInstanceQueue.Enqueue(Instantiate(prefab, transform));
            }
        }
    }

    public GameObject GetInstance()
    {
        if (pooledInstanceQueue.Count > 0)
        {
            GameObject instanceToReuse = pooledInstanceQueue.Dequeue();
            instanceToReuse.SetActive(true);
            return instanceToReuse;
        }

        var tObj = Instantiate(prefab, transform);
        pooledInstanceQueue.Enqueue(tObj);
        return tObj;
    }

    public void ReturnInstance(GameObject pReturnGameObject)
    {
        pooledInstanceQueue.Enqueue(pReturnGameObject);
        //pReturnGameObject.SetActive(false);
        pReturnGameObject.transform.SetParent(transform);
    }

}