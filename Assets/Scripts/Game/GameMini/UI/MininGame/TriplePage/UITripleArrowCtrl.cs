using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITripleArrowCtrl : MonoBehaviour
{
    private Transform target;  // 目标位置
    private float arrowScreenEdgePadding = 5f;  // 箭头距离屏幕边缘的最小间距
    private float arrowHeight;
    private float arrowWidth;

    private float topHeightMask;
    private float bottomHeightMask;
    private float middleHeight;
    private float leftWidth;
    private float rightWidth;

    private bool isInit;
    private Camera sceneCamera = null;
    private Camera uiCamera = null;

    private bool isStartShow;
    private bool isPause;

    private float addValue = 10f;

    private RectTransform rectTransform;
    private RectTransform parentRect;
    private RectTransform arrowRect;

    public Transform top;
    public Transform bottom;

    private void Awake()
    {
        arrowWidth = 0;
        arrowHeight = 0;
        rectTransform = transform.GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponent<RectTransform>();
        isInit = false;
    }


    public void OnRefreshTopAndBottomHeight(float topHeight, float bottomHeight)
    {
        topHeightMask = topHeight;
        bottomHeightMask = bottomHeight;
        middleHeight = (Screen.height - topHeightMask - bottomHeightMask) / 2f + bottomHeightMask;
        leftWidth = arrowScreenEdgePadding + arrowWidth / 2;
        rightWidth = Screen.width - arrowScreenEdgePadding - arrowWidth / 2;
        isInit = true;
    }

    public void StopShowArrow()
    {

        isStartShow = false;
        isPause = true;
        gameObject.SetActive(false);
    }

    public void StartShowArrow(Transform targetTrans)
    {
        StopShowArrow();
        var cams = Camera.allCameras;
        for (int i = 0; i < cams.Length; i++)
        {
            if (cams[i].name.Equals("TripleMathCamera"))
                sceneCamera = cams[i];
            if (cams[i].name.Equals("UICamera"))
                uiCamera = cams[i];
        }
        target = targetTrans;
        transform.gameObject.SetActive(true);

        if (arrowWidth == 0 && arrowHeight == 0)
        {
            rectTransform.anchoredPosition = new Vector2(Screen.width / 2, Screen.height / 2);
            var tTargetPos = RectTransformUtility.WorldToScreenPoint(uiCamera, top.position);
            var tTargetPos1 = RectTransformUtility.WorldToScreenPoint(uiCamera, bottom.position);
            arrowWidth = Mathf.Abs(tTargetPos.x - tTargetPos1.x);
            arrowHeight = Mathf.Abs(tTargetPos.y - tTargetPos1.y);
        }
        isStartShow = true;
        isPause = false;
    }

    public void ParuseTime()
    {

    }

    void Update()
    {
        if (isStartShow && !isPause)
        {
            HandleArrow();
        }
    }



    // 显示箭头并指引目标位置
    void HandleArrow()
    {
        if (target == null)
        {
            return;
        }
        Vector3 screenPos = sceneCamera.WorldToScreenPoint(target.transform.position);
        Vector3 arrowPos = screenPos;


        bool isResetRotation = false;
        if (screenPos.x >= leftWidth && screenPos.x <= rightWidth) //&& screenPos.y >= bottomHeightMask + arrowScreenEdgePadding && screenPos.y <= (Screen.height - arrowScreenEdgePadding - topHeightMask)
        {
            if (screenPos.y >= middleHeight + arrowHeight / 2)
            {
                arrowPos.y -= (addValue + arrowHeight / 2);
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            }
            else
            {
                arrowPos.y += (addValue + arrowHeight / 2);
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            }
        }
        else
        {
            isResetRotation = true;
            if (screenPos.x <= leftWidth && screenPos.y >= middleHeight)
            {
                arrowPos.x += (addValue + arrowWidth / 2);
                arrowPos.y -= (addValue + arrowHeight / 2);
            }
            else if (screenPos.x > rightWidth && screenPos.y >= middleHeight)
            {
                arrowPos.x -= (addValue + arrowWidth / 2);
                arrowPos.y -= (addValue + arrowHeight / 2);
            }
            else if (screenPos.x < leftWidth && screenPos.y < middleHeight)
            {
                arrowPos.x += (addValue + arrowWidth / 2);
                arrowPos.y += (addValue + arrowHeight / 2);
            }
            else if (screenPos.x > rightWidth && screenPos.y < middleHeight)
            {
                arrowPos.x -= (addValue + arrowWidth / 2);
                arrowPos.y += (addValue + arrowHeight / 2);
            }
        }

        // 确保箭头不会超出屏幕
        arrowPos.x = Mathf.Clamp(arrowPos.x, arrowScreenEdgePadding + arrowWidth / 2, Screen.width - arrowScreenEdgePadding - arrowWidth / 2);
        arrowPos.y = Mathf.Clamp(arrowPos.y, bottomHeightMask + arrowScreenEdgePadding + arrowHeight / 2, Screen.height - arrowScreenEdgePadding - topHeightMask - arrowHeight / 2);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, arrowPos, uiCamera, out Vector2 localPoint);
        rectTransform.anchoredPosition = localPoint;

        if (isResetRotation)
        {
            var tmpScreenPos = screenPos;
            //tmpScreenPos.y = Mathf.Clamp(arrowPos.y, bottomHeightMask, Screen.height - topHeightMask );

            Vector3 targetDirection = screenPos - arrowPos;
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));  // 设置箭头的旋转角度
        }
    }
}
