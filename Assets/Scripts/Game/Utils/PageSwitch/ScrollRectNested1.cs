using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 解决ScrollRect和拖拽事件冲突问题
/// </summary>
public class ScrollRectNested1 : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    const bool isUpAndDown = true; 
    ScrollRect scrollRect; 
    PageSwitch pageSwitch;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        pageSwitch = GetComponentInParent<PageSwitch>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        float angle = Vector2.Angle(eventData.delta, Vector2.up); //拖动方向和up方向的夹角
        //根据夹角判断启用哪一个Scrollview
        if (angle > 45 && angle < 135)
        {
            scrollRect.enabled = !isUpAndDown;
            pageSwitch?.OnBeginDrag(eventData);
        }
        else
        {
            scrollRect.enabled = isUpAndDown;
        }
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        if (!scrollRect.enabled)
        {
            pageSwitch?.OnEndDrag(eventData);
        }
        scrollRect.enabled = true;
    }
}