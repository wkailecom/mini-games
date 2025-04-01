using System.Collections.Generic;

namespace Config
{
	public class PropConfigController : ConfigControllerBase<PropConfig>
	{
		protected override string GetFileName()
		{
			return "PropConfig";
		}

		protected override void AddPrimaryDict(PropConfig pModel)
		{
			primaryDict[pModel.ID.ToString()] = pModel;
		}
	}
}