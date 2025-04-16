using System;
using System.Collections; 
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UITripleTarget : MonoBehaviour
{
    public Image target;
    public Text txtCount;
    public string curItemType = "";
    public Animator targetAnim;

    int allCount;
    TripleMath.MatchItem matchItem;
    SortingGroup itemSorting;
    BoxCollider itemCollider;
    public void Init(string pItemType, int pCount)
    {
        curItemType = pItemType;
        allCount = pCount;

        txtCount.text = pCount.ToString();

        matchItem = ResTool.CreatePrefab<TripleMath.MatchItem>(pItemType, TripleMath.Constant.MATCH_ITEM_PATH, target.transform);
        itemSorting = matchItem.GetComponent<SortingGroup>();
        itemCollider = matchItem.GetComponent<BoxCollider>();

        if (itemSorting == null)
        {
            itemSorting = matchItem.gameObject.AddComponent<SortingGroup>();
        }
        itemSorting.sortingOrder = 15;
        int layer = LayerMask.NameToLayer("UI");
        SetLayerRecursive(matchItem.transform, layer);
        matchItem.transform.SetParent(target.transform);
        matchItem.transform.localPosition = Vector3.zero;
        var targetScale = 1f;
        var width = target.GetComponent<RectTransform>().rect.width;
        var height = target.GetComponent<RectTransform>().rect.height;
        targetScale = Math.Min(width, height) / Math.Max(itemCollider.size.x, itemCollider.size.y);
        matchItem.transform.localScale = Vector3.one * targetScale;
        gameObject.SetActive(true);
    }

    private void SetLayerRecursive(Transform obj, int layer)
    {
        obj.gameObject.layer = layer;

        // 遍历所有子物体
        foreach (Transform child in obj)
        {
            SetLayerRecursive(child, layer);
        }
    }
    public void SetMathItemTypeAndCount(string mathchItemType, int count, bool isAndAni = false)
    {
        targetAnim.Play("TripleMathTarget_HuoDe", 0, 0);
        if (isAndAni)
        {
            SetTypeAndCount(mathchItemType, count);
        }
        else
        {
            SetTypeAndCount(mathchItemType, count);
        }
        CheckIsFinish();
    }

    public void CheckIsFinish()
    {
        if (allCount == 0)
        {
            Clear();
            gameObject.SetActive(false);
        }
    }

    private void SetTypeAndCount(string pItemType, int pCount)
    {
        if (curItemType == pItemType)
        {
            allCount += pCount;
            txtCount.text = allCount.ToString();
        }
    }

    public void Clear()
    {
        if (curItemType != "")
        {
            curItemType = "";
            txtCount.text = "";
            if (matchItem != null)
            {
                Destroy(matchItem.gameObject);
                matchItem = null;
            }
        }
    }
}
