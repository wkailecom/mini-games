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
            //prop.SetData(mPropData);
            prop.propCount.name = "Full";
            timeCountDown.StartCountDown(ModuleManager.UserInfo.HealthHarvestTime, "Full");
        }

        void ClickBtnBuy()
        {
            var tIsf = ModuleManager.Prop.ExpendProp(PropID.Coin, CommonDefine.energyCoinCount);
            if (tIsf)
            {
                var tAddCount = CommonDefine.energyFunllCount - ModuleManager.Prop.GetPropCount(PropID.Energy);
                ModuleManager.Prop.AddProp(PropID.Energy, tAddCount, PropSource.CoinSwap);

                Close();
            }
            else
            {
                PageManager.Instance.OpenPage(PageID.ShopPage, new ShopPageParam(ShopPageParam.ShopGroup.CoinFirst));
            }
        }
    }
}


