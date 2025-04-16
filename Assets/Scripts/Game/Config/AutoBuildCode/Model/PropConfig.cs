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
		///参数1
		/// </summary>
		public string param1 { get; private set; }
		/// <summary>
		///多语言对应Key
		/// </summary>
		public string nameTextKey { get; private set; }

		public override void ParseData(string[] pData)
		{
			if (pData == null || pData.Length < 5)
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
				param1 = pData[3];
			}
			if (!string.IsNullOrEmpty(pData[4]))
			{
				nameTextKey = pData[4];
			}
		}
	}
}