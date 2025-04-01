using Config;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class PropCountText : MonoBehaviour
{
    public PropID propID;

    protected int mCount;
    protected Text mCountText;

    float animTime = 0.3f;
    float delayTime = 0;
    Tweener tweener;

    void Awake()
    {
        mCountText = GetComponent<Text>();
    }

    void OnEnable()
    {
        EventManager.Register(EventKey.PropCountChange, OnPropCountChange);
        EventManager.Register(EventKey.GameStartBefore, OnGameStartBefore);
        Refresh();
    }

    void OnDisable()
    {
        EventManager.Unregister(EventKey.PropCountChange, OnPropCountChange);
        EventManager.Unregister(EventKey.GameStartBefore, OnGameStartBefore);
    }

    void OnPropCountChange(EventData pEventData)
    {
        if ((pEventData as PropCountChange).propID == propID)
        {
            Refresh(true);
        }
    }

    void OnGameStartBefore(EventData pEventData)
    {
        Refresh();
    }

    public virtual void Refresh(bool pIsAnim = false)
    {
        mCount = ModuleManager.Prop.GetPropCount(propID);
        if (pIsAnim)
        {
            tweener = DOTween.To(value => mCountText.text = Mathf.Floor(value).ToString(), mCountText.text.ToInt(), mCount, animTime).SetDelay(delayTime);
        }
        else
        {
            tweener?.Complete();
            mCountText.text = mCount.ToString();
        }

    }

    public void SetDelayTime(float pDelayTime)
    {
        delayTime = pDelayTime;
    }
}