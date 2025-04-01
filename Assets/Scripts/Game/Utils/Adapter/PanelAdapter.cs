using System.Text;
using UnityEngine;

public class PanelAdapter : MonoBehaviour
{
    public RectTransform TopRect;
    public RectTransform ViewRect;
    void Awake()
    {
        var offsetY = UIRoot.SafeOffset.top;

        TopRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TopRect.rect.height + offsetY);
        ViewRect.offsetMax = new Vector2(ViewRect.offsetMax.x, ViewRect.offsetMax.y - offsetY);
    }
}