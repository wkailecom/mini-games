namespace Config
{
	public class MiniScheduleConfig : ConfigModelBase
	{
		/// <summary>
		///活动期数
		/// </summary>
		public int ID { get; private set; }
		/// <summary>
		///开始时间
		/// </summary>
		public string startTime { get; private set; }
		/// <summary>
		///结束时间
		/// </summary>
		public string endTime { get; private set; }
		/// <summary>
		///活动类型
		/// </summary>
		public int gameType { get; private set; }
		/// <summary>
		///关卡复健索引
		/// </summary>
		public int mapIndex { get; private set; }

		public override void ParseData(string[] pData)
		{
			if (pData == null || pData.Length < 5)
			{
				LogManager.LogError("MiniScheduleConfig.ParseData param wrong!");
				return;
			}

			if (!string.IsNullOrEmpty(pData[0]))
			{
				ID = int.Parse(pData[0]);
			}
			if (!string.IsNullOrEmpty(pData[1]))
			{
				startTime = pData[1];
			}
			if (!string.IsNullOrEmpty(pData[2]))
			{
				endTime = pData[2];
			}
			if (!string.IsNullOrEmpty(pData[3]))
			{
				gameType = int.Parse(pData[3]);
			}
			if (!string.IsNullOrEmpty(pData[4]))
			{
				mapIndex = int.Parse(pData[4]);
			}
		}
	}
}