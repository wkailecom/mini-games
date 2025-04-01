
using System;

namespace Game
{
    public class GameConst
    {
        /***************常量*******************/
        public const string GooglePlayPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAiftrCKNgW1stKRA597waQ1QFXfRfZkPGsAlKlFlOjpcAdQ4pX2DtIAjH70dhVPTkNC76Dq4GDuvtz4Tj8Gj86rkRo/zNZOUVBZV5SHSwThILVFOlDIJGqMntrLW/+ulQ5zm2RplB0QOaDa2HFU2xO+9U9+BfbsEpPQPAslaw8Cn5fjbzFHKe0FRCBiQX8HZEbyyJ60sVOu/nHtI2LFJptcmPw3bDTPlAws+nLjlE6caUaXTKXc7oV8Oy0PyfSfCxPoJCTe7bPqleqzHstUvRET9vsOIcJLkKncs5kcqXAEwCdh8k1N8zh8cjGKAfwVNC/fTsXTi38dFJe0zaWn0fWQIDAQAB";
        public const string AppsflyerDevkey = "aenSuFdSXYGEa8HyMfspZA";
        public const string IosAppID = "6572296568";
        public const string ShuShuAppID = "b065ac7070ba4ccbbc7801df4fe3aa0a";

        public const int TARGET_FRAME_RATE = 60; // 目标帧率 
        public const string CONFIG_ROOT_PATH = "Configs/ConfigData"; //配置根目录A
        public const string CONFIG_ROOT_PATHB = "Configs/ConfigDataB";//配置根目录B
        public const string CONFIG_ROOT_PATHC = "Configs/ConfigDataC";//配置根目录C
        public const string CONFIG_ROOT_PATHD = "Configs/ConfigDataD";//配置根目录D

        public const string ATLAS_PROPS_PATH = "Atlas/Props";          //图集道具Icon路径
        public const string ATLAS_TOKENS_PATH = "Atlas/Tokens";        //图集收集品Icon路径

        public const string EVENT_ASSET_PATH = "UI/Widget/EventWidget";        //活动相关资源路径
         
        public const int TOKENS_DOUBLE_COUNT = 2;               //奖励加倍倍数 
        public const int MINI_GAME_LEVEL = 4;                   //小游戏开启关卡    

        public const int QueryID_ADS = 101;                     //去广告查询id
        public const int QueryID_102 = 102;                     //102商品查询id
        public static string ProductID_ADS;                     //去广告商品id
        public static string ProductID_102;                     //102商品id

    }

    public class GameVariable
    {
        /***************变量*******************/
        public static bool IsDebugMode = false;                 //是否是debug模式 
        public static bool IsTestDevice = false;                //是否测试设备
        public static bool IsSkipPurchasValidate = true;        //是否购买验证      

        public static string AFMediaSource = string.Empty;      //AF归因来源
        public static bool IsCMPRequiredAtLocation = false;     //是否CMP指定地区
        public static string UserBuyFrom = string.Empty;        //用户购买来源
        public static bool IngamePurchase = false;              //用户是否游戏内购买

        public static DateTime ADCumulativeDurationLastTime = DateTime.MinValue;//累计时间广告上次播放时间

        public static UIBtnHeart CurUIBtnHeart;                 //当前体力按钮
        public static GameSceneType CurSceneType;               //当前场景类型

    }

}