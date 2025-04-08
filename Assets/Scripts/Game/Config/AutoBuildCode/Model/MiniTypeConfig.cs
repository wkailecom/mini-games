namespace Config
{
	public class MiniTypeConfig : ConfigModelBase
	{
		/// <summary>
		///id
		/// </summary>
		public int ID { get; private set; }
		/// <summary>
		///模式
		/// </summary>
		public string gameType { get; private set; }
		/// <summary>
		///场景名
		/// </summary>
		public string sceneName { get; private set; }
		/// <summary>
		///标题名
		/// </summary>
		public string titleName { get; private set; }
		/// <summary>
		///封面图
		/// </summary>
		public string coverIcon { get; private set; }
		/// <summary>
		///入口图
		/// </summary>
		public string enterIcon { get; private set; }
		/// <summary>
		///动画icon（预制体）
		/// </summary>
		public string animIcon { get; private set; }
		/// <summary>
		///动画标题（预制体）
		/// </summary>
		public string animTitle { get; private set; }

		public override void ParseData(string[] pData)
		{
			if (pData == null || pData.Length < 8)
			{
				LogManager.LogError("MiniTypeConfig.ParseData param wrong!");
				return;
			}

			if (!string.IsNullOrEmpty(pData[0]))
			{
				ID = int.Parse(pData[0]);
			}
			if (!string.IsNullOrEmpty(pData[1]))
			{
				gameType = pData[1];
			}
			if (!string.IsNullOrEmpty(pData[2]))
			{
				sceneName = pData[2];
			}
			if (!string.IsNullOrEmpty(pData[3]))
			{
				titleName = pData[3];
			}
			if (!string.IsNullOrEmpty(pData[4]))
			{
				coverIcon = pData[4];
			}
			if (!string.IsNullOrEmpty(pData[5]))
			{
				enterIcon = pData[5];
			}
			if (!string.IsNullOrEmpty(pData[6]))
			{
				animIcon = pData[6];
			}
			if (!string.IsNullOrEmpty(pData[7]))
			{
				animTitle = pData[7];
			}
		}
	}
}