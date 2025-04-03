using Config;
using Game;
using Game.UI;
using Game.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class SwapEnergyPage : PageBase
    {
        public UIPropItem prop;
        public Button buyBtn;
        public Button closeBtn;
        public UICountDown timeCountDown;

        protected override void OnInit()
        {
            base.OnInit();

            closeBtn.onClick.AddListener(Close);
            buyBtn.onClick.AddListener(ClickBtnBuy);
        }


        protected override void OnBeginOpen()
        {
            //mParam = PageParam as WatchAdsPageParam;
            //if (mParam == null)
            //{
            //    LogManager.LogError("WatchAdsPage.OnBeginOpen: PageParam is Invalid");
            //    return;
            //}
            //timeCountDown.StartCountDown(ModuleManager.MiniGame.GetHeartRecoverTime());
        }

        void ClickBtnBuy()
        {
            var tIsf = ModuleManager.Prop.ExpendProp(PropID.Coin, CommonDefine.energyCoinCount);
            if (tIsf)
            {
                Close();
                ModuleManager.Prop.AddProp(PropID.Energy, 1, PropSource.CoinSwap);
            }
            else
            {
                PageManager.Instance.OpenPage(PageID.ShopPage, new ShopPageParam(ShopPageParam.ShopGroup.CoinFirst));
            }
        }
    }
}


