using Config;
using System;

public interface IShopItem
{
    void Init(IAPProductConfig tProductConfig, Action<string> pOnClickBuy);
    void OnShow();
    void OnHide();
}
