namespace Config
{
	public class APPDefineController
	{
		const string FILE_NAME = "APPDefine";

		public void LoadData(string pFolderPath)
		{
			TableFileReader tFileReader = new TableFileReader(pFolderPath + "/" + FILE_NAME);
			APPDefine.pricacyPolicyURL = tFileReader.ReadLine()[0];
			APPDefine.termsOfServiceURL = tFileReader.ReadLine()[0];
			APPDefine.email = tFileReader.ReadLine()[0];
			APPDefine.BQURL = tFileReader.ReadLine()[0];
			APPDefine.serverFunctionURL = tFileReader.ReadLine()[0];
			APPDefine.serverAssetsURL = tFileReader.ReadLine()[0];
			APPDefine.notificationIconURL = tFileReader.ReadLine()[0];
		}
	}
}