namespace Config
{
	public class TextsConfig : ConfigModelBase
	{
		/// <summary>
		///文本Key
		/// </summary>
		public string Key { get; private set; }
		/// <summary>
		///ID
		/// </summary>
		public int ID { get; private set; }
		/// <summary>
		///中文
		/// </summary>
		public string ZH { get; private set; }
		/// <summary>
		///英语(美国)
		/// </summary>
		public string EN_US { get; private set; }
		/// <summary>
		///葡萄牙语（巴西）
		/// </summary>
		public string PT_BR { get; private set; }
		/// <summary>
		///德语（德国）
		/// </summary>
		public string DE_DE { get; private set; }
		/// <summary>
		///法语
		/// </summary>
		public string FR { get; private set; }
		/// <summary>
		///日语
		/// </summary>
		public string JA { get; private set; }

		public override void ParseData(string[] pData)
		{
			if (pData == null || pData.Length < 8)
			{
				LogManager.LogError("TextsConfig.ParseData param wrong!");
				return;
			}

			if (!string.IsNullOrEmpty(pData[0]))
			{
				Key = pData[0];
			}
			if (!string.IsNullOrEmpty(pData[1]))
			{
				ID = int.Parse(pData[1]);
			}
			if (!string.IsNullOrEmpty(pData[2]))
			{
				ZH = pData[2];
			}
			if (!string.IsNullOrEmpty(pData[3]))
			{
				EN_US = pData[3];
			}
			if (!string.IsNullOrEmpty(pData[4]))
			{
				PT_BR = pData[4];
			}
			if (!string.IsNullOrEmpty(pData[5]))
			{
				DE_DE = pData[5];
			}
			if (!string.IsNullOrEmpty(pData[6]))
			{
				FR = pData[6];
			}
			if (!string.IsNullOrEmpty(pData[7]))
			{
				JA = pData[7];
			}
		}
	}
}