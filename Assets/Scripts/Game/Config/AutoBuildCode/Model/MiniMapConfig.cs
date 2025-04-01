namespace Config
{
	public class MiniMapConfig : ConfigModelBase
	{
		/// <summary>
		///id
		/// </summary>
		public int ID { get; private set; }
		/// <summary>
		///模式
		/// </summary>
		public int mode { get; private set; }
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
		///难度标识
		/// </summary>
		public int HardMark { get; private set; }
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
				ID = int.Parse(pData[0]);
			}
			if (!string.IsNullOrEmpty(pData[1]))
			{
				mode = int.Parse(pData[1]);
			}
			if (!string.IsNullOrEmpty(pData[2]))
			{
				level = int.Parse(pData[2]);
			}
			if (!string.IsNullOrEmpty(pData[3]))
			{
				Chapter = int.Parse(pData[3]);
			}
			if (!string.IsNullOrEmpty(pData[4]))
			{
				Chessboard = pData[4];
			}
			if (!string.IsNullOrEmpty(pData[5]))
			{
				HardMark = int.Parse(pData[5]);
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