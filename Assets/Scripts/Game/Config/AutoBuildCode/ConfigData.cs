namespace Config
{
	public static class ConfigData
	{
		public static ADVideoConfigController aDVideoConfig = new ADVideoConfigController();
		public static APPDefineController aPPDefine = new APPDefineController();
		public static CommonDefineController commonDefine = new CommonDefineController();
		public static IAPProductConfigController iAPProductConfig = new IAPProductConfigController();
		public static JamLevelGroupConfigController jamLevelGroupConfig = new JamLevelGroupConfigController();
		public static LevelConfigController levelConfig = new LevelConfigController();
		public static LogAppEventConfigController logAppEventConfig = new LogAppEventConfigController();
		public static LogEcpmEventConfigController logEcpmEventConfig = new LogEcpmEventConfigController();
		public static MiniMapConfigController miniMapConfig = new MiniMapConfigController();
		public static MiniScheduleConfigController miniScheduleConfig = new MiniScheduleConfigController();
		public static MiniTypeConfigController miniTypeConfig = new MiniTypeConfigController();
		public static PageConfigController pageConfig = new PageConfigController();
		public static PropConfigController propConfig = new PropConfigController();

		public static void Init(string dataPath)
		{
			aDVideoConfig.LoadData(dataPath);
			aPPDefine.LoadData(dataPath);
			commonDefine.LoadData(dataPath);
			iAPProductConfig.LoadData(dataPath);
			jamLevelGroupConfig.LoadData(dataPath);
			levelConfig.LoadData(dataPath);
			logAppEventConfig.LoadData(dataPath);
			logEcpmEventConfig.LoadData(dataPath);
			miniMapConfig.LoadData(dataPath);
			miniScheduleConfig.LoadData(dataPath);
			miniTypeConfig.LoadData(dataPath);
			pageConfig.LoadData(dataPath);
			propConfig.LoadData(dataPath);
		}
	}
}