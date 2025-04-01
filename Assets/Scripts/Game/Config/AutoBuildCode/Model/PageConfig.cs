namespace Config
{
	public class PageConfig : ConfigModelBase
	{
		/// <summary>
		///界面ID
		/// </summary>
		public int pageID { get; private set; }
		/// <summary>
		///预设路径
		/// </summary>
		public string prefabPath { get; private set; }
		/// <summary>
		///是否为全屏界面
		/// </summary>
		public bool isFullPage { get; private set; }
		/// <summary>
		///是否保存到历史记录
		/// </summary>
		public bool saveToHistory { get; private set; }
		/// <summary>
		///关闭后是否需要缓存起来
		/// </summary>
		public bool needCache { get; private set; }
		/// <summary>
		///遮罩Alpha值
		/// </summary>
		public float maskAlpha { get; private set; }
		/// <summary>
		///遮罩颜色
		/// </summary>
		public string maskColor { get; private set; }

		public override void ParseData(string[] pData)
		{
			if (pData == null || pData.Length < 7)
			{
				LogManager.LogError("PageConfig.ParseData param wrong!");
				return;
			}

			if (!string.IsNullOrEmpty(pData[0]))
			{
				pageID = int.Parse(pData[0]);
			}
			if (!string.IsNullOrEmpty(pData[1]))
			{
				prefabPath = pData[1];
			}
			if (!string.IsNullOrEmpty(pData[2]))
			{
				isFullPage = !pData[2].Equals("0");
			}
			if (!string.IsNullOrEmpty(pData[3]))
			{
				saveToHistory = !pData[3].Equals("0");
			}
			if (!string.IsNullOrEmpty(pData[4]))
			{
				needCache = !pData[4].Equals("0");
			}
			if (!string.IsNullOrEmpty(pData[5]))
			{
				maskAlpha = float.Parse(pData[5]);
			}
			if (!string.IsNullOrEmpty(pData[6]))
			{
				maskColor = pData[6];
			}
		}
	}
}