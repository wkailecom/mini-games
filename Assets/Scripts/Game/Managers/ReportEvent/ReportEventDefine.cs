using Config;
using Game;
using System;
using System.Collections.Generic;
using System.Text;

public static class ReportEventDefine
{
    public static void Init()
    {

    }

    #region APP
    public const string AppID_Key = "appid";
    public const string AppName_Key = "appName";
    public const string AppVersion_Key = "version";
    public const string AppOS_Key = "appOS";
    public const string AppBuildType_Key = "buildType";
    public const string ResVersion_Key = "resVersion";
    public const string FirstInstallVersion_Key = "firstVersion";
    public const string DataInsertTime_Key = "insertTime";

    public static string AppID_Value => AppInfoManager.Instance.AppIdentifier;
    public static string AppName_Value => AppInfoManager.Instance.AppName;
    public static string AppOS_Value => AppInfoManager.Instance.AppOS;
    public static string AppBuildType_Value => AppInfoManager.Instance.AppBuildType;
    public static string AppVersion_Value => AppInfoManager.Instance.AppVersion;
    public static string ResVersion_Value => AppInfoManager.Instance.ResVersion;
    //public static string FirstInstallVersion_Value => AppInfoManager.Instance.FirstVersion;
    public static string DataInsertTime_Value => DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
    #endregion

    #region 设备
    public const string DeviceSystem_Key = "system";
    public const string DeviceModel_Key = "phoneModel";
    public const string DeviceCountry_Key = "region";
    public const string DeviceLanguage_Key = "systemLanguage";

    public static string DeviceSystem_Value => AppInfoManager.Instance.DeviceOS;
    public static string DeviceModel_Value => AppInfoManager.Instance.DeviceModel;
    public static string DeviceCountry_Value => AppInfoManager.Instance.DeviceCountry;
    public static string DeviceLanguage_Value => AppInfoManager.Instance.DeviceLanguage;
    #endregion

    #region ID
    public const string UserID_Key = "userid";
    public const string UserGroup_Key = "userIDGroup";
    public const string FirebaseUserID_Key = "firebaseUserID";
    public const string AppsflyerID_Key = "appsflyer_id";
    public const string IDFA_Key = "IDFA";
    public const string IDFV_Key = "IDFV";
    public const string MADID_Key = "madid";

    public static string UserID_Value => AppInfoManager.Instance.UserID;
    public static string UserGroup_Value => AppInfoManager.Instance.UserGroupName;
    public static string FirebaseUserID_Value => AppInfoManager.Instance.FirebaseUserID;
    public static string AppsflyerID_Value => AppInfoManager.Instance.AppsflyerID;
    public static string IDFA_Value => AppInfoManager.Instance.IDFA;
    public static string IDFV_Value => AppInfoManager.Instance.IDFV;
    public static string MADID_Value => AppInfoManager.Instance.MADID;
    #endregion

    #region Appsflyer
    public const string AFMediaSource_Key = "installationChannel";
    public const string AFChannelType_Key = "channelType";
    public const string AFCampaignID_Key = "campaignID";
    public const string AFCampaign_Key = "campaignName";
    public const string AFCampaignName_Key = "afCampaignName";
    public const string AFChannel_Key = "Channel";
    public const string AFCID_Key = "af_c_id";
    public const string AFADID_Key = "ad_id";
    public const string AFAFADID_Key = "af_ad_id";
    #endregion

    #region Firebase
    public const string VALUE = "value";
    public const string CURRENCY = "currency";
    #endregion

    #region 付费

    public const string PayOrderID_Key = "order";                   //订单号
    public const string PayOrderCreateTime_Key = "orderCreateTime"; //订单创建时间  
    public const string PayOrderType_Key = "orderType";             //订单类型 
    public const string PayTimesTotal_Key = "buyCount";             //购买总次数
    public const string PayIntervalDays_Key = "daysSinceLastBuy";   //距离上次支付间隔天数

    public const string PayProductID_Key = "buyItem";               //购买商品id
    public const string PayProductPrice_Key = "money";              //购买商品价格
    public const string PayReason_Key = "buyFrom";                  //购买商品位置


    public static int PayReason_Value(bool pIsHomePageBuy) => pIsHomePageBuy ? 2 : 1;
    public static int PayIntervalDays_Value => ModuleManager.UserInfo.LastPayDayInterval;
    public static int PayTimesTotal_Value => ModuleManager.UserInfo.PayTimes;
    #endregion

    #region 广告 
    public const string ADPlatform_Key = "platform";              //广告平台 
    public const string ADPlacementID_Key = "placementid";        //广告平台id
    public const string ADShowReason_Key = "address";             //广告播放来源
    public const string ADRewardADID_Key = "videoId";             //广告id
    public const string ADEcpm_Key = "ecpm";                      //广告价值
    public const string ADCount_Key = "adCount";                  //今日展示广告次数
    public const string ADEcpmCount_Key = "ecpmCount";
    public const string ADEadtimesKey = "adtimes";
    public const string ADErrorCode_Key = "errorcode";

    public static string ADShowReason_Value(ADShowReason pReason)
    {
        return pReason switch
        {
            ADShowReason.Video_GetPropHealth => "addHeart",
            ADShowReason.Video_GetPropHint => "addHint",
            ADShowReason.Video_GameRevive => "revive",
            ADShowReason.Video_RewardDoubled => "doubledReward",
            ADShowReason.Video_GetScrewExtraSlot => "addScrewExtraSlot",
            ADShowReason.Video_GetScrewHammer => "addScrewHammer",
            ADShowReason.Video_GetScrewExtraBox => "addScrewExtraBox",
            ADShowReason.Video_GetJam3DReplace => "addJam3DReplace",
            ADShowReason.Video_GetJam3DRevert => "addJam3DRevert",
            ADShowReason.Video_GetJam3DShuffle => "addJam3DShuffle",
            ADShowReason.Video_GetTileRecall => "addTileRecall",
            ADShowReason.Video_GetTileMagnet => "addTileMagnet",
            ADShowReason.Video_GetTileShuffle => "addTileShuffle",
            ADShowReason.Interstitial_GameStart => "enterLevel",
            ADShowReason.Interstitial_GameOver => "clearLevel",
            ADShowReason.Interstitial_GameRetry => "restartLevel",
            ADShowReason.Interstitial_ReturnHome => "returnHome",
            ADShowReason.Interstitial_CumulativeDuration => "timedInterstitial",
            ADShowReason.Interstitial_MiniGameStart => "enterLevel",
            ADShowReason.Interstitial_MiniGameOver => "clearLevel",
            ADShowReason.Interstitial_MiniGameRetry => "restartLevel",
            ADShowReason.Interstitial_MiniGameReturn => "returnHome",
            _ => "Editor",
        };
    }
    public static int ADVideoTimes_Value => ModuleManager.Statistics.GetValue(StatsID.ADVideo, StatsGroup.Total);
    public static int ADInterstitialTimes_Value => ModuleManager.Statistics.GetValue(StatsID.ADInterstitial, StatsGroup.Total);

    #endregion

    #region 统计

    public const string InstallTime_Key = "installTime";                //安装时间
    public const string InstallDays_Key = "installDays";                //安装天数
    public const string ActiveDays_Key = "onlineDays";                  //活跃天数
    public const string TotalPlayTimes_Key = "installBoard";            //累计游戏局数
    public const string TodayPlayTimes_Key = "onlineBoard";             //当天累计游戏局数

    public const string EndlessOverTimes_Key = "topEndless";            //无尽完成关卡数
    public const string EndlessOverTimes2_Key = "clearCountEndless";    //无尽完成关卡数
    public const string DailyOverTimes_Key = "topDaily";                //Daily完成关卡数
    public const string DailyOverTimes2_Key = "clearCountDaily";        //Daily完成关卡数

    public const string LetterCount_Key = "lettersSolved";              //字母完成数量
    public const string WordCount_Key = "wordsSolved";                  //单词完成数量


    public static string InstallTime_Value => ModuleManager.UserInfo.FirstLoginTime.ToString("yyyy-MM-dd");
    public static int InstallDays_Value => ModuleManager.UserInfo.InstallDaysCount;
    public static int ActiveDays_Value => ModuleManager.Statistics.GetValue(StatsID.LoginDays, StatsGroup.Total);

    public static int TotalPlayTimes_Value => ModuleManager.Statistics.GetValue(StatsID.PlayTimes, StatsGroup.Total);
    public static int TodayPlayTimes_Value => ModuleManager.Statistics.GetValue(StatsID.PlayTimes, StatsGroup.TotalDay);

    public static int EndlessOverTimes_Value => ModuleManager.Statistics.GetValue(StatsID.OverTimes, StatsGroup.Type, GameModeType.Endless);
    public static int DailyOverTimes_Value => ModuleManager.Statistics.GetValue(StatsID.OverTimes, StatsGroup.Type, GameModeType.Daily);
     
    #endregion

    #region UI点击行为
    public const string UIAction_Key = "Pageaction";   // 1：点击  2：打开  3：关闭  4：同意  5：拒绝 6：点击空白
    public const string UIPage_Key = "Page";           // 交互页面
    public const string UIName_Key = "interactives";   // 交互UI
    public const string IsADValid_Key = "isADValid";   // 广告是否有效
    public const string IsCouncil_Key = "isGame";      // 是否游戏页内
    #endregion

    #region 局内行为

    public const string UseLanguage_Key = "localizationType";                    //游戏当前语言  
    public const string UseFontSize_Key = "useFontSize";                         //游戏当前字号  
    public const string MistakeTimes_Key = "mistakeCount";                       //游戏当前错误次数  
    public const string TotalMistakeTimes_Key = "totalMistakeCount";             //总共错误次数 
    public const string SceneName_Key = "sceneName";                             //游戏场景 
    public const string GameMode_Key = "gameMode";                               //游戏模式 
    public const string LevelNum_Key = "levelNum";                               //关卡id 用户
    public const string BuyLevel_Key = "buyLevel";                               //关卡id 用户购买时主线关卡
    public const string SeedNum_Key = "seedNum";                                 //关卡id 数据
    public const string IssueNum_Key = "period";                                 //期数
    public const string LevelProgress_Key = "emptyProgress";                     //当局进度
    public const string LevelUniqueID_Key = "entryUniqueID";                     //当局的唯一标识
    public const string NormalLevel_Key = "normalLevel";                         //主线关卡id
    public const string PlayTime_Key = "playTimes";                              //当局游戏时间
    public const string LevelDifficulty_Key = "sceneDifficulty";                 //关卡难度  0 无  easy 简单 medium 中等   hard 困难
    public const string IsWinStreak_Key = "isWinStreak";                         //是否处于连胜状态 TRUE 是 FALSE 否
    public const string RetryCount_Key = "restartCount";                         //当前关卡的重试次数

    public static string UseLanguage_Value => ModuleManager.UserInfo.Data.Language.ToString();
    public static string UseFontSize_Value => ModuleManager.UserInfo.Data.FontSize.ToString();  
    public static string GameMode_Value => GameManager.Instance.CurrentGameModeType.ToString();
    public static string SceneName_Value
    {
        get
        {
            if (GameVariable.CurSceneType == GameSceneType.MiniGame)
            {
                return $"miniGame_{ModuleManager.MiniGame.GameType}";
            }
            else
            {
                return GameManager.Instance.CurrentGameModeType switch
                {
                    GameModeType.Endless => "normal",
                    GameModeType.Daily => "daily ",
                    GameModeType.Novel => "novel",
                    _ => "normal",
                };
            }
        }
    }
    public static int LevelNum_Value
    {
        get
        {
            return MiniGameManager.Instance.Level;
        }
    }
    public static int LevelNumOver_Value => ModuleManager.Analyse.LevelNum;
    public static string SeedNum_Value
    {
        get
        {
            //if (GameVariable.CurSceneType == GameSceneType.MiniGame)
            //{
            //    return ModuleManager.MiniGame.GetLevelConfig(MiniGameManager.Instance.Level).Chessboard;
            //}
            return ModuleManager.MiniGame.GetLevelConfig(MiniGameManager.Instance.Level).Chessboard;
        }
    }
    public static string IssueNum_Value
    {
        get
        {
            if (GameVariable.CurSceneType == GameSceneType.MiniGame)
            {
                return ModuleManager.MiniGame.IssueNum.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    } 
    public static string LevelUniqueID_Value => GameManager.Instance.BaseData.UniqueID.ToString(); 
    public static int PlayTime_Value => GameManager.Instance.BaseData.TakeTime; 
     

    #endregion

    #region 产出
    public const string RewardSource_Key = "source";           //道具物品获取来源 
    public const string PropName_Key = "hintType";             //物品名称
    public const string PropCount_Key = "propCount";           //物品数量
    //剩余道具数量
    public const string LeftPropCount_Key = "leftProp";
    public const string LeftHintCount_Key = "leftHints";
    public const string LeftHealthCount_Key = "leftHearts";
    public const string LeftExSlotCount_Key = "leftExSlot";
    public const string LeftHammerCount_Key = "leftHammer";
    public const string leftExBoxCount_Key = "leftExBox";
    public const string LeftShuffleCount_Key = "leftShuffle";
    public const string LeftReplaceCount_Key = "leftReplace";
    public const string LeftRevertCount_Key = "leftRevert";
    //购买后数量
    public const string BuyLeftHintCount_Key = "buyleftHints";
    public const string BuyLeftHealthCount_Key = "buyleftHearts";
    public const string BuyLeftExSlotCount_Key = "buyleftExSlot";
    public const string BuyLeftHammerCount_Key = "buyleftHammer";
    public const string BuyleftExBoxCount_Key = "buyleftExBox";
    public const string BuyLeftShuffleCount_Key = "buyleftShuffle";
    public const string BuyLeftReplaceCount_Key = "buyleftReplace";
    public const string BuyLeftRevertCount_Key = "buyleftRevert";
    //获得道具数量 
    public const string GetPropCount_Key = "getProp";
    public const string GetHintCount_Key = "getHints";
    public const string GetHealthCount_Key = "getHearts";
    public const string GetExSlotCount_Key = "getExSlot";
    public const string GetHammerCount_Key = "getHammer";
    public const string GetExBoxCount_Key = "getExBox";
    public const string GetShuffleCount_Key = "getShuffle";
    public const string GetReplaceCount_Key = "getReplace";
    public const string GetRevertCount_Key = "getRevert";
    //使用道具数量
    public const string UseHintCount_Key = "useHint";
    public const string UseHealthCount_Key = "useHealth";

    public static string PropName_Value(PropID pPropID)
    {
        return pPropID switch
        {
            PropID.RemoveAD => "removeAD",
            PropID.Health => "hearts",
            PropID.Hint => "hints",
            PropID.Invalid => "Editor",
            _ => pPropID.ToString(),
        };
    }

    public static string PropReason_Value(PropSource pReason)
    {
        return pReason switch
        {
            PropSource.Shop => "shop",
            PropSource.ShopFree => "shopFree",
            PropSource.Rrewarded => "rewardAd",
            PropSource.TimeRecover => "timeRecover",
            PropSource.EventComplete => "event",
            PropSource.Tournament => "tournament",
            PropSource.MiniGameOver => "miniGame",
            PropSource.IngamePurchase => "ingamePurchase",
            _ => pReason.ToString(),
        };
    }

    public static string GetPropCount_Value(List<PropData> pRewardData)
    {
        //Health_5,Hint_2
        StringBuilder tStr = new StringBuilder();
        bool tFirst = true;
        foreach (var tProp in pRewardData)
        {
            if (tFirst)
            {
                tFirst = false;
            }
            else
            {
                tStr.Append(',');
            }

            tStr.Append(PropName_Value(tProp.ID)).Append('_').Append(tProp.Count);
        }
        return tStr.ToString();
    }

    public static int LeftPropCount_Value(PropID pPropID)
    {
        return ModuleManager.Prop.GetPropCount(pPropID);
    }

    public static string LeftPropCount_Value(List<PropData> pRewardData)
    {
        StringBuilder tStr = new StringBuilder();
        bool tFirst = true;
        foreach (var tProp in pRewardData)
        {
            if (tFirst)
            {
                tFirst = false;
            }
            else
            {
                tStr.Append(',');
            }

            tStr.Append(PropName_Value(tProp.ID)).Append('_').Append(LeftPropCount_Value(tProp.ID));
        }
        return tStr.ToString();
    }

    public static string LeftPropCount_Value(params PropID[] props)
    {
        StringBuilder tStr = new StringBuilder();
        bool tFirst = true;
        foreach (var tPropID in props)
        {
            if (tFirst)
            {
                tFirst = false;
            }
            else
            {
                tStr.Append(',');
            }

            tStr.Append(PropName_Value(tPropID)).Append('_').Append(LeftPropCount_Value(tPropID));
        }
        return tStr.ToString();
    }

    public static int PayBeforePropCount_Value(IAPProductConfig pConfig, PropID pPropID)
    {
        return LeftPropCount_Value(pPropID) - GameMethod.GetIAPProductPropCount(pConfig, pPropID);
    }

    public static int UsePropCount_Value(PropID pPropID)
    {
        return pPropID switch
        {
            //PropID.Health => ModuleManager.Statistics.GetValue(StatsID.UseHintTimes, StatsGroup.Type),
            PropID.Hint => ModuleManager.Statistics.GetValue(StatsID.UseHintTimes, StatsGroup.Type),
            _ => 1,
        };
    }

    public static string MiniGameLeftPropCount_Value()
    {
        return ModuleManager.MiniGame.GameType switch
        {
            MiniGameType.Screw => LeftPropCount_Value(PropID.Hint, PropID.ScrewExtraSlot, PropID.ScrewHammer, PropID.ScrewExtraBox),
            MiniGameType.Jam3d => LeftPropCount_Value(PropID.Hint, PropID.Jam3DShuffle, PropID.Jam3DReplace, PropID.Jam3DRevert),
            MiniGameType.Tile => LeftPropCount_Value(PropID.Hint, PropID.TileRecall, PropID.TileMagnet, PropID.TileShuffle),
            _ => string.Empty,
        };
    }

    #endregion
}