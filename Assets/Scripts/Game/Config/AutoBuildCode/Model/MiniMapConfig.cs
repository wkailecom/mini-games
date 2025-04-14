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
		///关卡奖励
		/// </summary>
		public string LevelReward { get; private set; }
		/// <summary>
		///重玩花金币
		/// </summary>
		public int ReplayCoin { get; private set; }

		public override void ParseData(string[] pData)
		{
			if (pData == null || pData.Length < 8)
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
				LogManager.LogError(Chessboard);
			}
			if (!string.IsNullOrEmpty(pData[6]))
			{
				LevelReward = pData[6];
			}
			if (!string.IsNullOrEmpty(pData[7]))
			{
				ReplayCoin = int.Parse(pData[7]);
			}
		}
	}
}