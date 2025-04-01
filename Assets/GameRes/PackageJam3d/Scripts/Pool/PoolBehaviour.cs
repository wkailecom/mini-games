using UnityEngine;
using UnityEngine.Serialization;

namespace Pool
{
    public class PoolBehaviour : MonoBehaviour
    {
        public string poolName;
        public int amountToPool;
        public bool shouldExpand = true;
        public string parentPath;
        [FormerlySerializedAs("name")] [HideInInspector]
        public string itemName;

        public ObjectPoolItem ConvertToObjectPoolItem()
        {
            var b = new ObjectPoolItem
            {
                poolName = poolName,
                amountToPool = amountToPool,
                shouldExpand = shouldExpand,
                objectToPool = gameObject
            };
            return b;
        }
    }
}