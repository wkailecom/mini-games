using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// terrain object
    /// </summary>
    public class TerrainTile : TileBase
    {
        /// <summary>
        /// allow this prefab to be placed inside other prefabs
        /// </summary>
        public bool allowOverlap = true;

        /// <summary>
        /// 格子元素
        /// </summary>
        public int tileItem;

        public Blocker blocker;

        public bool isBlock = true;

        public int affectItem;

        private readonly List<TerrainTile> _adjacentList = new List<TerrainTile>();

        public void ClearData()
        {
            _adjacentList.Clear();
        }

        public void AddAdjacentTile(TerrainTile tile)
        {
            if (_adjacentList.Any(t => t.index == tile.index))
            {
                Debug.LogError($"tile {index} adjacent has same index {tile.index}");
                return;
            }

            _adjacentList.Add(tile);
            isBlock = true;
        }

        public void OnPreUnlock(bool isInvoke)
        {
            if (isInvoke)
            {
                Invoke(nameof(DoNext), 1);
            }
            else
            {
                DoNext();
            }
        }

        private void DoNext()
        {
            if (affectItem == 0) return;
            var item = JamManager.GetSingleton().GetTileItem(affectItem);
            if (!(item is SpawnItem spawnItem)) return;
            spawnItem.DONext(false, true);
        }

        private void ShowTwinsEffect()
        {
            if (tileItem == 0)
                return;
            ClearGridItem();
        }

        /// <summary>
        /// on start navigation
        /// </summary>
        public void ClearGridItem(bool direct = true)
        {
            if (affectItem != 0)
            {
                var item = JamManager.GetSingleton().GetTileItem(affectItem);
                if (item is SpawnItem spawnItem)
                {
                    if (spawnItem.DONext())
                    {
                        isBlock = true;
                        walkable = false;
                        return;
                    }
                }
            }

            isBlock = false;
            walkable = true;

            AdjacentTileOpen(direct);
        }

        /// <summary>
        /// set affect 
        /// </summary>
        private void AdjacentTileOpen(bool direct)
        {
            foreach (var tile in _adjacentList)
            {
                tile.UnlockTileItem(direct);
            }
        }

        public SpawnItem GetAdjacentSpawn()
        {
            foreach (var tile in _adjacentList)
            {
                if (tile.tileItem == 0) continue;
                var item = JamManager.GetSingleton().GetTileItem(tile.tileItem);
                if (item is SpawnItem spawnItem)
                    return spawnItem;
            }

            return null;
        }

        /// <summary>
        /// on adjacent tile start navigation
        /// </summary>
        private void UnlockTileItem(bool direct)
        {
            if (walkable)
                return;
            if (tileItem != 0)
            {
                var item = JamManager.GetSingleton().GetTileItem(tileItem);

                if ((item is FakeItem || item is KeyItem) && !direct)
                {
                    return;
                }

                item.Unlock();
                if (!isBlock)
                {
                    ClearGridItem(false);
                }
            }
            else
            {
                ClearGridItem(false);
            }
        }

        /// <summary>
        /// undo operate
        /// </summary>
        public void AssignChildGridItem()
        {
            walkable = false;
            AdjacentSpotClose();
            if (tileItem != 0)
            {
                isBlock = true;
                var item = JamManager.GetSingleton().GetTileItem(tileItem);
                item.Undo();
            }

            if (affectItem != 0)
            {
                var item = JamManager.GetSingleton().GetTileItem(affectItem);
                item.Undo();
            }
        }

        public void AdjacentSpotClose()
        {
            foreach (var tile in _adjacentList)
            {
                tile.AdjacentTileClose();
            }
        }

        private void AdjacentTileClose()
        {
            if (tileItem == 0)
                return;
            var item = JamManager.GetSingleton().GetTileItem(tileItem);
            if (!item || (item.VirusState != VirusState.Born && item.VirusState != VirusState.Idle))
                return;
            if (item is FakeItem)
            {
                item.VirusState = VirusState.Born;
                return;
            }

            var pathList = JamManager.GetSingleton().StartNavigation(index);
            var state = pathList != null ? VirusState.Idle : VirusState.Born;
            if (state != item.VirusState)
                item.VirusState = state;
        }

        public void TryUnlock()
        {
            isBlock = false;
            if (HasPath())
            {
                UnlockTileItem(true);
            }
        }

        private bool HasPath()
        {
            return _adjacentList.Any(tile => tile.walkable);
        }

        public void FillInItem()
        {
            tileItem = index;
            walkable = false;
        }

        public void Init(int infoIndex, Transform terrainParent, Vector3 tileIndexToPosition)
        {
            index = infoIndex;
            tileItem = 0;
            Transform transform1;
            (transform1 = transform).SetParent(terrainParent);
            transform1.localPosition = tileIndexToPosition;
        }
    }
}