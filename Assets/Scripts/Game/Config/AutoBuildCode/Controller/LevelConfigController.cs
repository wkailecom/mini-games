using System.Collections.Generic;

namespace Config
{
	public class LevelConfigController : ConfigControllerBase<LevelConfig>
	{
		protected override string GetFileName()
		{
			return "LevelConfig";
		}

		protected override void AddPrimaryDict(LevelConfig pModel)
		{
			primaryDict[pModel.ID.ToString()] = pModel;
		}

		protected override void AddIndexesDict(LevelConfig pModel)
		{
			string tKey = GetIndexesKey(pModel.mode.ToString());
			if (!indexesDict.ContainsKey(tKey))
			{
				indexesDict[tKey] = new List<LevelConfig>();
			}

			indexesDict[tKey].Add(pModel);
		}
	}
}