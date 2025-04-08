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

    UISwitch _switnLike;
    MiniTypeConfig mTypeConfig;
    MiniGameType mMiniGameType;
    private void Awake()
    {
        _switnLike = BtnLike.GetComponentInChildren<UISwitch>(true);
        BtnLike.onClick.AddListener(OnClickBtnLike);
    }

    public void Init(MiniTypeConfig pConfig)
    {
        mTypeConfig = pConfig;

        BtnEnter.onClick.AddListener(OnClickBtnEnter); 
        mMiniGameType = (MiniGameType)pConfig.ID;
        ImgIcon.sprite = ResTool.LoadIcon(pConfig.coverIcon, GameConst.ATLAS_MINI_EVENT_PATH);
        TxtTitle.text = pConfig.titleName;
    }

    void OnClickBtnLike()
    {

    }

    void OnClickBtnEnter()
    { 
        PageManager.Instance.OpenPage(PageID.MiniEnterPage, new MiniEnterPageParam(mTypeConfig));
    }
}
