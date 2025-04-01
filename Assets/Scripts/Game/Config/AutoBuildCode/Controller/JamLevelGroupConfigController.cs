using System.Collections.Generic;

namespace Config
{
	public class JamLevelGroupConfigController : ConfigControllerBase<JamLevelGroupConfig>
	{
		protected override string GetFileName()
		{
			return "JamLevelGroupConfig";
		}

		protected override void AddPrimaryDict(JamLevelGroupConfig pModel)
		{
			primaryDict[pModel.id.ToString()] = pModel;
		}
	}
}