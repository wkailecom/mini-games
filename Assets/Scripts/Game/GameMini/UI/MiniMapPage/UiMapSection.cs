using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Game.MiniGame.UILevelItem;

namespace Game.MiniGame
{
    public class UIMapSection : MonoBehaviour
    {
        public static readonly Vector2[] ItemsPos = {
            new(85, 256),
            new(-39, 486),
            new(-144, 714),
            new(109, 940),
            new(-64, 1206),
            new(-33, 1480),
            new(187, 1694),
            new(177, 2003),
            new(-32, 2205),
            new(-191, 2440),
        };

        public RectTransform rect;
        public Transform ItemsRoot;
        [HideInInspector] public List<UILevelItem> levelItems;
        public int index;

        int mBeginLevel;
        int mLevelCount;

        MiniMapPage mMapPage;
        public void Init(MiniMapPage pMapPage)
        {
            mMapPage = pMapPage;
            levelItems = new List<UILevelItem>();
            for (int i = 0; i < ItemsPos.Length; i++)
            {
                var tMapItem = mMapPage.LevelItemPool.GetOne(ItemsRoot);
                tMapItem.transform.localPosition = ItemsPos[i];
                levelItems.Add(tMapItem);
            }
        }

        public void SetData(int pCurLevel, int pIndex)
        {
            index = pIndex;
            var tTotalLevel = mMapPage.TotalLevel;
            for (int i = 0; i < levelItems.Count; i++)
            {
                int tLevel = pIndex * ItemsPos.Length + i + 1;
                if (tLevel <= tTotalLevel)
                {
                    ItemState tState = tLevel <= pCurLevel ? ItemState.Unlock : ItemState.Lock;
                    ItemType tType = ModuleManager.MiniGame.GetLevelConfig(tLevel).HardMark == 1 ? ItemType.Hard : ItemType.Normal;
                    levelItems[i].SetData(pIndex, i, tLevel, tState, tType);
                    levelItems[i].gameObject.SetActive(true);
                }
                else
                {
                    levelItems[i].gameObject.SetActive(false);
                }
            }
        }

    }
}