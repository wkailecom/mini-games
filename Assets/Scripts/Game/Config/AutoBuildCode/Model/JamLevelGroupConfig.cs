namespace Config
{
	public class JamLevelGroupConfig : ConfigModelBase
	{
		/// <summary>
		///Id
		/// </summary>
		public int id { get; private set; }
		/// <summary>
		///注解
		/// </summary>
		public string note { get; private set; }
		/// <summary>
		///prefab
		/// </summary>
		public int prefab { get; private set; }
		/// <summary>
		///关卡类型
		/// </summary>
		public int levelType { get; private set; }
		/// <summary>
		///markCulling
		/// </summary>
		public int markCulling { get; private set; }
		/// <summary>
		///stepsExchange
		/// </summary>
		public string stepsExchange { get; private set; }
		/// <summary>
		///threshold
		/// </summary>
		public int threshold { get; private set; }
		/// <summary>
		///outset1
		/// </summary>
		public string outset1 { get; private set; }
		/// <summary>
		///direction1
		/// </summary>
		public string direction1 { get; private set; }
		/// <summary>
		///outset2
		/// </summary>
		public string outset2 { get; private set; }
		/// <summary>
		///direction2
		/// </summary>
		public string direction2 { get; private set; }

		public override void ParseData(string[] pData)
		{
			if (pData == null || pData.Length < 11)
			{
				LogManager.LogError("JamLevelGroupConfig.ParseData param wrong!");
				return;
			}

			if (!string.IsNullOrEmpty(pData[0]))
			{
				id = int.Parse(pData[0]);
			}
			if (!string.IsNullOrEmpty(pData[1]))
			{
				note = pData[1];
			}
			if (!string.IsNullOrEmpty(pData[2]))
			{
				prefab = int.Parse(pData[2]);
			}
			if (!string.IsNullOrEmpty(pData[3]))
			{
				levelType = int.Parse(pData[3]);
			}
			if (!string.IsNullOrEmpty(pData[4]))
			{
				markCulling = int.Parse(pData[4]);
			}
			if (!string.IsNullOrEmpty(pData[5]))
			{
				stepsExchange = pData[5];
			}
			if (!string.IsNullOrEmpty(pData[6]))
			{
				threshold = int.Parse(pData[6]);
			}
			if (!string.IsNullOrEmpty(pData[7]))
			{
				outset1 = pData[7];
			}
			if (!string.IsNullOrEmpty(pData[8]))
			{
				direction1 = pData[8];
			}
			if (!string.IsNullOrEmpty(pData[9]))
			{
				outset2 = pData[9];
			}
			if (!string.IsNullOrEmpty(pData[10]))
			{
				direction2 = pData[10];
			}
		}
	}
}