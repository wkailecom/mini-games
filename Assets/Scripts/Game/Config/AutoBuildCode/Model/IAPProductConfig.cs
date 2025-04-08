namespace Config
{
	public class IAPProductConfig : ConfigModelBase
	{
		/// <summary>
		///商品ID
		/// </summary>
		public string productID { get; private set; }
		/// <summary>
		///商品来源
		/// </summary>
		public int source { get; private set; }
		/// <summary>
		///商品类型
		/// </summary>
		public int productType { get; private set; }
		/// <summary>
		///价格
		/// </summary>
		public float price { get; private set; }
		/// <summary>
		///折扣
		/// </summary>
		public int discount { get; private set; }
		/// <summary>
		///道具ID
		/// </summary>
		public int[] propsID { get; private set; }
		/// <summary>
		///道具数量
		/// </summary>
		public int[] propsCount { get; private set; }
		/// <summary>
		///种类
		/// </summary>
		public int category { get; private set; }
		/// <summary>
		///商店id
		/// </summary>
		public int shopId { get; private set; }
		/// <summary>
		///查询id
		/// </summary>
		public int queryId { get; private set; }
		/// <summary>
		///图标
		/// </summary>
		public string icon { get; private set; }
		/// <summary>
		///文本ID
		/// </summary>
		public string textID { get; private set; }

		public override void ParseData(string[] pData)
		{
			if (pData == null || pData.Length < 12)
			{
				LogManager.LogError("IAPProductConfig.ParseData param wrong!");
				return;
			}

			if (!string.IsNullOrEmpty(pData[0]))
			{
				productID = pData[0];
			}
			if (!string.IsNullOrEmpty(pData[1]))
			{
				source = int.Parse(pData[1]);
			}
			if (!string.IsNullOrEmpty(pData[2]))
			{
				productType = int.Parse(pData[2]);
			}
			if (!string.IsNullOrEmpty(pData[3]))
			{
				price = float.Parse(pData[3]);
			}
			if (!string.IsNullOrEmpty(pData[4]))
			{
				discount = int.Parse(pData[4]);
			}
			if (!string.IsNullOrEmpty(pData[5]))
			{
				propsID = TableParser.ParseArrayData<int>(pData[5]);
			}
			if (!string.IsNullOrEmpty(pData[6]))
			{
				propsCount = TableParser.ParseArrayData<int>(pData[6]);
			}
			if (!string.IsNullOrEmpty(pData[7]))
			{
				category = int.Parse(pData[7]);
			}
			if (!string.IsNullOrEmpty(pData[8]))
			{
				shopId = int.Parse(pData[8]);
			}
			if (!string.IsNullOrEmpty(pData[9]))
			{
				queryId = int.Parse(pData[9]);
			}
			if (!string.IsNullOrEmpty(pData[10]))
			{
				icon = pData[10];
			}
			if (!string.IsNullOrEmpty(pData[11]))
			{
				textID = pData[11];
			}
		}
	}
}