using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// ���ScrollRect����ק�¼���ͻ����
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
        float angle = Vector2.Angle(eventData.delta, Vector2.up); //�϶������up����ļн�
        //���ݼн��ж�������һ��Scrollview
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