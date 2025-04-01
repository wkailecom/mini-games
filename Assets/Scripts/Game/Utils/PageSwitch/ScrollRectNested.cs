using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// �������ScrollRectǶ������
/// </summary>
public class ScrollRectNested : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    const bool isUpAndDown = true; //�ӿ�Ļ�������
    ScrollRect parentScrollRect; //����
    ScrollRect scrollRect;       //�ӿ�
    PageViewSwitch pageViewSwitch;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        parentScrollRect ??= scrollRect.GetComponentsInParent<ScrollRect>()[1]; //���Ҹ��ڵ��Scrollview
        pageViewSwitch ??= parentScrollRect.GetComponent<PageViewSwitch>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentScrollRect.OnBeginDrag(eventData);
        float angle = Vector2.Angle(eventData.delta, Vector2.up); //�϶������up����ļн�
        //���ݼн��ж�������һ��Scrollview
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
        //�����϶�,��Ҫ��2������������
        parentScrollRect.OnEndDrag(eventData);
        scrollRect.enabled = true;
        parentScrollRect.enabled = true;
    }
}