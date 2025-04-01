namespace Config
{
	public class CommonDefineController
	{
		const string FILE_NAME = "CommonDefine";

		public void LoadData(string pFolderPath)
		{
			TableFileReader tFileReader = new TableFileReader(pFolderPath + "/" + FILE_NAME);
			CommonDefine.noramlStartLevelID = int.Parse(tFileReader.ReadLine()[0]);
			CommonDefine.dailyStartLevelID = int.Parse(tFileReader.ReadLine()[0]);
			CommonDefine.activityStartLevelID = int.Parse(tFileReader.ReadLine()[0]);
			CommonDefine.loopStartLevelID = int.Parse(tFileReader.ReadLine()[0]);
			CommonDefine.shopFreeCount = int.Parse(tFileReader.ReadLine()[0]);
			CommonDefine.shopFreeTimeInterval = int.Parse(tFileReader.ReadLine()[0]);
			CommonDefine.healthFunllCount = int.Parse(tFileReader.ReadLine()[0]);
			CommonDefine.healthHarvestInterval = int.Parse(tFileReader.ReadLine()[0]);
		}
	}
}