using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;


public class ScrollRectNavigate : MonoBehaviour
{
    private ScrollRect _scrollRect;
    private RectTransform _viewport;
    private RectTransform _content;
    private RectTransform _scrollTran;
    private Tweener _tweener;

    public bool isHorizontal = true;
    public bool isVertical = true;

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _scrollTran = GetComponent<RectTransform>();
        _viewport = _scrollRect.viewport;
        _content = _scrollRect.content;
    }

    public IEnumerator Navigate(GameObject pTarget, float waitTime)
    {
        return Navigate(pTarget.GetComponent<RectTransform>(), waitTime);
    }

    public IEnumerator Navigate(RectTransform pTarget, float pWaitTime)
    {
        Vector3 currentLocalPosition = WorldToLocalPosition(pTarget.position);
        Vector3 targetLocalPosition = WorldToLocalPosition(_viewport.position);

        Vector3 diff = targetLocalPosition - currentLocalPosition;
        diff.z = 0.0f;

        Vector2 newNormalizedPosition = _scrollRect.normalizedPosition;

        if (isHorizontal && _scrollRect.horizontal)
        {
            newNormalizedPosition.x -= diff.x / (_content.rect.width - _viewport.rect.width);
            newNormalizedPosition.x = Mathf.Clamp01(newNormalizedPosition.x);
        }

        if (isVertical && _scrollRect.vertical)
        {
            newNormalizedPosition.y -= diff.y / (_content.rect.height - _viewport.rect.height);
            newNormalizedPosition.y = Mathf.Clamp01(newNormalizedPosition.y);
        }

        if (pWaitTime <= 0)
        {
            _scrollRect.normalizedPosition = newNormalizedPosition;
        }
        else
        {
            _tweener?.Kill();
            _tweener = DOTween.To(() => _scrollRect.normalizedPosition, x => _scrollRect.normalizedPosition = x, newNormalizedPosition, pWaitTime);
            yield return null;//new WaitForSeconds(pWaitTime);
        }
    }

    private Vector3 WorldToLocalPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = _viewport.InverseTransformPoint(worldPosition);
        return localPosition;
    }
}
