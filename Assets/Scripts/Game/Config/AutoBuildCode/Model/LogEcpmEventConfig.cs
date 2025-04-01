namespace Config
{
	public class LogEcpmEventConfig : ConfigModelBase
	{
		/// <summary>
		///ID
		/// </summary>
		public int ID { get; private set; }
		/// <summary>
		///事件名
		/// </summary>
		public string eventName { get; private set; }
		/// <summary>
		///目标值(美金)
		/// </summary>
		public int targetValue { get; private set; }
		/// <summary>
		///上报值
		/// </summary>
		public double reportValue { get; private set; }
		/// <summary>
		///时间限制(天)
		/// </summary>
		public int limitTime { get; private set; }
		/// <summary>
		///目标国家
		/// </summary>
		public string[] targetCountry { get; private set; }
		/// <summary>
		///安卓上报平台
		/// </summary>
		public bool[] andriodPlatform { get; private set; }
		/// <summary>
		///IOS上报平台
		/// </summary>
		public bool[] iosPlatform { get; private set; }
		/// <summary>
		///是否一次
		/// </summary>
		public int isOnce { get; private set; }
		/// <summary>
		///是否同步BQ
		/// </summary>
		public int reportBQ { get; private set; }

		public override void ParseData(string[] pData)
		{
			if (pData == null || pData.Length < 10)
			{
				LogManager.LogError("LogEcpmEventConfig.ParseData param wrong!");
				return;
			}

			if (!string.IsNullOrEmpty(pData[0]))
			{
				ID = int.Parse(pData[0]);
			}
			if (!string.IsNullOrEmpty(pData[1]))
			{
				eventName = pData[1];
			}
			if (!string.IsNullOrEmpty(pData[2]))
			{
				targetValue = int.Parse(pData[2]);
			}
			if (!string.IsNullOrEmpty(pData[3]))
			{
				reportValue = double.Parse(pData[3]);
			}
			if (!string.IsNullOrEmpty(pData[4]))
			{
				limitTime = int.Parse(pData[4]);
			}
			if (!string.IsNullOrEmpty(pData[5]))
			{
				targetCountry = TableParser.ParseArrayData(pData[5]);
			}
			if (!string.IsNullOrEmpty(pData[6]))
			{
				andriodPlatform = TableParser.ParseArrayData<bool>(pData[6]);
			}
			if (!string.IsNullOrEmpty(pData[7]))
			{
				iosPlatform = TableParser.ParseArrayData<bool>(pData[7]);
			}
			if (!string.IsNullOrEmpty(pData[8]))
			{
				isOnce = int.Parse(pData[8]);
			}
			if (!string.IsNullOrEmpty(pData[9]))
			{
				reportBQ = int.Parse(pData[9]);
			}
		}
	}
}