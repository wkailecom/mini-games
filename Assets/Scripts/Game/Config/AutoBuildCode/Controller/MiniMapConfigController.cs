using System.Collections.Generic;

namespace Config
{
	public class MiniMapConfigController : ConfigControllerBase<MiniMapConfig>
	{
		protected override string GetFileName()
		{
			return "MiniMapConfig";
		}

		protected override void AddPrimaryDict(MiniMapConfig pModel)
		{
			primaryDict[pModel.ID] = pModel;
		}

		protected override void AddIndexesDict(MiniMapConfig pModel)
		{
			string tKey = GetIndexesKey(pModel.mode.ToString(), pModel.issue.ToString());
			if (!indexesDict.ContainsKey(tKey))
			{
				indexesDict[tKey] = new List<MiniMapConfig>();
			}

			indexesDict[tKey].Add(pModel);
		}
	}
}