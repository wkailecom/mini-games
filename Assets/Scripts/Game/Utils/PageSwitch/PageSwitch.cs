using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageSwitch : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform Content;
    public Action<int, bool> OnPageChanged;

    int curPageIndex = -1;
    float startX;
    float animTime = 0.5f;

    void Awake()
    {
        
    }

    public void SwitchPageTo(int index, bool isAnim = true)
    {
        if (index >= 0 && index < Content.childCount)
        {
            if (curPageIndex != index || !isAnim)
            {
                curPageIndex = index;
                OnPageChanged?.Invoke(index, isAnim);
            }

            if (isAnim)
            {
                Content.DOLocalMoveX(-Content.GetChild(index).localPosition.x, animTime);
            }
            else
            {
                Content.localPosition = new Vector3(-Content.GetChild(index).localPosition.x, Content.localPosition.y, Content.localPosition.z);
            }

        }
        else
        {
            Debug.LogWarning("页码不存在");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startX = eventData.position.x;
    }


    public void OnDrag(PointerEventData eventData)
    {
        // parentScrollRect.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if ((eventData.position.x - startX) > 0)
        {
            SwitchPageTo(curPageIndex - 1);
        }
        else
        {
            SwitchPageTo(curPageIndex + 1);
        }
    }
}