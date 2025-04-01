namespace Config
{
	public class PropConfig : ConfigModelBase
	{
		/// <summary>
		///ID
		/// </summary>
		public int ID { get; private set; }
		/// <summary>
		///名称ID
		/// </summary>
		public string propID { get; private set; }
		/// <summary>
		///图标
		/// </summary>
		public string icon { get; private set; }
		/// <summary>
		///多语言对应Key
		/// </summary>
		public string nameTextKey { get; private set; }

		public override void ParseData(string[] pData)
		{
			if (pData == null || pData.Length < 4)
			{
				LogManager.LogError("PropConfig.ParseData param wrong!");
				return;
			}

			if (!string.IsNullOrEmpty(pData[0]))
			{
				ID = int.Parse(pData[0]);
			}
			if (!string.IsNullOrEmpty(pData[1]))
			{
				propID = pData[1];
			}
			if (!string.IsNullOrEmpty(pData[2]))
			{
				icon = pData[2];
			}
			if (!string.IsNullOrEmpty(pData[3]))
			{
				nameTextKey = pData[3];
			}
		}
	}
}