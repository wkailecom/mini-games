using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private float patternFrequency = 1.0f;

    [Header("Scroll Speed")]
    [SerializeField] private float scrollSpeedX = 0.1f;
    [SerializeField] private float scrollSpeedY = 0.05f;

    void Start()
    {
        UpdateTiling();
    }

    void Update()
    {
        ScrollBackground();
    }

    void UpdateTiling()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float screenAspect = screenWidth / screenHeight;

        float textureAspect = 1.0f;

        if (screenAspect >= textureAspect)
        {
            rawImage.uvRect = new Rect(
                rawImage.uvRect.position,
                new Vector2(screenAspect * patternFrequency, patternFrequency)
            );
        }
        else
        {
            rawImage.uvRect = new Rect(
                rawImage.uvRect.position,
                new Vector2(patternFrequency, (1.0f / screenAspect) * patternFrequency)
            );
        }
    }

    void ScrollBackground()
    {
        rawImage.uvRect = new Rect(
            rawImage.uvRect.position + new Vector2(scrollSpeedX, scrollSpeedY) * Time.deltaTime,
            rawImage.uvRect.size
        );
    }

    void OnRectTransformDimensionsChange()
    {
        UpdateTiling();
    }
}