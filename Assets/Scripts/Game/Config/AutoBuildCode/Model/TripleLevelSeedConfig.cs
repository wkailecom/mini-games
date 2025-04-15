namespace Config
{
	public class TripleLevelSeedConfig : ConfigModelBase
	{
		/// <summary>
		///种子id
		/// </summary>
		public int id { get; private set; }
		/// <summary>
		///难度1时长(s)
		/// </summary>
		public int diff_1 { get; private set; }
		/// <summary>
		///难度2时长(s)
		/// </summary>
		public int diff_2 { get; private set; }
		/// <summary>
		///难度3时长(s)
		/// </summary>
		public int diff_3 { get; private set; }
		/// <summary>
		///难度4时长(s)
		/// </summary>
		public int diff_4 { get; private set; }
		/// <summary>
		///难度5时长(s)
		/// </summary>
		public int diff_5 { get; private set; }

		public override void ParseData(string[] pData)
		{
			if (pData == null || pData.Length < 6)
			{
				LogManager.LogError("TripleLevelSeedConfig.ParseData param wrong!");
				return;
			}

			if (!string.IsNullOrEmpty(pData[0]))
			{
				id = int.Parse(pData[0]);
			}
			if (!string.IsNullOrEmpty(pData[1]))
			{
				diff_1 = int.Parse(pData[1]);
			}
			if (!string.IsNullOrEmpty(pData[2]))
			{
				diff_2 = int.Parse(pData[2]);
			}
			if (!string.IsNullOrEmpty(pData[3]))
			{
				diff_3 = int.Parse(pData[3]);
			}
			if (!string.IsNullOrEmpty(pData[4]))
			{
				diff_4 = int.Parse(pData[4]);
			}
			if (!string.IsNullOrEmpty(pData[5]))
			{
				diff_5 = int.Parse(pData[5]);
			}
		}
	}
}