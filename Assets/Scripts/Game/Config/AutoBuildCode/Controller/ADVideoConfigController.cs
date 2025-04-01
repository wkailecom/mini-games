using System.Collections.Generic;

namespace Config
{
	public class ADVideoConfigController : ConfigControllerBase<ADVideoConfig>
	{
		protected override string GetFileName()
		{
			return "ADVideoConfig";
		}

		protected override void AddPrimaryDict(ADVideoConfig pModel)
		{
			primaryDict[pModel.ID.ToString()] = pModel;
		}

		protected override void AddIndexesDict(ADVideoConfig pModel)
		{
			string tKey = GetIndexesKey(pModel.showReason.ToString());
			if (!indexesDict.ContainsKey(tKey))
			{
				indexesDict[tKey] = new List<ADVideoConfig>();
			}

			indexesDict[tKey].Add(pModel);
		}
	}
}