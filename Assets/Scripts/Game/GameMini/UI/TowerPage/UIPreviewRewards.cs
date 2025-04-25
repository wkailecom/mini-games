using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPreviewRewards : MonoBehaviour
{
    public RectTransform Layout;
    public UIPropItem RewardItem;

    List<PropData> propDatas = new();
    List<UIPropItem> rewards = new();
    public void SetData(List<PropData> pPropDatas, float pOffsetX = 0)
    {
        propDatas = pPropDatas;
        Layout.anchoredPosition = new Vector2(pOffsetX, Layout.anchoredPosition.y);
        rewards.Clear();
        foreach (Transform child in Layout)
        {
            Destroy(child.gameObject);
        }
        foreach (var tData in pPropDatas)
        {
            var tItem = Instantiate(RewardItem, Layout);
            tItem.SetData(tData);
        }
    }
}
