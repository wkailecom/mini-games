using Config; 
using UnityEngine;

public class SafeAdapter : MonoBehaviour
{
    //将想要匹配安全区域的地方设为true。
    [SerializeField] bool left;
    [SerializeField] bool right;
    [SerializeField] bool top = true;
    [SerializeField] bool bottom = true;
    [SerializeField] bool isBanner = true;

    RectTransform panel;

    void Awake()
    {
        panel = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void OnEnable()
    {
        if (isBanner)
        {
            EventManager.Register(EventKey.PropCountChange, OnPropCountChange);
            ApplyBannerAdapter();
        }
    }

    void OnDisable()
    {
        if (isBanner)
        {
            EventManager.Unregister(EventKey.PropCountChange, OnPropCountChange);
        }
    }

    void OnPropCountChange(EventData pEventData)
    {
        var tEventData = pEventData as PropCountChange;
        if (tEventData.propID == PropID.RemoveAD)
        {
            ApplyBannerAdapter();
        }
    }

    void ApplySafeArea()
    {
        var area = Screen.safeArea;

        var anchorMin = area.position;
        var anchorMax = area.position + area.size;

        if (left) anchorMin.x /= Screen.width;
        else anchorMin.x = 0;

        if (right) anchorMax.x /= Screen.width;
        else anchorMax.x = 1;

        if (bottom) anchorMin.y /= Screen.height;
        else anchorMin.y = 0;

        if (top) anchorMax.y /= Screen.height;
        else anchorMax.y = 1;

        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;
    }

    void ApplyBannerAdapter()
    {
        if (GameMethod.HasRemoveAD())
        {
            panel.offsetMin = Vector2.zero;
        }
        else
        {
            panel.offsetMin = new Vector2(0, 150);
        }
    }
}