using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;
using UnityEngine.UI;
using Game.UISystem;
using System;
using Game.UI;

public class UIBtnProp : MonoBehaviour
{
    public Canvas canvasRoot;
    public Button buttonRoot;
    public PropCountTmpText countTxt;

    int openCount = 0;
    private void Awake()
    {
        buttonRoot.onClick.AddListener(OpenPropShop);
    }

    private void OnEnable()
    {
        openCount = 0;
        canvasRoot.overrideSorting = false;

    }

    private void OnDisable()
    {

        openCount = 0;
        canvasRoot.overrideSorting = false;
    }

    public void OpenPropShop()
    {
        var tShopGroup = countTxt.propID switch
        {
            PropID.Coin => ShopPageParam.ShopGroup.CoinFirst,
            _ => ShopPageParam.ShopGroup.None
        };

        PageManager.Instance.OpenPage(PageID.ShopPage, new ShopPageParam(tShopGroup));
    }
}
