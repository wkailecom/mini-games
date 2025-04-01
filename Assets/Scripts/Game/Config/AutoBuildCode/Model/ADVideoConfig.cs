namespace Config
{
	public class ADVideoConfig : ConfigModelBase
	{
		/// <summary>
		///ID
		/// </summary>
		public int ID { get; private set; }
		/// <summary>
		///cd时间（分）
		/// </summary>
		public int cdTime { get; private set; }
		/// <summary>
		///每日次数
		/// </summary>
		public int everydayCount { get; private set; }
		/// <summary>
		///广告播放原因
		/// </summary>
		public string showReason { get; private set; }

		public override void ParseData(string[] pData)
		{
			if (pData == null || pData.Length < 4)
			{
				LogManager.LogError("ADVideoConfig.ParseData param wrong!");
				return;
			}

			if (!string.IsNullOrEmpty(pData[0]))
			{
				ID = int.Parse(pData[0]);
			}
			if (!string.IsNullOrEmpty(pData[1]))
			{
				cdTime = int.Parse(pData[1]);
			}
			if (!string.IsNullOrEmpty(pData[2]))
			{
				everydayCount = int.Parse(pData[2]);
			}
			if (!string.IsNullOrEmpty(pData[3]))
			{
				showReason = pData[3];
			}
		}
	}
}