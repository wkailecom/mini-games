using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHintController : MonoBehaviour
{
    public GameObject guideNode;
    [Space()]
    [Header("无操作检测")]
    [Tooltip("检测间隔")] public float inspectionInterval = 60f;
    [Tooltip("刷新间隔")] public float refreshInterval = 1f;
    [Tooltip("是否启用检测")] public bool isEnabled = true;

    private float lastInteractionTime;

    public void ResetTime()
    {
        lastInteractionTime = Time.time;
    }

    void Start()
    {
        guideNode.SetActive(false);
        lastInteractionTime = Time.time;
        InvokeRepeating("CheckInactivity", 0f, refreshInterval);
    }

    void Update()
    {
        if (!isEnabled) return;

        CheckUserInput();
    }


    void CheckUserInput()
    {
#if UNITY_EDITOR 
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
#elif UNITY_IOS || UNITY_ANDROID
        // 在移动设备上检测触摸输入
        if (Input.touchCount > 0)
#endif
        {
            lastInteractionTime = Time.time;
            if (guideNode.activeSelf)
            {
                guideNode.SetActive(false);
            }
        }
    }

    void CheckInactivity()
    {
        if (!isEnabled) return;

        float timeSinceLastInteraction = Time.time - lastInteractionTime;
        if (timeSinceLastInteraction > inspectionInterval)
        {
            if (!guideNode.activeSelf)
            {
                guideNode.SetActive(true);
            }
        }
    }
}
