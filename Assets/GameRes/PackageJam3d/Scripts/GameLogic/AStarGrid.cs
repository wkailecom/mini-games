using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;

namespace GameLogic
{
    public class PathNode
    {
        public readonly int TileIndex;
        public PathNode LastNode;
        public int GPoint;
        public int HPoint;
        public int FPoint;

        public PathNode(int tileIndex)
        {
            TileIndex = tileIndex;
        }
    }


    public class AStarGrid : MonoBehaviour
    {
        public Dictionary<int, TerrainTile> walkableTiles;
        
        [HideInInspector] public int gridSize = 16;

        private bool IsEndPoint(int index)
        {
            var row = index / gridSize;
            return row == gridSize - 1;
        }

        public List<int> FindPath(int startTileIndex)
        {
            var path = new List<int>();

            if (IsEndPoint(startTileIndex))
            {
                path.Add(startTileIndex);
                return path;
            }

            PathNode endNode = null;

            var finishedList = new List<int>();
            var openList = new List<PathNode>();
            var startNode = new PathNode(startTileIndex);
            openList.Add(startNode);


            while (openList.Count > 0)
            {
                var minIndex = GetOpenListNodeIndexHasMinFPoint(openList);
                var node = openList[minIndex];
                finishedList.Add(node.TileIndex);
                openList.RemoveAt(minIndex);
                if (IsEndPoint(node.TileIndex))
                {
                    endNode = node;
                    break;
                }

                var newNode = GetUpNode(node);
                CheckAndInsertNewNodeToOpenList(openList, finishedList, node, newNode, 1);

                newNode = GetLeftNode(node);
                CheckAndInsertNewNodeToOpenList(openList, finishedList, node, newNode, 1);

                newNode = GetDownNode(node);
                CheckAndInsertNewNodeToOpenList(openList, finishedList, node, newNode, 1);

                newNode = GetRightNode(node);
                CheckAndInsertNewNodeToOpenList(openList, finishedList, node, newNode, 1);
            }

            if (endNode == null) return null;
            var lastNode = endNode;
            while (lastNode != null)
            {
                path.Add(lastNode.TileIndex);
                lastNode = lastNode.LastNode;
            }

            if (path.Count <= 0) return null;
            path.Reverse();
            return path;
        }


        private PathNode GetUpNode(PathNode node)
        {
            var tileIndex = node.TileIndex - gridSize;
            var tile = GetGridSpot(tileIndex);
            if (!tile || !tile.walkable)
            {
                return null;
            }

            var newNode = new PathNode(tileIndex)
            {
                GPoint = node.GPoint + 1, HPoint = (int)GetHPoint(node.TileIndex)
            };
            newNode.FPoint = newNode.GPoint + newNode.HPoint;
            newNode.LastNode = node;
            return newNode;
        }

        private PathNode GetLeftNode(PathNode node)
        {
            var tileIndex = node.TileIndex - 1;
            var tile = GetGridSpot(tileIndex);
            if (!tile || !tile.walkable)
            {
                return null;
            }

            var newNode = new PathNode(tileIndex)
            {
                GPoint = node.GPoint + 1, HPoint = (int)GetHPoint(node.TileIndex)
            };
            newNode.FPoint = newNode.GPoint + newNode.HPoint;
            newNode.LastNode = node;
            return newNode;
        }

        private PathNode GetDownNode(PathNode node)
        {
            var tileIndex = node.TileIndex + gridSize;
            var tile = GetGridSpot(tileIndex);
            if (!tile || !tile.walkable)
            {
                return null;
            }

            var newNode = new PathNode(tileIndex)
            {
                GPoint = node.GPoint + 1, HPoint = (int)GetHPoint(node.TileIndex)
            };
            newNode.FPoint = newNode.GPoint + newNode.HPoint;
            newNode.LastNode = node;
            return newNode;
        }

        private PathNode GetRightNode(PathNode node)
        {
            var tileIndex = node.TileIndex + 1;
            var tile = GetGridSpot(tileIndex);
            if (!tile || !tile.walkable)
            {
                return null;
            }

            var newNode = new PathNode(tileIndex)
            {
                GPoint = node.GPoint + 1, HPoint = (int)GetHPoint(node.TileIndex)
            };
            newNode.FPoint = newNode.GPoint + newNode.HPoint;
            newNode.LastNode = node;
            return newNode;
        }

        private void CheckAndInsertNewNodeToOpenList(ICollection<PathNode> openList, List<int> finishedList,
            PathNode node,
            PathNode newNode, int gPoint)
        {
            if (newNode == null)
            {
                return;
            }

            if (IsOpenListExistNode(openList, newNode))
            {
                newNode = GetOpenListNodeByNodePosition(openList, newNode.TileIndex);
                if (node.GPoint + gPoint >= newNode.GPoint) return;
                newNode.GPoint = node.GPoint + gPoint;
                newNode.FPoint = newNode.GPoint + newNode.HPoint;
                newNode.LastNode = node;
            }
            else
            {
                var tile = GetGridSpot(newNode.TileIndex);
                if (tile && tile.walkable && !IsFinishedListExistNode(finishedList, newNode))
                {
                    openList.Add(newNode);
                }
            }
        }
        
        /// <summary>
        /// get grid spot by index,used by editor 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TerrainTile GetGridSpot(int index)
        {
            return walkableTiles.ContainsKey(index) ? walkableTiles[index] : null;
        }
        

        //漏网之鱼
        private static bool IsFinishedListExistNode(IEnumerable<int> finishedList, PathNode newNode)
        {
            return finishedList.Any(nodePosition => nodePosition == newNode.TileIndex);
        }

        private static bool IsOpenListExistNode(IEnumerable<PathNode> openList, PathNode newNode)
        {
            return openList.Any(node => node.TileIndex == newNode.TileIndex);
        }

        private static PathNode GetOpenListNodeByNodePosition(IEnumerable<PathNode> openList, int nodePosition)
        {
            return openList.FirstOrDefault(node => node.TileIndex == nodePosition);
        }

        private static int GetOpenListNodeIndexHasMinFPoint(IReadOnlyList<PathNode> openList)
        {
            if (openList.Count <= 0)
            {
                return 0;
            }

            var minIndex = 0;
            var minFPoint = openList[0].FPoint;

            for (var i = 0; i < openList.Count; i++)
            {
                var node = openList[i];
                if (node.FPoint >= minFPoint) continue;
                minFPoint = node.FPoint;
                minIndex = i;
            }

            return minIndex;
        }

        private float GetHPoint(int index)
        {
            var row = index / gridSize;
            return (gridSize - row) * gridSize;
        }
    }
}