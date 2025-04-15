namespace Config
{
	public class MiniMapConfig : ConfigModelBase
	{
		/// <summary>
		///id
		/// </summary>
		public string ID { get; private set; }
		/// <summary>
		///模式
		/// </summary>
		public int mode { get; private set; }
		/// <summary>
		///期数
		/// </summary>
		public int issue { get; private set; }
		/// <summary>
		///关卡
		/// </summary>
		public int level { get; private set; }
		/// <summary>
		///章节数
		/// </summary>
		public int Chapter { get; private set; }
		/// <summary>
		///关卡文件
		/// </summary>
		public string Chessboard { get; private set; }
		/// <summary>
		///图案数(BuOut)
		/// </summary>
		public int IconNumber { get; private set; }
		/// <summary>
		///限制时间(Triple)
		/// </summary>
		public int LimitTime { get; private set; }
		/// <summary>
		///关卡奖励
		/// </summary>
		public string LevelReward { get; private set; }
		/// <summary>
		///重玩花金币
		/// </summary>
		public int ReplayCoin { get; private set; }

		public override void ParseData(string[] pData)
		{
			if (pData == null || pData.Length < 10)
			{
				LogManager.LogError("MiniMapConfig.ParseData param wrong!");
				return;
			}

			if (!string.IsNullOrEmpty(pData[0]))
			{
				ID = pData[0];
			}
			if (!string.IsNullOrEmpty(pData[1]))
			{
				mode = int.Parse(pData[1]);
			}
			if (!string.IsNullOrEmpty(pData[2]))
			{
				issue = int.Parse(pData[2]);
			}
			if (!string.IsNullOrEmpty(pData[3]))
			{
				level = int.Parse(pData[3]);
			}
			if (!string.IsNullOrEmpty(pData[4]))
			{
				Chapter = int.Parse(pData[4]);
			}
			if (!string.IsNullOrEmpty(pData[5]))
			{
				Chessboard = pData[5];
			}
			if (!string.IsNullOrEmpty(pData[6]))
			{
				IconNumber = int.Parse(pData[6]);
			}
			if (!string.IsNullOrEmpty(pData[7]))
			{
				LimitTime = int.Parse(pData[7]);
			}
			if (!string.IsNullOrEmpty(pData[8]))
			{
				LevelReward = pData[8];
			}
			if (!string.IsNullOrEmpty(pData[9]))
			{
				ReplayCoin = int.Parse(pData[9]);
			}
		}
	}
}