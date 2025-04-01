#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace ScrewJam.ScrewEditor
{
    public partial class MapBuilder : MonoBehaviour
    {
        /// <summary>
        /// 检查板子间重叠
        /// </summary>
        private void CheckLayersBoardOverlap()
        {
            foreach (var layerTr in mapRoot.Cast<Transform>())
            {
                var boards = layerTr.Cast<Transform>().Select(t => t.GetComponent<Collider2D>());
                GameUtils.CheckBoardOverlap(boards);
            }
            Debug.Log("done");
        }

        /// <summary>
        /// 检测螺丝重叠并显示
        /// </summary>
        private void ShowLayersScrewOverlap()
        {
            var tScrewTrans = mapRoot.GetComponentsInChildren<SortingGroup>().Select(t => t.transform).ToList();

            int layerCount = 0;
            List<Collider2D> cacheCollider = new List<Collider2D>();
            var descLayers = mapRoot.Cast<Transform>().OrderByDescending(t => t.GetSiblingIndex());
            Collider2D[] tempResult = new Collider2D[1];
            foreach (var layerTr in descLayers)
            {
                if (layerCount != 0)
                {
                    var screws = layerTr.GetComponentsInChildren<SortingGroup>().Select(t => t.transform);
                    foreach (var screwTr in screws)
                    {
                        int overlapCount = GameUtils.CheckScrewOverlapNonAlloc(screwTr.position, Constant.SCREW_RADIUS, cacheCollider, tempResult);
                        if (overlapCount > 0)
                        {
                            SetScrewColor(screwTr, Color.red);
                        }
                    }
                }
                var boards = layerTr.Cast<Transform>().Select(t => t.GetComponent<Collider2D>());
                cacheCollider.AddRange(boards);
                layerCount++;
            }
        }

        /// <summary>
        /// 检查并标记螺丝数据
        /// </summary>
        private void CheckScrewRelated()
        {
            var tScrewTrans = mapRoot.GetComponentsInChildren<ScrewEditorData>().Select(t => t.transform).ToList();

            int layerCount = 0;
            Collider2D[] boardResult = new Collider2D[10];
            List<Collider2D> cacheCollider = new List<Collider2D>();
            var descLayers = mapRoot.Cast<Transform>().OrderByDescending(t => t.GetSiblingIndex());
            foreach (var layerTr in descLayers)
            {
                if (layerCount != 0)
                {
                    var screws = layerTr.GetComponentsInChildren<ScrewEditorData>();
                    foreach (var screwTr in screws)
                    {
                        var resultNumber = GameUtils.CheckScrewOverlapNonAlloc(screwTr.transform.position, Constant.SCREW_RADIUS, cacheCollider, boardResult);
                        if (resultNumber > 0)
                        {
                            List<int> overlapScrewIndex = new List<int>();
                            for (int i = 0; i < resultNumber; i++)
                            {
                                var boardChild = boardResult[i].transform.GetComponentsInChildren<ScrewEditorData>();
                                IEnumerable<int> elements = boardChild.Select(t => tScrewTrans.IndexOf(t.transform));
                                for (int j = 0; j < boardChild.Length; j++)
                                {
                                    if (boardChild[j].preIndexArray != null && boardChild[j].preIndexArray.Length > 0)
                                        elements = elements.Concat(boardChild[j].preIndexArray);
                                }
                                overlapScrewIndex.AddRange(elements);
                            }
                            screwTr.preIndexArray = overlapScrewIndex.Distinct().ToArray();
                            screwTr.preTrArray = overlapScrewIndex.Select(t => tScrewTrans[t]).ToArray();
                        }
                    }
                }
                var boards = layerTr.Cast<Transform>().Select(t => t.GetComponent<Collider2D>());
                cacheCollider.AddRange(boards);
                layerCount++;
            }

            var m = mapRoot.GetComponentsInChildren<ScrewEditorData>();
            for (int i = 0; i < m.Length; i++)
            {
                m[i].selfIndex = i;
                m[i].preCount = m[i].preIndexArray.Length;
            }

            //====
            //int[] unimportant = new int[m.Length];
            //for (int i = 0; i < m.Length; i++)
            //{
            //    if (m[i].preIndex.Length > 0)
            //    {
            //        unimportant[i] += 1;
            //        for (int k = 0; k < m[i].preIndex.Length; k++)
            //        {
            //            unimportant[m[i].preIndex[k]] += 1;
            //        }
            //    }
            //}
            //for (int i = 0; i < unimportant.Length; i++)
            //{
            //    if (unimportant[i] == 0)
            //    {
            //        SetScrewColor(tScrewTrans[i], Color.green);
            //    }
            //}
        }
    }
}
#endif