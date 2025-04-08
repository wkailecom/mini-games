using System.Collections.Generic;

namespace Config
{
	public class MiniTypeConfigController : ConfigControllerBase<MiniTypeConfig>
	{
		protected override string GetFileName()
		{
			return "MiniTypeConfig";
		}

		protected override void AddPrimaryDict(MiniTypeConfig pModel)
		{
			primaryDict[pModel.ID.ToString()] = pModel;
		}
	}
}