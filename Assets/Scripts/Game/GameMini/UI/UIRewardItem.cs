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

    public GameObject goldRoot;
    public GameObject tileHeartRoot;
    public GameObject tileRecallRoot;
    public GameObject tileMagnetRoot;
    public GameObject tileShuffleRoot;

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

        goldRoot?.SetActive(false);

        tileHeartRoot?.SetActive(false);
        tileRecallRoot?.SetActive(false);
        tileMagnetRoot?.SetActive(false);
        tileShuffleRoot?.SetActive(false);

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
        if (imgProp == null)
        {
            return;
        }

        var mConfig = ConfigData.propConfig.GetByPrimary((int)propData.ID);
        if (mConfig == null)
        {
            return;
        }
        string tPropName = propData.ID switch
        {
            //PropID.Hint => "sticker_hint",
            PropID.Energy => "ui_heart",
            _ => mConfig.icon,
        };
        imgProp.SetPropIcon(tPropName, false);
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