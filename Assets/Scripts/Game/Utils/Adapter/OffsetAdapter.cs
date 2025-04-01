using UnityEngine;

public class OffsetAdapter : MonoBehaviour
{
    public bool IsBottom;
    void Awake()
    {
        if (IsBottom)
        {
            transform.localPosition += new Vector3(0, UIRoot.SafeOffset.bottom, 0);
        }
        else
        {
            transform.localPosition -= new Vector3(0, UIRoot.SafeOffset.top, 0);
        }
    }
}