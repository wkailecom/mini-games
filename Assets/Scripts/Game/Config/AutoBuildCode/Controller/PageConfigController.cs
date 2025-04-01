using System.Collections.Generic;

namespace Config
{
	public class PageConfigController : ConfigControllerBase<PageConfig>
	{
		protected override string GetFileName()
		{
			return "PageConfig";
		}

		protected override void AddPrimaryDict(PageConfig pModel)
		{
			primaryDict[pModel.pageID.ToString()] = pModel;
		}
	}
}