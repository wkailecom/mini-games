
namespace Game
{
    public struct UIPageName
    {
        public const string PageHome = "home";                       // 发生在主界面本身的交互
        public const string PopupAddHeart = "heartsPopout";          // 发生在加生命值弹窗上的交互
        public const string PopupSettings = "settingsPopout";        // 发生在主页上的settings弹窗的交互
        public const string PopupSettingsIngame = "ingameSettings";  // 发生在各种游戏界面中的settings弹窗里的交互
        public const string PageDaily = "dailySubPage";              // 发生在daily二级页面上的交互
        public const string PageTournament = "tournamentSubPage";    // 发生在daily二级页面上的交互
        public const string PopupEvent = "eventPopout";              // 发生在event弹窗上的交互
        public const string PageGame = "ingame";                     // 发生在endless/daily/novel游戏页面中的交互，看levelMode字段可得知属于哪一个
        public const string PopupTutor = "tutorialPopout";           // 各种教学弹窗
        public const string PopupRevive = "revivePopout";            // 发生在复活弹窗里的交互
        public const string PageOver = "clear";                      // 发生在结算页面里的交互

    }

    public struct UIActionName
    {
        public const string AddHeart = "addHearts";               // 点击增加生命
        public const string AddCoin = "addCoin";                  // 点击增加金币
        public const string Settings = "settings";                // 点击settings
        public const string RemoveAds = "removeAds";              // 点击购买去广告
        public const string EnterDaily = "enterDaily";            // 点击daily的入口按钮
        public const string EnterEvent = "enterEvent";            // 点击event的进度和入口
        public const string EnterNovel = "enterNovel";            // 点击novel的入口按钮
        public const string EnterTournament = "tournament";       // 点击tournament的进度和入口
        public const string EnterMiniGame = "miniGame";           // 点击小游戏入口
        public const string TalentPanel = "talentPanel";          // 点击talent区域展开或关闭数值/talent右上角的info button展开或关闭数值 2/3
        public const string Play = "play";                        // 点击开始游戏（进endless）
        public const string EnterShop = "enterShop";              // 点击商店按钮
        public const string Na = "na";                            // 同意观看广告获得生命值奖励/关闭窗口 4/3
        public const string Notifications = "notifications";      // 打开/关闭notifications
        public const string Vibration = "vibration";              // 打开/关闭vibration
        public const string Sound = "sound";                      // 打开/关闭sound
        public const string RestoreBuy = "restoreBuy";            // 点击restore purchases
        public const string Contact = "contact";                  // 点击contact Us
        public const string FontSize = "fontSize";                // 选择字号大小
        public const string ReturnHome = "home";                  // 点击回到主界面
        public const string Restart = "restart";                  // 点击重新开始这局游戏
        public const string ClickTutorial = "clickTutorial";      // 点击查看教学
        public const string Close = "close";                      // 通过x按钮关闭弹窗 3
        public const string BuyHints = "buyHints";                // 购买hints
        public const string UseHints = "useHints";                // 使用hints
        public const string RevealLetter = "revealLetter";        // hints的二次确认，确认揭示某字母
        public const string CloseHint = "closeHint";              // 通过x按钮关闭hint的二次确认
        public const string AdHints = "adHints";                  // 当没有hints库存时，通过播放激励广告获得一个hint
        public const string PreviousKey = "previousKey";          // 点击键盘上的向前键
        public const string NextKey = "nextKey";                  // 点击键盘上的向后键
        public const string Revive = "revive";                    // 点击观看激励广告并复活
        public const string Share = "share";                      // 点击分享图片按钮
        public const string PlayAd = "playAd";                    // 点击观看激励广告并获得3倍数量的tokens
        public const string Next = "next";                        // 点击进入下一关并收集1倍数量的tokens

    }

    public struct UserBuyFrom
    {
        public const string ShopPage = "shopSubmenu";             // 商店弹出菜单
        public const string HomePage = "home";                    // 主界面上发生的付费（包括但不限于：去广告按钮，等）
        public const string DailyPage = "dailySubPage";           // 在进入日历关卡之前的二级界面上发生的付费（包括但不限于：去广告按钮）
        public const string NovelPage = "novelSubPage";           // 在进入小说关卡之前的二级界面上发生的付费（包括但不限于：去广告按钮）
        public const string Endless = "endless";                  // 在无尽关卡中发生的付费（hint,去广告）
        public const string Daily = "daily";                      // 在日历关卡中发生的付费（hint,去广告）
        public const string Novel = "novel";                      // 在小说关卡中发生的付费（hint,去广告）
        public const string SettingsPage = "ingameSettings";      // 在任意关卡进程中，打开setting页面发生的付费
        public const string HomeSettings = "homeSettings";        // 在主界面通过打开Settings页面发生的付费
        public const string MiniSingle = "miniSingle";            // 小游戏单个商店
        public const string MiniBundle = "miniBundle ";           // 小游戏礼包商店
    }

    public struct GameSceneName
    {
        //GameModeType.Endless => "normal",
        //GameModeType.Daily => "daily ",
        //GameModeType.Novel => "novel",
        //_ => "normal",
    }
}