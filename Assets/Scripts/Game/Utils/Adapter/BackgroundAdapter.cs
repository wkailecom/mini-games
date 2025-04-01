using UnityEngine;


public class BackgroundAdapter : MonoBehaviour
{
    void Awake()
    {
        float tHeightScale = (float)Screen.height / UIRoot.ORIGINAL_SCREEN_HEIGHT;
        float tWidthScale = (float)Screen.width / UIRoot.ORIGINAL_SCREEN_WIDTH;
        if (Mathf.Approximately(tHeightScale, tWidthScale))
        {
            return;
        }

        float tScale = tHeightScale > tWidthScale ? tHeightScale / tWidthScale : tWidthScale / tHeightScale;
        transform.localScale = new Vector3(tScale, tScale, 1);
    }
}