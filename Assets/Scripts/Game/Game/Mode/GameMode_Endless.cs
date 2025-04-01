using Config;
using System.Text;
using UnityEngine;

public class GameMode_Endless : GameMode
{
    public override GameModeType ModeType => GameModeType.Endless;

    public LevelConfig LevelConfig;

    public override void Archive()
    {
        StringBuilder tStringBuilder = new StringBuilder();
        tStringBuilder.Append(GameManager.Instance.BaseData.Serialize()).Append('|');

        PlayerPrefs.SetString(ARCHIVE_KEY, tStringBuilder.ToString());
        //Debug.Log(tStringBuilder.ToString());
    }

    public override void LoadArchive()
    {
        var tArchiveString = PlayerPrefs.GetString(ARCHIVE_KEY);
        if (string.IsNullOrEmpty(tArchiveString))
        {
            return;
        }

        var tSplitedStrings = tArchiveString.Split('|');
        if (tSplitedStrings.Length < 3)
        {
            LogManager.LogError($"GameMode_Endless.LoadArchive: invalid archiveString -- {tArchiveString}");
            return;
        }

        GameManager.Instance.BaseData.Deserialize(tSplitedStrings[0]); 
    }

    public override bool IsGameSuccess()
    {
        return false;
    }

    public override bool StartGame(int pLevelID)
    {
        //var tConfigId = pLevelID + CommonDefine.noramlStartLevelID;
        //EndlessConfig = ConfigData.endlessLevelConfig.GetByPrimary(pLevelID); 
         

        return LevelConfig != null;
    }

}