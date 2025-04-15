using System.Collections.Generic;

namespace Config
{
	public class TripleLevelSeedConfigController : ConfigControllerBase<TripleLevelSeedConfig>
	{
		protected override string GetFileName()
		{
			return "TripleLevelSeedConfig";
		}

		protected override void AddPrimaryDict(TripleLevelSeedConfig pModel)
		{
			primaryDict[pModel.id.ToString()] = pModel;
		}
	}
}