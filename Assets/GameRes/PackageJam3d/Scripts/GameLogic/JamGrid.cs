using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

namespace GameLogic
{
    
    [Serializable]
    public class SwapSetting
    {
        [Min(1)] public int swapStep = 1;
        [Min(1)] public int swapTimer = 1;
    }
    
    public class JamGrid : AStarGrid
    {
        public Transform elementsParent;
        public Transform terrainParent;
        public Transform blockerParent;
        private static readonly Color[] Colors = {
            new Color(0f, 0.71f, 0.82f),new Color(0.77f, 0.45f, 0.02f), new Color(0.83f, 0.14f, 0.14f),
            new Color(0f, 0.77f, 0f), new Color(0.3f, 0.55f, 0.5f), new Color(0.74f, 0.21f, 0.87f),
            new Color(0.52f,0.22f,0.22f), new Color(0.33f,0.27f,0.90f), new Color(0.55f,0.55f,0.55f),
            new Color(0.65f, 0.62f, 0f)
        };
        private int totalCapacity;
        public int uniqueColorsCount;

        private Dictionary<int, Blocker> blockers;
        [HideInInspector] public SwapSetting swapSetting;
        private Dictionary<int, TileItem> tileItems;

        public int GridItemCount => tileItems.Count;

        public int GridGroupCount => totalCapacity / 3;

        private bool onShuffle;

        private int minCol;
        private int maxCol;
        private int minRow;
        private static readonly int BaseColor = Shader.PropertyToID("_Color");
        public bool hasFake;
        public bool hasKey;
        public bool hasSpawn;
        public bool hasPin;

        public string GetLevelElementInfo()
        {
            var sb = new StringBuilder();
            var elementTypeList = new List<int>();
            if (hasFake)
            {
                elementTypeList.Add(1);
            }

            if (hasKey)
            {
                elementTypeList.Add(4);
            }

            if (hasSpawn)
            {
                elementTypeList.Add(2);
            }

            if (hasPin)
            {
                elementTypeList.Add(3);
            }

            elementTypeList.Sort();

            for (int i = 0; i < elementTypeList.Count; i++)
            {
                sb.Append(elementTypeList[i]);
                if (i < elementTypeList.Count - 1)
                    sb.Append("_");
            }

            return sb.ToString();
        }

        #region Editor Function

        public void ParseSwapSetting(string setting)
        {
            var swapStr = setting.Split(',');
            swapSetting = new SwapSetting
            {
                swapTimer = int.Parse(swapStr[0]),
                swapStep = int.Parse(swapStr[1])
            };
        }

        [ContextMenu("Assign Color")]
        public void TestAssignColor(int colorsCount)
        {
            AssignTileItem();

            if (totalCapacity % 3 != 0)
            {
                Debug.LogError("item count error");
                return;
            }

            RandomColors(colorsCount);
        }

        #endregion

        private void AssignTileItem()
        {
            minCol = int.MaxValue;
            maxCol = 0;
            minRow = gridSize;

            tileItems = new Dictionary<int, TileItem>();
            if(elementsParent == null)
                return;
            var items = elementsParent.GetComponentsInChildren<TileItem>();
            foreach (var item in items)
            {
                var index = item.index;

                if (index % gridSize < minCol)
                {
                    minCol = index % gridSize;
                }

                if (index % gridSize > maxCol)
                {
                    maxCol = index % gridSize;
                }

                var row = index / gridSize;
                if (row < minRow)
                {
                    minRow = row;
                }

                for (var c = 0; c < item.capacity; c++)
                {
                    totalCapacity++;
                }
                
                if(walkableTiles.ContainsKey(item.index))
                    walkableTiles[item.index].FillInItem();

                item.VirusState = VirusState.Born;
                tileItems.Add(item.index, item);
            }
        }

        public TileItem GetTileItem(int index)
        {
            return tileItems.ContainsKey(index) ? tileItems[index] : null;
        }


        /// <summary>
        /// construct map data
        /// </summary>
        public void Construct(GridConfig gridConfig)
        {
            uniqueColorsCount = gridConfig.uniqueColorsCount;

            SetUpTerrains(gridConfig);
            AssignTileItem();
            SetUpBlocker();

            //todo-1 : up flow
            SetupGridBlockState();
            RandomColors(uniqueColorsCount);
        }

        private void SetUpTerrains(GridConfig gridConfig)
        {
            walkableTiles = new SerializedDictionary<int, TerrainTile>();
            foreach (var info in gridConfig.cellInfo)
            {
                var blockName = info.walkable ? "dirt_block" : "grass_block";

                var go = Pool.ObjectPool.Instance.GetPooledObject(blockName);
                if (go == null)
                {
                    Debug.LogError($"terrain name error : {blockName}");
                    continue;
                }
                var c = go.GetComponent<TerrainTile>();
                c.Init(info.index,terrainParent,TileIndexToPosition(info.index));
                c.walkable = info.walkable;
                if (!info.walkable) continue;
                walkableTiles.Add(info.index, c);
            }
        }

        private void SetUpBlocker()
        {
            blockers = new Dictionary<int, Blocker>();
            var bs = blockerParent.GetComponentsInChildren<Blocker>();
            foreach (var block in bs)
            {
                blockers.Add(block.index, block);
                block.ShowBlockerEffect();
            }
        }

        /// <summary>
        /// setup grid left,right,top,bottom neighbors
        /// </summary>
        private void SetupGridBlockState()
        {
            foreach (var terrainTile in walkableTiles)
            {
                var tile = terrainTile.Value;
                SetupGridNeighbors(tile);

                var pathList = FindPath(tile.index);
                var row = tile.index / gridSize;

                if (row != gridSize - 1)
                {
                    if (tile.walkable && pathList == null)
                    {
                        tile.walkable = false;
                    }
                }
                else
                {
                    tile.walkable = true;
                }

                if (tile.tileItem != 0)
                {
                    var item = GetTileItem(tile.tileItem);
                    switch (item.tileItemType)
                    {
                        case TileItemType.Virus:
                            if (pathList != null)
                                item.VirusState = VirusState.Idle;
                            break;
                        case TileItemType.Spawn:
                            if (tile.index != item.index)
                                item.VirusState = VirusState.Idle;
                            break;
                        case TileItemType.Fake:
                            item.VirusState = VirusState.Fake;
                            break;
                    }
                }
            }
        }

        private void SetupGridNeighbors(TerrainTile tile)
        {
            tile.ClearData();
            var index = tile.index;
            var adjacent = GetLeftGridSpot(index);
            if (adjacent)
            {
                tile.AddAdjacentTile(adjacent);
            }

            adjacent = GetRightGridSpot(index);
            if (adjacent)
            {
                tile.AddAdjacentTile(adjacent);
            }

            adjacent = GetTopGridSpot(index);
            if (adjacent)
            {
                tile.AddAdjacentTile(adjacent);
            }

            adjacent = GetBottomGridSpot(index);
            if (adjacent)
            {
                tile.AddAdjacentTile(adjacent);
            }
        }

        private void RandomColors(int colorNum, bool demote = false)
        {
            var colorEnum = 1;
            var assignCount = 0;
            var colors = new List<int>();


            for (var i = 0; i < totalCapacity; i++)
            {
                if (colorEnum > colorNum)
                {
                    colorEnum = 1;
                }

                assignCount++;
                colors.Add(colorEnum - 1);
                if (assignCount % 3 == 0)
                    colorEnum++;
            }


            for (var i = 0; i < colors.Count; i++)
            {
                var targetIndex = i;

                for (var j = 0; j < swapSetting.swapTimer; j++)
                {
                    if (targetIndex >= colors.Count)
                        break;
                    var rm = Random.Range(0, swapSetting.swapStep);
                    targetIndex += rm;
                }


                targetIndex = Mathf.Min(targetIndex, colors.Count - 1);
                var itemColor = colors[i];
                var targetColor = colors[targetIndex];
                colors[i] = targetColor;
                colors[targetIndex] = itemColor;
            }

            assignCount = 0;
            foreach (var tileItem in tileItems)
            {
                var item = tileItem.Value;
                if (item is KeyItem keyItem)
                {
                    var lockItem = FindKeyLock(keyItem.keysColor);
                    keyItem.lockItem = lockItem;
                    lockItem.keyItem = keyItem;
                }

                for (var j = 0; j < item.capacity; j++)
                {
                    item.AssignColor(colors[assignCount], j);
                    assignCount++;
                }

                if (demote && item is VirusItem virusItem)
                {
                    //virusItem.PlayDemoteEffect();
                }
            }
        }


        private LockItem FindKeyLock(KeysColor color)
        {
            foreach (var item in tileItems)
            {
                if (!(item.Value is LockItem lockItem)) continue;
                var lockItemKeysColor = lockItem.keysColor;
                if (lockItemKeysColor == color)
                {
                    return lockItem;
                }
            }

            return null;
        }

        /// <summary>
        /// adjust camera position
        /// </summary>
        /// <returns></returns>
        public float GetOffsetX()
        {
            var maxPos = JamManager.GetSingleton().TileIndexToPosition(maxCol);
            var minPos = JamManager.GetSingleton().TileIndexToPosition(minCol);
            return (maxPos.x - minPos.x) * 0.5f + minPos.x;
        }

        /// <summary>
        /// adjust camera position
        /// </summary>
        /// <returns></returns>
        public float GetRowCount()
        {
            return minRow;
        }


        public void ReBron()
        {
            onShuffle = false;
            ClearGridSpotsOpen();
            RandomColors(uniqueColorsCount);
        }

        private float GetMin()
        {
            return -gridSize * 0.5f + 0.5f;
        }

        private float GetMax()
        {
            return gridSize * 0.5f - 0.5f;
        }

        public int TilePositionToIndex(Vector3 position)
        {
            if (position.x < GetMin() || position.x > GetMax() || position.y < GetMin() || position.y > GetMax())
                return -1;
            var col = position.x - GetMin();
            var row = (GetMax() - position.z) * gridSize;
            return (int)(row + col);
        }

        public Vector3 TileIndexToPosition(int index)
        {
            var x = index % gridSize;
            var y = index / gridSize;
            var ret = new Vector3(x + GetMin(), 0, GetMax() - y);
            return ret;
        }

        // /// <summary>
        // /// get grid spot by index,used by editor 
        // /// </summary>
        // /// <param name="index"></param>
        // /// <returns></returns>
        // public TerrainTile GetGridSpot(int index)
        // {
        //     return index >= WalkableTiles.Length || index <= 0 ? null : WalkableTiles[index];
        // }


        public void Unblock(int index)
        {
            var tile = GetGridSpot(index);
            tile.ClearGridItem();
        }

        private void ClearGridSpotsOpen()
        {
            foreach (var t in tileItems)
            {
                var item = t.Value;
                var tile = GetGridSpot(item.index);
                tile.walkable = false;
                tile.isBlock = true;
                item.VirusState = VirusState.Reborn;
            }
        }

        public TerrainTile GetLeftGridSpot(int index)
        {
            var leftIndex = index - 1;
            return GetGridSpot(leftIndex);
        }

        public TerrainTile GetRightGridSpot(int index)
        {
            var leftIndex = index + 1;
            return GetGridSpot(leftIndex);
        }

        public TerrainTile GetTopGridSpot(int index)
        {
            var topIndex = index - gridSize;
            return GetGridSpot(topIndex);
        }

        public TerrainTile GetBottomGridSpot(int index)
        {
            var bottomIndex = index + gridSize;
            return GetGridSpot(bottomIndex);
        }

        public bool AllBlocksRemoved()
        {
            foreach (var terrainTile in walkableTiles)
            {
                var tileItem = terrainTile.Value.tileItem;
                if (tileItem == 0) continue;
                var item = GetTileItem(tileItem);
                if (item.VirusState != VirusState.Dead)
                {
                    return false;
                }
            }

            return true;
        }

        public bool Shuffle()
        {
            if (onShuffle)
                return false;
            var tileItemList = new List<TileItem>();
            foreach (var tileItem in tileItems)
            {
                var i = tileItem.Value;
                if ((i.VirusState is VirusState.Idle) || (i.VirusState is VirusState.Born)
                    && (i is VirusItem && !(i is FakeItem)) && !GetBlocker(i.index))
                {
                    tileItemList.Add(i);
                }
            }

            if (tileItemList.Count <= 1)
                return false;

            onShuffle = true;
            //var count = tileItemList.Count;
            while (tileItemList.Count > 0)
            {
                if (tileItemList.Count == 1)
                {
                    tileItemList[0].VirusState = VirusState.Shuffle;
                    break;
                }

                var index = Random.Range(1, tileItemList.Count);
                var indexItem = tileItemList[index];
                var firstItem = tileItemList[0];

                // var firstTile = GetGridSpot(firstItem.index);
                // var indexTile = GetGridSpot(indexItem.index);
                // firstTile.tileItem = indexItem.index;
                // indexTile.tileItem = firstItem.index;
                (tileItems[firstItem.index], tileItems[indexItem.index]) = (tileItems[indexItem.index], tileItems[firstItem.index]);
                (firstItem.index, indexItem.index) = (indexItem.index, firstItem.index);
                firstItem.VirusState = VirusState.Shuffle;
                indexItem.VirusState = VirusState.Shuffle;

                tileItemList.RemoveAt(index);
                tileItemList.RemoveAt(0);
            }

            // tileItems.Sort((item, tileItem) => item.index < tileItem.index ? 1 : -1);
            // var colors = new int[tileItems.Count];
            // for (var i = 0; i < tileItems.Count; i++)
            // {
            //     var tileItem = tileItems[i];
            //     var color = tileItem.virusColor;
            //     for (var j = 0; j < Colors.Length; j++)
            //     {
            //         if (color == Colors[j])
            //         {
            //             colors[i] = j;
            //         }
            //     }
            // }
            //
            // var json = JsonConvert.SerializeObject(colors);
            //
            // XLuaKit.CallLua("ReceiveGameBehaviour", (int)BehaviourType.click_props, (int)PropsType.Shuffle,
            //     json);


            Invoke(nameof(EndShuffling), 1.5f);
            return true;
        }

        public VirusItem PullColorItem(int color)
        {
            // var tileItemList = (from tile in walkableTiles
            //     where tile && tile.Value.tileItem && tile.Value.tileItem.VirusState is VirusState.Idle or VirusState.Born &&
            //           tile.Value.tileItem is VirusItem
            //     select tile.Value.tileItem).ToList();
            //
            // if (tileItemList.Count <= 0)
            //     return null;
            // var ret = tileItemList[0];
            // foreach (var item in tileItemList.Where(item => item.virusColor == color && item.index > ret.index))
            // {
            //     ret = item;
            // }
            //
            // var virusItem = ret as VirusItem;
            // if (virusItem == null) return virusItem;
            // var tt = JamManager.GetSingleton().GetTerrainTile(virusItem.index);
            // tt.TryUnlock();
            // return virusItem;
            return null;
        }

        private void EndShuffling()
        {
            onShuffle = false;
        }

        public bool IsKeyUnlockingLock()
        {
            return false;
        }

        public bool CanAddBlocks()
        {
            return true;
        }

        private void SwapForcedColorBlocks(int count, int range)
        {
        }

        private void OnDisable()
        {
        }

        public bool IsBlocker(int index)
        {
            if (blockers.TryGetValue(index, out var blocker))
            {
                return !blocker.CanMove();
            }

            return false;
        }

        public Blocker GetBlocker(int index)
        {
            foreach (var blocker in blockers)
            {
                if (blocker.Key == index)
                {
                    return blocker.Value.blood > 0 ? blocker.Value : null;
                }

                if (!(blocker.Value is TwinsBlocker twinsBlocker)) continue;
                var affectIndex = twinsBlocker.index;
                switch (twinsBlocker.direction)
                {
                    case TwinsDirection.Horizontal:
                        affectIndex += 1;
                        break;
                    case TwinsDirection.Vertical:
                        affectIndex += gridSize;
                        break;
                }

                if (affectIndex != index || twinsBlocker.blood <= 0) continue;
                twinsBlocker.ClickOther();
                return blocker.Value;
            }

            return null;
        }

        public List<Blocker> GetBlockersByType(BlockerType blockerType)
        {
            return (from blockers in blockers where blockers.Value.blockerType == blockerType select blockers.Value)
                .ToList();
        }
    }
}