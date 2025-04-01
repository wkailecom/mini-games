using System.Collections.Generic;
using UnityEngine;

namespace ScrewJam
{
    public static class GameUtils
    {
        /// <summary>
        /// 检测板子是否有重叠
        /// </summary>
        /// <param name="poly"></param>
        /// <returns></returns>
        public static bool CheckBoardOverlap(IEnumerable<Collider2D> poly)
        {
            bool result = false;
            var checkLayer = LayerMask.NameToLayer("Water");
            int originalLayer = -1;
            foreach (var item in poly)
            {
                if (originalLayer == -1)
                    originalLayer = item.gameObject.layer;
                item.gameObject.layer = checkLayer;
            }
            ContactFilter2D filter = new ContactFilter2D();
            filter.layerMask = 1 << checkLayer;
            filter.useLayerMask = true;
            Collider2D[] colliders = new Collider2D[5];
            foreach (var item in poly)
            {
                var t = item.OverlapCollider(filter, colliders);
                if (t > 0)
                {
                    Debug.Log($"{item.name} 和其他碰撞有重叠,碰撞个数{t}", item);
                }
            }
            foreach (var item in poly)
            {
                item.gameObject.layer = originalLayer;
            }
            return result;
        }

        /// <summary>
        /// 检测螺丝重叠
        /// </summary>
        /// <param name="point">位置</param>
        /// <param name="radius">半径</param>
        /// <param name="poly">要检测的碰撞</param>
        /// <returns>重叠碰撞体数量</returns>
        public static int CheckScrewOverlapNonAlloc(Vector3 point, float radius, IEnumerable<Collider2D> poly, Collider2D[] result)
        {
            int num = 0;
            var checkLayer = LayerMask.NameToLayer("Water");
            int originalLayer = -1;
            foreach (var item in poly)
            {
                if (originalLayer == -1)
                    originalLayer = item.gameObject.layer;
                item.gameObject.layer = checkLayer;
            }
            var resultCount = Physics2D.OverlapCircleNonAlloc(point, radius, result, 1 << checkLayer);
            if (resultCount > 0)
                num = resultCount;
            foreach (var item in poly)
            {
                item.gameObject.layer = originalLayer;
            }
            return num;
        }

    }
}
