public static class ModuleManager
{
    public static PropModule Prop = new();
    public static UserInfoModule UserInfo = new();
    public static StatisticsModule Statistics = new();
    public static GameAnalyseModule Analyse = new();
    public static GuideModule Guide = new();
    public static MiniGameModule MiniGame = new();
    public static MiniFavorModule MiniFavor = new();

    public static void Init()
    {
        Prop.Init();
        UserInfo.Init();
        Statistics.Init();
        Analyse.Init();
        MiniGame.Init();
        MiniFavor.Init();
        Guide.Init();
    }

    public static void Uninit()
    {
        Prop.Uninit();
        UserInfo.Uninit();
        Statistics.Uninit();
        Analyse.Uninit();
        MiniGame.Uninit();
        MiniFavor.Uninit();
        Guide.Uninit();
    }
}