using System.Collections.Generic;

namespace Config
{
	public class LogEcpmEventConfigController : ConfigControllerBase<LogEcpmEventConfig>
	{
		protected override string GetFileName()
		{
			return "LogEcpmEventConfig";
		}

		protected override void AddPrimaryDict(LogEcpmEventConfig pModel)
		{
			primaryDict[pModel.ID.ToString()] = pModel;
		}
	}
}