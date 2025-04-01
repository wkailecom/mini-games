namespace Config
{
	public class LevelConfig : ConfigModelBase
	{
		/// <summary>
		///关卡id
		/// </summary>
		public int ID { get; private set; }
		/// <summary>
		///模式
		/// </summary>
		public int mode { get; private set; }
		/// <summary>
		///数据ID
		/// </summary>
		public int dataID { get; private set; }
		/// <summary>
		///语言
		/// </summary>
		public int language { get; private set; }
		/// <summary>
		///原始文本
		/// </summary>
		public string originalText { get; private set; }
		/// <summary>
		///加密文本
		/// </summary>
		public string encryptedText { get; private set; }
		/// <summary>
		///作者
		/// </summary>
		public string author { get; private set; }
		/// <summary>
		///作者信息
		/// </summary>
		public string authorInfo { get; private set; }
		/// <summary>
		///锁数量
		/// </summary>
		public int lockerCount { get; private set; }
		/// <summary>
		///单双锁比
		/// </summary>
		public int[] lockRatio { get; private set; }
		/// <summary>
		///难度
		/// </summary>
		public int difficulty { get; private set; }
		/// <summary>
		///键可灰置
		/// </summary>
		public bool keyDisabled { get; private set; }

		public override void ParseData(string[] pData)
		{
			if (pData == null || pData.Length < 12)
			{
				LogManager.LogError("LevelConfig.ParseData param wrong!");
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
				dataID = int.Parse(pData[2]);
			}
			if (!string.IsNullOrEmpty(pData[3]))
			{
				language = int.Parse(pData[3]);
			}
			if (!string.IsNullOrEmpty(pData[4]))
			{
				originalText = pData[4];
			}
			if (!string.IsNullOrEmpty(pData[5]))
			{
				encryptedText = pData[5];
			}
			if (!string.IsNullOrEmpty(pData[6]))
			{
				author = pData[6];
			}
			if (!string.IsNullOrEmpty(pData[7]))
			{
				authorInfo = pData[7];
			}
			if (!string.IsNullOrEmpty(pData[8]))
			{
				lockerCount = int.Parse(pData[8]);
			}
			if (!string.IsNullOrEmpty(pData[9]))
			{
				lockRatio = TableParser.ParseArrayData<int>(pData[9]);
			}
			if (!string.IsNullOrEmpty(pData[10]))
			{
				difficulty = int.Parse(pData[10]);
			}
			if (!string.IsNullOrEmpty(pData[11]))
			{
				keyDisabled = !pData[11].Equals("0");
			}
		}
	}
}