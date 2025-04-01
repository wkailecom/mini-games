using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

public class PageViewSwitch : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public float smooting = 4;  //滑动速度 
    public float sensitivity = 1;
    public Action<int> OnPageChanged;

    ScrollRect scrollRect;
    RectTransform rectTran;
    List<float> posList = new();//求出每页的临界角，页索引从0开始 

    int curPageIndex = -1;
    float targethorizontal = 0;//滑动的起始坐标 
    float startDragHorizontal;
    float startTime;
    bool isDrag = false;       //是否拖拽结束 
    bool stopMove = true;


    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        rectTran = GetComponent<RectTransform>();
        SetPosList();
    }

    void Update()
    {
        if (!isDrag && !stopMove)
        {
            startTime += Time.deltaTime;
            float t = startTime * smooting;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, targethorizontal, t);
            if (t >= 1)
                stopMove = true;
        }
    }

    void SetPosList()
    {
        posList.Clear();
        posList.Add(0);
        float horizontalLength = scrollRect.content.rect.width - rectTran.rect.width;
        for (int i = 1; i < scrollRect.content.childCount - 1; i++)
        {
            posList.Add(rectTran.rect.width * i / horizontalLength);
        }
        posList.Add(1);
    }

    public void UpdatePosList()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
        SetPosList();
    }

    public void SwitchPageTo(int index, bool isAnim = true)
    {
        if (index >= 0 && index < posList.Count)
        {
            if (isAnim)
            {
                DOTween.To(() => scrollRect.horizontalNormalizedPosition, x => scrollRect.horizontalNormalizedPosition = x, posList[index], 0.5f);
            }
            else
            {
                scrollRect.horizontalNormalizedPosition = posList[index];
            }
            SetPageIndex(index);
        }
        else
        {
            Debug.LogWarning("页码不存在");
        }
    }
    private void SetPageIndex(int index)
    {
        if (curPageIndex != index)
        {
            curPageIndex = index;
            OnPageChanged?.Invoke(index);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
        startDragHorizontal = scrollRect.horizontalNormalizedPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float posX = scrollRect.horizontalNormalizedPosition;
        posX += ((posX - startDragHorizontal) * sensitivity);
        posX = posX < 1 ? posX : 1;
        posX = posX > 0 ? posX : 0;
        int index = 0;
        float offset = Mathf.Abs(posList[index] - posX);
        for (int i = 1; i < posList.Count; i++)
        {
            float temp = Mathf.Abs(posList[i] - posX);
            if (temp < offset)
            {
                index = i;
                offset = temp;
            }
        }
        SetPageIndex(index);

        targethorizontal = posList[index]; //设置当前坐标，更新函数进行插值 
        isDrag = false;
        startTime = 0;
        stopMove = false;
    }
}