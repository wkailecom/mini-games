using System.Collections.Generic;

namespace Config
{
	public class TextsConfigController : ConfigControllerBase<TextsConfig>
	{
		protected override string GetFileName()
		{
			return "TextsConfig";
		}

		protected override void AddPrimaryDict(TextsConfig pModel)
		{
			primaryDict[pModel.Key] = pModel;
		}
	}
}