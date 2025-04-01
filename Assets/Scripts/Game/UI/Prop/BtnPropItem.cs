using Config;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BtnPropItem : MonoBehaviour
{
    public PropID propID;
    [HideInInspector] public int propCount;

    [SerializeField] private Text txtCount;
    [SerializeField] private Image imgAdd;

    float animTime = 0.3f;
    float delayTime = 0;
    Tweener tweener;

    void OnEnable()
    {
        EventManager.Register(EventKey.PropCountChange, OnPropCountChange);
        Refresh();
    }

    void OnDisable()
    {
        EventManager.Unregister(EventKey.PropCountChange, OnPropCountChange);
    }

    void OnPropCountChange(EventData pEventData)
    {
        if ((pEventData as PropCountChange).propID == propID)
        {
            Refresh(true);
        }
    }

    public virtual void Refresh(bool pIsAnim = false)
    {
        propCount = ModuleManager.Prop.GetPropCount(propID);
        if (propCount <= 0)
        {
            imgAdd.gameObject.SetActive(true);
            txtCount.transform.parent.gameObject.SetActive(false);
            return;
        }

        imgAdd.gameObject.SetActive(false);
        txtCount.transform.parent.gameObject.SetActive(true);
        if (pIsAnim)
        {
            tweener = DOTween.To(value => txtCount.text = Mathf.Floor(value).ToString(), txtCount.text.ToInt(), propCount, animTime).SetDelay(delayTime);
        }
        else
        {
            tweener?.Complete();
            txtCount.text = propCount.ToString();
        }
    }

    public void SetDelayTime(float pDelayTime)
    {
        delayTime = pDelayTime;
    }
}