using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    public Image ImgIcon;
    public TextMeshProUGUI TxtCount;
    public TextMeshProUGUI TxtName;

    public void SetUIItem(Sprite pSprite, string pCount, string pName)
    {
        SetIcon(pSprite);
        SetCount(pCount);
        SetName(pName);
    }

    public void SetIcon(Sprite pSprite)
    {
        if (ImgIcon != null)
        {
            ImgIcon.sprite = pSprite;
        }
    }

    public void SetCount(string pCount)
    {
        if (TxtCount != null)
        {
            TxtCount.text = pCount;
        }
    }

    public void SetName(string pName)
    {
        if (TxtName != null)
        {
            TxtName.text = pName;
        }
    }
}
