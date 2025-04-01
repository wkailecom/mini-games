using Config;
using Game.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileStaminaPage : PageBase
{
    //    //public UIPropItem prop;
    //    public Button buyBtn;
    //    public Button closeBtn;
    //    public UICountDown timeCountDown;


    //    protected override void OnInit()
    //    {
    //        base.OnInit();

    //        closeBtn.onClick.AddListener(ClickCloseBtn);
    //        buyBtn.onClick.AddListener(ClickBuyBtn);
    //    }


    //    protected override void OnBeginOpen()
    //    {
    //        //mParam = PageParam as WatchAdsPageParam;
    //        //if (mParam == null)
    //        //{
    //        //    LogManager.LogError("WatchAdsPage.OnBeginOpen: PageParam is Invalid");
    //        //    return;
    //        //}
    //        timeCountDown.Show(ModuleManager.TileGame.GetHeartRecoverTime());
    //    }

    //    private void ClickCloseBtn()
    //    {
    //        Close();
    //    }

    //    private void ClickBuyBtn()
    //    {
    //        var tIsf = ModuleManager.Prop.ExpendProp(PropID.Gold, CommonDefine.tileExchangeStaminaGoldCount);
    //        if (tIsf)
    //        {
    //            Close();
    //            ModuleManager.Prop.AddProp(PropID.TileHeart, 1, PropSource.TileHeartExchange);
    //        }
    //        else
    //        {
    //            // HintHelp.Instance.ShowHint("Current gold shortage");

    //            PageManager.Instance.OpenPage(PageID.MiniShopGoldPage);
    //        }
    //    }
}



