using System.Collections.Generic;

namespace Config
{
	public class IAPProductConfigController : ConfigControllerBase<IAPProductConfig>
	{
		protected override string GetFileName()
		{
			return "IAPProductConfig";
		}

		protected override void AddPrimaryDict(IAPProductConfig pModel)
		{
			primaryDict[pModel.productID] = pModel;
		}

		protected override void AddIndexesDict(IAPProductConfig pModel)
		{
			string tKey = GetIndexesKey(pModel.source.ToString());
			if (!indexesDict.ContainsKey(tKey))
			{
				indexesDict[tKey] = new List<IAPProductConfig>();
			}

			indexesDict[tKey].Add(pModel);
		}
	}
}