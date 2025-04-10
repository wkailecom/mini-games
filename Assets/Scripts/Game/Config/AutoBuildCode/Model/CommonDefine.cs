namespace Config
{
	public class CommonDefine
	{
		///<summary>
		///普通关卡开始ID
		///</summary>
		public static int noramlStartLevelID = 10000;
		///<summary>
		///Daily关卡开始ID
		///</summary>
		public static int dailyStartLevelID = 300000;
		///<summary>
		///活动关卡开始ID
		///</summary>
		public static int activityStartLevelID = 500000;
		///<summary>
		///兜底循环关卡开始ID
		///</summary>
		public static int loopStartLevelID = 900000;
		///<summary>
		///商店免费金币数
		///</summary>
		public static int shopFreeCoinCount = 100;
		///<summary>
		///商店免费奖励次数
		///</summary>
		public static int shopFreeCount = 10;
		///<summary>
		///商店免费领取间隔(分钟)
		///</summary>
		public static int shopFreeTimeInterval = 5;
		///<summary>
		///体力满值
		///</summary>
		public static int energyFunllCount = 5;
		///<summary>
		///体力恢复间隔(分钟)
		///</summary>
		public static int energyHarvestInterval = 30;
		///<summary>
		///体力购买花费金币数
		///</summary>
		public static int energyCoinCount = 1000;
		///<summary>
		///小游戏显示顺序
		///</summary>
		public static int[] miniShowSort = { 103, 101, 105, 102, 104 };
		///<summary>
		///小游戏状态（0无，1火，2新）
		///</summary>
		public static int[] miniShowState = { 1, 1, 2, 0, 0 };
	}
}