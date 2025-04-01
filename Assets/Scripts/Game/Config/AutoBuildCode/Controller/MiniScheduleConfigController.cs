using System.Collections.Generic;

namespace Config
{
	public class MiniScheduleConfigController : ConfigControllerBase<MiniScheduleConfig>
	{
		protected override string GetFileName()
		{
			return "MiniScheduleConfig";
		}

		protected override void AddPrimaryDict(MiniScheduleConfig pModel)
		{
			primaryDict[pModel.ID.ToString()] = pModel;
		}
	}
}