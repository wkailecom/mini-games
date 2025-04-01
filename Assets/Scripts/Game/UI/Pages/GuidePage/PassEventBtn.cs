using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using Spine;

public class PassEventBtn : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{

    public void OnPointerDown(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.pointerDownHandler);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.pointerUpHandler);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.pointerClickHandler);
    }


    private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();
    //把事件透下去
    public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
    {
        _raycastResults.Clear();
        EventSystem.current.RaycastAll(data, _raycastResults);
        GameObject current = data.pointerCurrentRaycast.gameObject;
        foreach (var result in _raycastResults)
        {
            if (current != result.gameObject)
            {
                ExecuteEvents.Execute(result.gameObject, data, function);
                //ExecuteEvents.ExecuteHierarchy(result.gameObject, data, function);//包含父子
                //RaycastAll后ugui会自己排序，如果你只想响应透下去的最近的一个响应，这里ExecuteEvents.Execute后直接break就行。
                break;
            }
        }
    }

}