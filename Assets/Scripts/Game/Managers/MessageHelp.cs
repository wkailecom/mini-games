using DG.Tweening;
using LLFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageHelp : MonoSingleton<MessageHelp>
{
    const float SCREEN_BACKGROUND_XSPACE = 100f;
    const float BACKGROUND_TEXT_XSPACE = 200f;
    const float BACKGROUND_TEXT_YSPACE = 30f;
    const float HIDE_ANIMATION_DURATION = 1f;

    public Text messageText;
    public RectTransform bgTran;
    public CanvasGroup canvasGroup;

    float mScreenWidth;
    ToastInfo mShowingToastInfo;
    Queue<ToastInfo> mToastInfoQueue = new Queue<ToastInfo>();

    protected override void Awake()
    {
        base.Awake();
        mScreenWidth = (transform as RectTransform).rect.width;
        HideMessage();
    }

    public void ShowMessage(string pMessage, float pDuration = 2.5f)
    {
        if (mShowingToastInfo != null && mShowingToastInfo.message.Equals(pMessage))
        {
            return;
        }

        if (mToastInfoQueue.Count > 0 && mToastInfoQueue.Peek().message.Equals(pMessage))
        {
            return;
        }

        ToastInfo tToastInfo = new ToastInfo(pMessage, pDuration);
        mToastInfoQueue.Enqueue(tToastInfo);
        ShowMessage();
    }

    void ShowMessage()
    {
        if (mToastInfoQueue.Count == 0 || mShowingToastInfo != null)
        {
            return;
        }

        StartCoroutine(ShowMessageTask());
    }

    IEnumerator ShowMessageTask()
    {
        mShowingToastInfo = mToastInfoQueue.Dequeue();

        bgTran.gameObject.SetActive(true);
        messageText.text = mShowingToastInfo.message;
        SetMessageSize();

        DOTween.To(x => canvasGroup.alpha = x, 0, 1, mShowingToastInfo.duration);
        yield return new WaitForSeconds(mShowingToastInfo.duration);
        DOTween.To(x => canvasGroup.alpha = x, 1, 0, HIDE_ANIMATION_DURATION);
        yield return new WaitForSeconds(HIDE_ANIMATION_DURATION);

        mShowingToastInfo = null;
        HideMessage();
        ShowMessage();
    }

    void SetMessageSize()
    {
        // Text.preferredWidth不受其他属性影响
        float tTextWidth = messageText.preferredWidth;
        if (tTextWidth + BACKGROUND_TEXT_XSPACE + SCREEN_BACKGROUND_XSPACE > mScreenWidth)
        {
            tTextWidth = mScreenWidth - SCREEN_BACKGROUND_XSPACE - BACKGROUND_TEXT_XSPACE;
        }

        // Text.preferredHeight受Text.Width影响，因此先设置正确的Text.Width再取Text.preferredHeight的值才正确
        bgTran.sizeDelta = new Vector2(tTextWidth + BACKGROUND_TEXT_XSPACE, BACKGROUND_TEXT_YSPACE);
        float tTextHeight = messageText.preferredHeight;

        bgTran.sizeDelta = new Vector2(tTextWidth + BACKGROUND_TEXT_XSPACE, tTextHeight + BACKGROUND_TEXT_YSPACE);
    }

    void HideMessage()
    {
        messageText.text = "";
        bgTran.gameObject.SetActive(false);
    }
    class ToastInfo
    {
        public string message;
        public float duration;

        public ToastInfo(string pMessage, float pDuration)
        {
            message = pMessage;
            duration = pDuration;
        }
    }
}
