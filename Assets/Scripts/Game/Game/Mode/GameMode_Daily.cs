using Config;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameMode_Daily : GameMode
{
    public override GameModeType ModeType => GameModeType.Daily;

    public int LevelId;
    public LevelConfig LevelConfig;


    public override void Archive()
    {
        StringBuilder tStringBuilder = new StringBuilder();
        tStringBuilder.Append(GameManager.Instance.BaseData.Serialize()).Append('|') ;
        //PlayerPrefs.SetString(ARCHIVE_KEY, tStringBuilder.ToString());
    }

    public override void LoadArchive()
    {
        //var tArchiveString = PlayerPrefs.GetString(ARCHIVE_KEY);    
        //GameManager.Instance.BaseData.Deserialize(tSplitedStrings[0]); 
    }
    public override bool IsGameSuccess()
    {
        throw new System.NotImplementedException();
    }

    public override bool StartGame(int pLevelID)
    {
        LevelId = pLevelID;
        LevelConfig = ConfigData.levelConfig.GetByPrimary(pLevelID); 

        return LevelConfig != null;
    }


}