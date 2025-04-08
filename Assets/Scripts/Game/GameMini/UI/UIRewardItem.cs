using Config;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIRewardItem : MonoBehaviour
{
    public Text propCount;
    public string propCountFormat;
    public Text countProgress;
    public Image imgProp; 

    public bool IsShow => gameObject.activeSelf;
    public PropData propData { get; private set; }

    void Reset()
    { 
        if (propCount != null)
        {
            propCount.text = "";
        }
        if (countProgress != null)
        {
            countProgress.text = "";
        } 

        propData = null;
    }
    public void SetData(PropData pPropData)
    {
        Reset();

        propData = pPropData;
        SetPropIcon();
        SetPropCount();
    }

    void SetPropIcon()
    {
        imgProp?.SetPropIcon(propData.ID, false);
    }

    void SetPropCount()
    {
        if (propCount == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(propCountFormat))
        {
            propCount.text = propData.Count.ToString();
        }
        else
        {
            propCount.text = string.Format(propCountFormat, propData.Count.ToString());
        }
    }

    void SwitchProp(GameObject pProp, bool pIsShow)
    {
        pProp?.SetActive(pIsShow);
    }
}