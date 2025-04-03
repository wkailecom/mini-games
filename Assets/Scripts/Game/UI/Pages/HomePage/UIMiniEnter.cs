using Config;
using Game;
using Game.UI;
using Game.UISystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIMiniEnter : MonoBehaviour
{
    public Button BtnEnter;
    public Image ImgIcon;
    public TextMeshProUGUI TxtTitle;
    public GameObject StickerRoot;
    public Button BtnLike;
    public GameObject GoHot;
    public GameObject GoNew;

    private UISwitch _switnLike;
    MiniGameType mMiniGameType;
    private void Awake()
    {
        _switnLike = BtnLike.GetComponentInChildren<UISwitch>(true);
        BtnLike.onClick.AddListener(OnClickBtnLike);
        BtnEnter.onClick.AddListener(OnClickBtnEnter);
    }

    public void Init(MiniGameType pMiniGameType)
    {
        mMiniGameType = pMiniGameType;
        //ImgIcon.sprite =
        TxtTitle.text = pMiniGameType.ToString();
    }

    void OnClickBtnLike()
    {

    }
    void OnClickBtnEnter()
    {
        if (mMiniGameType == MiniGameType.Screw)
        {

        }

        PageManager.Instance.OpenPage(PageID.MiniEnterPage);
    }
}
