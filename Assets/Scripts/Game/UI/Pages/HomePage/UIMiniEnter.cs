using Config;
using Game;
using Game.MiniGame;
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
    public Transform BtnLike;
    public GameObject GoHot;
    public GameObject GoNew;

    MiniTypeConfig mTypeConfig;
    MiniGameType mMiniGameType;
    private void Awake()
    {

    }

    public void Init(MiniTypeConfig pConfig)
    {
        mTypeConfig = pConfig;

        BtnEnter.onClick.AddListener(OnClickBtnEnter);
        mMiniGameType = (MiniGameType)pConfig.ID;
        ImgIcon.sprite = ResTool.LoadIcon(pConfig.coverIcon, GameConst.ATLAS_MINI_EVENT_PATH);
        TxtTitle.text = pConfig.titleName;

        var tIsFavor = ModuleManager.MiniFavor.IsFavor(pConfig.ID);
        var tTagState = ModuleManager.MiniFavor.GetTagState(pConfig.ID);
        BtnLike.gameObject.SetActive(tIsFavor);
        if (tTagState == 1)
        {
            GoHot.gameObject.SetActive(true);
            GoNew.gameObject.SetActive(false);
        }
        else if (tTagState == 2)
        {
            GoHot.gameObject.SetActive(false);
            GoNew.gameObject.SetActive(true);
        }
        else
        {
            GoHot.gameObject.SetActive(false);
            GoNew.gameObject.SetActive(false);
        }
    }

    void OnClickBtnEnter()
    {
        PageManager.Instance.OpenPage(PageID.MiniEnterPage, new MiniEnterPageParam(mTypeConfig));
    }
}
