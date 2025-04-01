namespace Config
{
	public class LogAppEventConfig : ConfigModelBase
	{
		/// <summary>
		///ID
		/// </summary>
		public int ID { get; private set; }
		/// <summary>
		///事件类型
		/// </summary>
		public int eventType { get; private set; }
		/// <summary>
		///事件名
		/// </summary>
		public string eventName { get; private set; }
		/// <summary>
		///目标值
		/// </summary>
		public int targetValue { get; private set; }
		/// <summary>
		///时间限制(小时)
		/// </summary>
		public int limitTime { get; private set; }
		/// <summary>
		///安卓上报平台
		/// </summary>
		public bool[] andriodPlatform { get; private set; }
		/// <summary>
		///IOS上报平台
		/// </summary>
		public bool[] iosPlatform { get; private set; }
		/// <summary>
		///是否同步BQ
		/// </summary>
		public int reportBQ { get; private set; }

		public override void ParseData(string[] pData)
		{
			if (pData == null || pData.Length < 8)
			{
				LogManager.LogError("LogAppEventConfig.ParseData param wrong!");
				return;
			}

			if (!string.IsNullOrEmpty(pData[0]))
			{
				ID = int.Parse(pData[0]);
			}
			if (!string.IsNullOrEmpty(pData[1]))
			{
				eventType = int.Parse(pData[1]);
			}
			if (!string.IsNullOrEmpty(pData[2]))
			{
				eventName = pData[2];
			}
			if (!string.IsNullOrEmpty(pData[3]))
			{
				targetValue = int.Parse(pData[3]);
			}
			if (!string.IsNullOrEmpty(pData[4]))
			{
				limitTime = int.Parse(pData[4]);
			}
			if (!string.IsNullOrEmpty(pData[5]))
			{
				andriodPlatform = TableParser.ParseArrayData<bool>(pData[5]);
			}
			if (!string.IsNullOrEmpty(pData[6]))
			{
				iosPlatform = TableParser.ParseArrayData<bool>(pData[6]);
			}
			if (!string.IsNullOrEmpty(pData[7]))
			{
				reportBQ = int.Parse(pData[7]);
			}
		}
	}
}