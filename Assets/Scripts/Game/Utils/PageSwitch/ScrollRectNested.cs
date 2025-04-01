using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 解决两个ScrollRect嵌套问题
/// </summary>
public class ScrollRectNested : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    const bool isUpAndDown = true; //子框的滑动方向
    ScrollRect parentScrollRect; //父框
    ScrollRect scrollRect;       //子框
    PageViewSwitch pageViewSwitch;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        parentScrollRect ??= scrollRect.GetComponentsInParent<ScrollRect>()[1]; //查找父节点的Scrollview
        pageViewSwitch ??= parentScrollRect.GetComponent<PageViewSwitch>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentScrollRect.OnBeginDrag(eventData);
        float angle = Vector2.Angle(eventData.delta, Vector2.up); //拖动方向和up方向的夹角
        //根据夹角判断启用哪一个Scrollview
        if (angle > 45 && angle < 135)
        {
            scrollRect.enabled = !isUpAndDown;
            parentScrollRect.enabled = isUpAndDown;
        }
        else
        {
            scrollRect.enabled = isUpAndDown;
            parentScrollRect.enabled = !isUpAndDown;
        }

        if (parentScrollRect.enabled)
        {
            pageViewSwitch?.OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        parentScrollRect.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (parentScrollRect.enabled)
        {
            pageViewSwitch?.OnEndDrag(eventData);
        }
        //结束拖动,需要将2个滑动框都启用
        parentScrollRect.OnEndDrag(eventData);
        scrollRect.enabled = true;
        parentScrollRect.enabled = true;
    }
}