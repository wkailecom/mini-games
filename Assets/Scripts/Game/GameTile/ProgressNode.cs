using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressNode : MonoBehaviour
{
    public Text levelText;

    public Image firstImage;

    public Image midImage;

    public Image endImage;

    public Image firstImageBg;

    public Image midImageBg;

    public Image endImageBg;

    public GameObject openedEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetNodeIndex(int index)
    {
        levelText.text = $"Lv.{index}";
        openedEffect.SetActive(false);
    }

    public void SetIsCurrentNode()
    {
        //_txtLevel.text = "Current Level";
        openedEffect.SetActive(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void IsFirst(bool isFill)
    {
        firstImage.enabled = isFill;
        midImage.enabled = false;
        endImage.enabled = false;

        firstImageBg.enabled = true;
        midImageBg.enabled = false;
        endImageBg.enabled = false;

        //openedEffect.SetActive(isFill);
    }

    public void IsEnd(bool isFill)
    {
        firstImage.enabled = false;
        midImage.enabled = false;
        endImage.enabled = isFill;

        firstImageBg.enabled = false;
        midImageBg.enabled = false;
        endImageBg.enabled = true;

        //openedEffect.SetActive(isFill);
    }

    public void IsMid(bool isFill)
    {
        firstImage.enabled = false;
        midImage.enabled = isFill;
        endImage.enabled = false;

        firstImageBg.enabled = false;
        midImageBg.enabled = true;
        endImageBg.enabled = false;

        //openedEffect.SetActive(isFill);
    }
}
