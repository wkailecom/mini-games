using Config;
using System;
using UnityEngine;

public abstract class ShopBaseItem : MonoBehaviour
{
    public IAPProductConfig mConfig;
    public abstract void Init(IAPProductConfig tProductConfig, Action<string> pOnClickBuy);
    public virtual void OnShow() { }
    public virtual void OnHide() { }
}
