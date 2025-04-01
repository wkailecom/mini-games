using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class ScrollRectNevigation : MonoBehaviour
{

    private ScrollRect scrollRect;
    private RectTransform viewport;
    private RectTransform content;

    float needTime = 0.3f;
    Tweener tweener;
    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        viewport = scrollRect.viewport;
        content = scrollRect.content;
    }

    public IEnumerator Nevigate(GameObject item, float pWaitTime)
    {
        var target = item.GetComponent<RectTransform>();

        Vector3 itemCurrentLocalPostion = scrollRect.GetComponent<RectTransform>().InverseTransformVector(ConvertLocalPosToWorldPos(target));
        Vector3 itemTargetLocalPos = scrollRect.GetComponent<RectTransform>().InverseTransformVector(ConvertLocalPosToWorldPos(viewport));

        Vector3 diff = itemTargetLocalPos - itemCurrentLocalPostion;
        diff.z = 0.0f;

        var newNormalizedPosition = new Vector2(
            diff.x / (content.rect.width - viewport.rect.width),
            diff.y / (content.rect.height - viewport.rect.height)
            );

        newNormalizedPosition = scrollRect.normalizedPosition - newNormalizedPosition;

        newNormalizedPosition.x = Mathf.Clamp01(newNormalizedPosition.x);
        newNormalizedPosition.y = Mathf.Clamp01(newNormalizedPosition.y);

        if (pWaitTime <= 0)
        {
            scrollRect.normalizedPosition = newNormalizedPosition;
        }
        else
        {
            tweener?.Kill();
            tweener = DOTween.To(() => scrollRect.normalizedPosition, x => scrollRect.normalizedPosition = x, newNormalizedPosition, pWaitTime);//.OnComplete(()=> { });

            yield return null;//new WaitForSeconds(pWaitTime);
        }
    }

    private Vector3 ConvertLocalPosToWorldPos(RectTransform target)
    {
        var pivotOffset = new Vector3(
            (0.5f - target.pivot.x) * target.rect.size.x,
            (0.5f - target.pivot.y) * target.rect.size.y,
            0f);

        var localPosition = target.localPosition + pivotOffset;

        return target.parent.TransformPoint(localPosition);
    }

}
