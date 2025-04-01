using System.Collections.Generic;

namespace Config
{
	public class LogAppEventConfigController : ConfigControllerBase<LogAppEventConfig>
	{
		protected override string GetFileName()
		{
			return "LogAppEventConfig";
		}

		protected override void AddPrimaryDict(LogAppEventConfig pModel)
		{
			primaryDict[pModel.ID.ToString()] = pModel;
		}

		protected override void AddIndexesDict(LogAppEventConfig pModel)
		{
			string tKey = GetIndexesKey(pModel.eventType.ToString());
			if (!indexesDict.ContainsKey(tKey))
			{
				indexesDict[tKey] = new List<LogAppEventConfig>();
			}

			indexesDict[tKey].Add(pModel);
		}
	}
}