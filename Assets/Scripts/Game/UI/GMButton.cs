using Game.UISystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class GMButton : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{

    Vector2 offsetPos;  //临时记录点击点与UI的相对位置
    bool isDrag;

    public void OnDrag(PointerEventData eventData)
    {
        isDrag = true;
        transform.localPosition = eventData.position - offsetPos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        offsetPos = eventData.position - (Vector2)transform.localPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDrag)
        {
            isDrag = false;
        }
        else
        {
            PageManager.Instance.OpenPage(Config.PageID.GMPage);
        }   
    }
}