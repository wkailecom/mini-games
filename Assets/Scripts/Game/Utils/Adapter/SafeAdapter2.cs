using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 未完善
/// </summary>
public class SafeAdapter2 : MonoBehaviour
{
    public enum Constraint
    {
        [Description("默认")]
        NONE,
        [Description("贴合模式")]
        SNAP,
        [Description("推移模式")]
        PUSH,
        [Description("扩展模式")]
        ENLARGE
    }

    //将想要匹配安全区域的地方设为true。
    [SerializeField] bool left;
    [SerializeField] bool right;
    [SerializeField] bool top;
    [SerializeField] bool bottom;
    [SerializeField] Constraint m_ConstraintType = Constraint.NONE;

    RectTransform m_Panel;
    Vector2 m_DefaultAnchorMin = Vector2.negativeInfinity;
    Vector2 m_DefaultAnchorMax = Vector2.negativeInfinity;

    void Awake()
    {
        m_Panel = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        ApplySafeArea();
    }

    public void Restore()
    {
        if (m_Panel == null) return;

        m_Panel.anchorMin = m_DefaultAnchorMin;
        m_Panel.anchorMax = m_DefaultAnchorMax;
    }

    /// <summary>
    /// 获取安全区域Top距离
    /// </summary>
    /// <returns></returns>
    float GetSafeAreaOfTop()
    {
        //#if UNITY_IOS && !UNITY_EDITOR
        //        return GetSafeAreaInsetsOfTop();
        //#else
        return 0f;
        //#endif
    }

    /// <summary>
    /// 获取安全区域View在Y轴的偏移(IphoneX 安全区需要上移到与刘海齐平)
    /// </summary>
    /// <returns></returns>
    float GetSafeViewYOff()
    {
        //#if UNITY_IOS && !UNITY_EDITOR
        //        return GetSafeViewScaleFactor() * 12;
        //#else
        return 0f;
        //#endif
    }

    void ApplySafeArea()
    {
        var scaler = GetComponentInParent<CanvasScaler>();

        var ORIGINAL_SCREEN_HEIGHT = scaler.referenceResolution.y;
        var ORIGINAL_SCREEN_WIDTH = scaler.referenceResolution.x;
        var aspectRatio = ((float)Screen.height / Screen.width) / ((float)ORIGINAL_SCREEN_HEIGHT / ORIGINAL_SCREEN_WIDTH);
        LogManager.LogError(Screen.width);
        LogManager.LogError(Screen.height);
        LogManager.LogError(scaler.referenceResolution);
        LogManager.LogError(scaler.matchWidthOrHeight); 
        LogManager.LogError(aspectRatio);
        LogManager.LogError(Screen.width/ aspectRatio);

        int width = (int)(scaler.referenceResolution.x * (1 - scaler.matchWidthOrHeight) +
            scaler.referenceResolution.y * Screen.width / Screen.height * scaler.matchWidthOrHeight);
        int height = (int)(scaler.referenceResolution.y * scaler.matchWidthOrHeight -
            scaler.referenceResolution.x * Screen.height / Screen.width * (scaler.matchWidthOrHeight - 1));
        float ratio = scaler.referenceResolution.y * scaler.matchWidthOrHeight / Screen.height -
            scaler.referenceResolution.x * (scaler.matchWidthOrHeight - 1) / Screen.width;

        LogManager.LogError("AAAA "+ width);
        LogManager.LogError("AAAA " + height);
        LogManager.LogError("AAAA " + ratio);

        if (m_DefaultAnchorMin == Vector2.negativeInfinity)
        {
            m_DefaultAnchorMin = m_Panel.anchorMin;
            m_DefaultAnchorMax = m_Panel.anchorMax;
        }

        var area = Screen.safeArea;
        var rectMin = area.position;
        var rectMax = area.position + area.size;

        Vector2 anchorMin = m_DefaultAnchorMin;
        Vector2 anchorMax = m_DefaultAnchorMax;

        //SNAP
        if (m_ConstraintType == Constraint.SNAP)
        {
            anchorMin.x = left ? rectMin.x / Screen.width : 0;
            anchorMax.x = right ? rectMax.x / Screen.width : 1;

            anchorMin.y = bottom ? rectMin.y / Screen.height : 0;
            if (top)
            {
                if (GetSafeAreaOfTop() > 0)
                {
                    anchorMax.y = (rectMax.y + GetSafeViewYOff()) / Screen.height;
                }
                else
                {
                    anchorMax.y = rectMax.y / Screen.height;
                }
            }
            else
            {
                anchorMax.y = 1;
            }
        }
        else if (m_ConstraintType == Constraint.ENLARGE)
        {
            // ENLARGE: 根据每个方向扩大锚点
            if (left) anchorMax.x += rectMin.x / Screen.width;
            if (right) anchorMin.x -= (Screen.width - area.width - rectMin.x) / Screen.width;
            if (bottom) anchorMin.y -= (Screen.height - area.height - rectMin.y) / Screen.height;
            if (top) anchorMax.y += rectMin.y / Screen.height;
        }
        else if (m_ConstraintType == Constraint.PUSH)
        {
            // PUSH: 根据每个方向平移锚点
            if (left)
            {
                float offset = rectMin.x / Screen.width;
                anchorMin.x += offset;
                anchorMax.x += offset;
            }

            if (right)
            {
                float offset = (Screen.width - area.width - rectMin.x) / Screen.width;
                anchorMin.x -= offset;
                anchorMax.x -= offset;
            }

            if (bottom)
            {
                float offset = rectMin.y / Screen.height;
                anchorMin.y += offset;
                anchorMax.y += offset;
            }

            if (top)
            {
                float offset1 = (Screen.height - area.height - rectMin.y) / Screen.height;
                float offset2 = ((Screen.height - area.height - rectMin.y) - GetSafeViewYOff()) / Screen.height;

                float offset = GetSafeAreaOfTop() > 0 ? offset2 : offset1;

                anchorMin.y -= offset;
                anchorMax.y -= offset;
            }
        }

        // 最后赋值回去
        m_Panel.anchorMin = anchorMin;
        m_Panel.anchorMax = anchorMax;
    }

}
