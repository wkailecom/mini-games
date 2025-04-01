using DG.Tweening;
using Game.UISystem;
using LLFramework;
using UnityEngine;
using UnityEngine.UI;

public class UIRoot : MonoSingleton<UIRoot>
{
    public const int ORIGINAL_SCREEN_WIDTH = 1080;
    public const int ORIGINAL_SCREEN_HEIGHT = 1920;
    public const int BANNER_OFFSET_Y = 150;
    public const int TOP_OFFSET_Y = 70;
    public const int BOTTOM_OFFSET_Y = 102;
    public const int TOP_DIST = 30;

    static Vector2 safeOffset = -Vector2.one;
    public static (float top, float bottom) SafeOffset
    {
        get
        {
            if (safeOffset == -Vector2.one)
            {
                if (Screen.safeArea.height == Screen.height && Screen.safeArea.width == Screen.width)
                {
                    safeOffset = Vector2.zero;
                }
                else
                {
                    float tRatio = (float)ORIGINAL_SCREEN_HEIGHT / Screen.height;
                    float tTop = (Screen.height - (Screen.safeArea.height + Screen.safeArea.y) - TOP_DIST) * tRatio;
                    float tBottom = Screen.safeArea.y * tRatio;
                    safeOffset = new Vector2(tTop, tBottom);
                }
            }
            return (safeOffset.x, safeOffset.y);
        }
    }

    static float aspectRatio = -1;
    public static float AspectRatio
    {
        get
        {
            if (aspectRatio < 0)
            {
                aspectRatio = ((float)Screen.height / Screen.width) / ((float)ORIGINAL_SCREEN_HEIGHT / ORIGINAL_SCREEN_WIDTH);
            }
            return aspectRatio;
        }
    }

    public Camera MainCamera;
    public Camera UICamera;
    public Canvas UICanvas;
    public CanvasScaler UIScaler;
    //public UILoadingManager loadingManager;
    public GameObject uiLoading;
    public UIADLoadingManager adLoadingManager;

    public Transform PagesPanel;
    public GameObject InputLockPanel;
    public CanvasGroup CutToPanel;


    public void Init()
    {
        UIScaler.matchWidthOrHeight = AspectRatio > 1 ? 0 : 1;
        //loadingManager.Init();
        adLoadingManager.Init();
        InputLockManager.Instance.Init();
        PageManager.Instance.Init();
        PopTaskManager.Instance.Init();
    }

    public void Uninit()
    {
        PageManager.Instance.Uninit();
        PopTaskManager.Instance.Uninit();
    }

    public void FadeEnter(float pTime = 0.5f)
    {
        CutToPanel.alpha = 1;
        DOTween.Sequence().Append(CutToPanel.DOFade(0, pTime).SetEase(Ease.InQuart));
    }

    public void FadeExit(float pTime = 0.5f)
    {
        CutToPanel.alpha = 0;
        DOTween.Sequence().Append(CutToPanel.DOFade(1, pTime).SetEase(Ease.InExpo));
    }

}