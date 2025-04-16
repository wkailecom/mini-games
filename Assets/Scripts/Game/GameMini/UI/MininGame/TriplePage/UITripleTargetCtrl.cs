using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITripleTargetCtrl : MonoBehaviour
{
    public UITripleTarget uiItem;
    List<UITripleTarget> itemList;
    SortedDictionary<string, UITripleTarget> itemDic;

    public void Init()
    {
        itemList = new List<UITripleTarget>();
        itemDic = new SortedDictionary<string, UITripleTarget>();
        uiItem.gameObject.SetActive(false);
    }

    public void InitTarget(SortedDictionary<string, int> pTargetDic)
    {
        itemList.SetItemsActive(pTargetDic.Count, uiItem, transform);
        var index = 0;
        itemDic.Clear();
        foreach (var item in pTargetDic)
        {
            itemList[index].Init(item.Key, item.Value);
            itemDic.Add(item.Key, itemList[index]);
            index++;
        }
    }

    public void RefershTarget(string pItemType, int pCount)
    {
        if (itemDic.ContainsKey(pItemType))
        {
            itemDic[pItemType].SetMathItemTypeAndCount(pItemType, pCount);
        }
    }

}
