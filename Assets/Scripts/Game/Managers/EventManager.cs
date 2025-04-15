using Config;
using Game;
using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;


public enum EventKey
{
    ApplicationFocus,
    ApplicationPause,

    PageBeginOpen,
    PageOpened,
    PageClosed,
    UIAction,

    ADShown,
    ADClosed,
    ADShowFailed,
    VideoADRewarded,
    VideoADLoaded,
    ADForecastRevenue,

    PurchaseSuccess,
    ValidateReceiptResult,
    AppsflyerCallBack,

    PropCountChange,
    GetRewards,

    GameStartBefore,
    GameStart,
    GameRetry,
    GameOver,

    StartNewDay,
    StartNewHour,
    SwitchUserGroup,

    MiniGameStart,
    MiniGameRetry,
    MiniGameOver,

    #region Screw
    MiniGameUsePropComplete,

    #endregion

    #region Jam
    MiniGameSubSuccess,         //完成小节

    #endregion

    #region BusOut
    BusOut_ReadyToSuccess,      //完成游戏（动画前）
    //------------------------------------
    BusOut_OnClickUnlockSlot,    //点击解锁停车位 
    BusOut_OnClickVIP,           //点击vip道具
    BusOut_VIPComplete,          //vip道具使用成功
    BusOut_PassengerNumberChange,//等待的乘客数量改变
    BusOut_VIPMoveFinish,        //vip移动完成
    BusOut_VehicleHit,           //车辆被击中
    BusOut_VehicleClick,         //车辆被点击
    BusOut_PassengerSeat,        //乘客到达座位
    #endregion
}

public static class EventManager
{
    public static void Init()
    {
        AddEventData(EventKey.ApplicationFocus, new ApplicationFocus());
        AddEventData(EventKey.ApplicationPause, new ApplicationPause());

        AddEventData(EventKey.PageBeginOpen, new PageOperation(EventKey.PageBeginOpen));
        AddEventData(EventKey.PageOpened, new PageOperation(EventKey.PageOpened));
        AddEventData(EventKey.PageClosed, new PageOperation(EventKey.PageClosed));
        AddEventData(EventKey.UIAction, new UIAction());

        AddEventData(EventKey.ADShown, new ADEvent(EventKey.ADShown));
        AddEventData(EventKey.ADClosed, new ADEvent(EventKey.ADClosed));
        AddEventData(EventKey.ADShowFailed, new ADEvent(EventKey.ADShowFailed));
        AddEventData(EventKey.VideoADRewarded, new ADEvent(EventKey.VideoADRewarded));
        AddEventData(EventKey.VideoADLoaded, new ADEvent(EventKey.VideoADLoaded));
        AddEventData(EventKey.ADForecastRevenue, new ADEvent(EventKey.ADForecastRevenue));

        AddEventData(EventKey.PurchaseSuccess, new PurchaseSuccess());
        AddEventData(EventKey.ValidateReceiptResult, new ValidateReceiptResult());
        AddEventData(EventKey.AppsflyerCallBack, new AppsflyerCallBack());

        AddEventData(EventKey.GameStart, new GameStart());
        AddEventData(EventKey.GameOver, new GameOver());

        AddEventData(EventKey.PropCountChange, new PropCountChange());
        AddEventData(EventKey.GetRewards, new GetRewards());

        AddEventData(EventKey.StartNewDay, new EventData((int)EventKey.StartNewDay));
        AddEventData(EventKey.StartNewHour, new EventData((int)EventKey.StartNewHour));
        AddEventData(EventKey.SwitchUserGroup, new EventData((int)EventKey.SwitchUserGroup));

        AddEventData(EventKey.MiniGameStart, new MiniGameStart());
        AddEventData(EventKey.MiniGameOver, new MiniGameOver());
        AddEventData(EventKey.MiniGameSubSuccess, new EventData((int)EventKey.MiniGameSubSuccess));
        AddEventData(EventKey.MiniGameUsePropComplete, new MiniGameUsePropComplete());

        AddEventData(EventKey.BusOut_ReadyToSuccess, new EventData((int)EventKey.BusOut_ReadyToSuccess));
        AddEventData(EventKey.BusOut_OnClickUnlockSlot, new BusOut_OnClickUnlockSlot());
        AddEventData(EventKey.BusOut_OnClickVIP, new EventData((int)EventKey.BusOut_OnClickVIP));
        AddEventData(EventKey.BusOut_VIPComplete, new EventData((int)EventKey.BusOut_VIPComplete));
        AddEventData(EventKey.BusOut_PassengerNumberChange, new BusOut_PassengerNumberChange());
        AddEventData(EventKey.BusOut_VIPMoveFinish, new EventData((int)EventKey.BusOut_VIPMoveFinish));
        AddEventData(EventKey.BusOut_VehicleHit, new EventData((int)EventKey.BusOut_VehicleHit));
        AddEventData(EventKey.BusOut_VehicleClick, new EventData((int)EventKey.BusOut_VehicleClick));
        AddEventData(EventKey.BusOut_PassengerSeat, new EventData((int)EventKey.BusOut_PassengerSeat));


    }

    public static void AddEventData(EventKey pEventKey, EventData pEventData)
    {
        //var tEventID = pEventData.GetHashCode();
        LLFramework.Event.EventManager.Instance.AddEventData((int)pEventKey, pEventData);
    }

    public static T GetEventData<T>(EventKey pEventKey) where T : EventData
    {
        return LLFramework.Event.EventManager.Instance.GetEventData((int)pEventKey) as T;
    }

    public static void Register(EventKey pEventKey, Action<EventData> pEventHandler, bool pPriority = false)
    {
        LLFramework.Event.EventManager.Instance.Register((int)pEventKey, pEventHandler, pPriority);
    }

    public static void Unregister(EventKey pEventKey, Action<EventData> pEventHandler, bool pPriority = false)
    {
        LLFramework.Event.EventManager.Instance.Unregister((int)pEventKey, pEventHandler, pPriority);
    }

    public static void Trigger(EventKey pEventKey)
    {
        LLFramework.Event.EventManager.Instance.Trigger((int)pEventKey);
    }

    public static void Trigger(EventData pEventData)
    {
        LLFramework.Event.EventManager.Instance.Trigger(pEventData);
    }
}

public class ApplicationFocus : EventData
{
    public ApplicationFocus() : base((int)EventKey.ApplicationFocus) { }

    public bool focus;
    public long loseFocusSeconds;

    public override void SetObjectFree()
    {
        base.SetObjectFree();
        loseFocusSeconds = 0;
    }
}

public class ApplicationPause : EventData
{
    public ApplicationPause() : base((int)EventKey.ApplicationPause) { }

    public bool pause;
    public long losePauseSeconds;

    public override void SetObjectFree()
    {
        base.SetObjectFree();
        losePauseSeconds = 0;
    }
}

public class PageOperation : EventData
{
    public PageOperation(EventKey pEventKey) : base((int)pEventKey) { }

    public PageID pageID;
}

public class UIAction : EventData
{
    public UIAction() : base((int)EventKey.UIAction) { }

    public string UIName;
    public string UIPageName;
    public UIActionType actionType;
    public bool isReport;
    public ADType ADType;

    public override void SetObjectFree()
    {
        base.SetObjectFree();
        isReport = false;
        ADType = ADType.Invalid;
    }

}
public enum UIActionType
{
    Click = 1,
    Show = 2,
    Close = 3,
    Agree = 4,
    Refuse = 5,
    ClickBlank = 6,
}
//public struct UIActionName
//{
//    public const string Home = "home";
//    public const string Shop = "shop";
//    public const string StartGame = "StartGame";
//    public const string RetryGame = "RetryGame";
//    public const string ReviveGame = "ReviveGame";
//    public const string ReturnHome = "ReturnHome";
//}

public class ADEvent : EventData
{
    public ADEvent(EventKey pEventKey) : base((int)pEventKey) { }

    public ADType ADType;
    public ADShowReason showReason;
    public string platform;
    public string placementID;
    public string ADID;
    public string country;
    public string ADRevenue;
}

public class PurchaseSuccess : EventData
{
    public PurchaseSuccess() : base((int)EventKey.PurchaseSuccess) { }

    public OrderInfo orderInfo;
    public Product product;
    public IAPProductConfig productConfig;
    public bool isRestore;
    public bool isSkipAFValidate;

    public override void SetObjectFree()
    {
        base.SetObjectFree();

        orderInfo = null;
        product = null;
        productConfig = null;
        isRestore = false;
        isSkipAFValidate = false;
    }
}

public class ValidateReceiptResult : EventData
{
    public ValidateReceiptResult() : base((int)EventKey.ValidateReceiptResult) { }

    public OrderInfo orderInfo;

    public override void SetObjectFree()
    {
        base.SetObjectFree();

        orderInfo = null;
    }
}

public class AppsflyerCallBack : EventData
{
    public AppsflyerCallBack() : base((int)EventKey.AppsflyerCallBack) { }

    public string conversionData;

    public override void SetObjectFree()
    {
        base.SetObjectFree();

        conversionData = null;
    }
}

public class PropCountChange : EventData
{
    public PropCountChange() : base((int)EventKey.PropCountChange) { }

    public PropID propID;
    public int changedCount;
    public int currentCount;
}

public class GetRewards : EventData
{
    public GetRewards() : base((int)EventKey.GetRewards) { }

    public List<PropData> rewards;
    public PropSource source;

    public override void SetObjectFree()
    {
        base.SetObjectFree();
        rewards = null;
    }
}

public class GameStart : EventData
{
    public GameStart() : base((int)EventKey.GameStart) { }

    public GameModeType gameModeType;
    public bool isNewGame;
    public int levelID;

    public override void SetObjectFree()
    {
        base.SetObjectFree();

        gameModeType = GameModeType.Endless;
        isNewGame = false;
    }
}

public class GameOver : EventData
{
    public GameOver() : base((int)EventKey.GameOver) { }

    public GameModeType gameModeType;
    public bool isSuccess;
    public int levelID;

    public override void SetObjectFree()
    {
        base.SetObjectFree();

        gameModeType = GameModeType.Endless;
        isSuccess = false;
    }
}

public class MiniGameStart : EventData
{
    public MiniGameStart() : base((int)EventKey.MiniGameStart) { }

    public MiniGameType modeType;
    public bool isNewGame;
    public int levelID;

    public override void SetObjectFree()
    {
        base.SetObjectFree();

        isNewGame = false;
    }
}

public class MiniGameOver : EventData
{
    public MiniGameOver() : base((int)EventKey.MiniGameOver) { }

    public MiniGameType modeType;
    public bool isSuccess;
    public int levelID;

    public override void SetObjectFree()
    {
        base.SetObjectFree();

        isSuccess = false;
    }
}

public class MiniGameUsePropComplete : EventData
{
    public MiniGameUsePropComplete() : base((int)EventKey.MiniGameUsePropComplete) { }

    public MiniGameType modeType;
    public PropID propID;

    public override void SetObjectFree()
    {
        base.SetObjectFree();

    }
}

#region BusOut

public class BusOut_PassengerNumberChange : EventData
{
    public BusOut_PassengerNumberChange() : base((int)EventKey.BusOut_PassengerNumberChange) { }

    public int count;

    public override void SetObjectFree()
    {
        base.SetObjectFree();
        count = 0;
    }
}

public class BusOut_OnClickUnlockSlot : EventData
{
    public BusOut_OnClickUnlockSlot() : base((int)EventKey.BusOut_OnClickUnlockSlot) { }

    public int index;

    public override void SetObjectFree()
    {
        base.SetObjectFree();
        index = 0;
    }
}


#endregion