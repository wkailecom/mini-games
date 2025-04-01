using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MiniGame
{
    public class UIMapGift : MonoBehaviour
    {
        public Transform propRoot;
        public UIPropItem propItem;

        List<UIPropItem> mPropItems = new();
        public void Init(List<PropData> rewards)
        {
            mPropItems.SetItemsActive(rewards.Count, propItem, propRoot);
            for (int i = 0; i < rewards.Count; i++)
            {
                mPropItems[i].SetData(rewards[i]);
            }
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}