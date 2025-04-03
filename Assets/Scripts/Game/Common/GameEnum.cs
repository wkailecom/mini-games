
namespace Game
{
    /// <summary>
    /// 道具获取来源
    /// </summary>
    public enum PropSource
    {
        Unknown = -1,
        Dfault,          //默认获得
        Guide,           //新手引导
        Shop,            //商城
        ShopFree,        //商城免费
        Rrewarded,       //激励视频 
        TimeRecover,     //时间恢复
        EventComplete,   //活动完成奖励
        Tournament,      //排行榜
        MiniGameOver,    //小游戏关卡完成
        IngamePurchase,  //游戏内购买
        CoinSwap,        //金币置换
    }

    /// <summary>
    /// 商品种类
    /// </summary>
    public enum ProductPack
    {
        RemoveAD = 1,
        Single = 2,   //单个商品
        Bundle = 3,  //礼包商品
    }

    /// <summary>
    /// 游戏类型
    /// </summary>
    public enum GameSceneType
    {
        WordGame,
        MiniGame,
    }

    /// <summary>
    /// 小游戏类型，同场景名
    /// </summary>
    public enum MiniGameType
    {
        Screw = 1,
        Jam3d = 2,
        Tile = 3,
        Bus = 4,
        Triple = 5,
    }


}