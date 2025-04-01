using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameMode_Novel : GameMode
{
    public override GameModeType ModeType => GameModeType.Novel;

    public override void Archive()
    {
        StringBuilder tStringBuilder = new StringBuilder();

        PlayerPrefs.SetString(ARCHIVE_KEY, tStringBuilder.ToString());
    }

    public override bool IsGameSuccess()
    {
        throw new System.NotImplementedException();
    }

    public override void LoadArchive()
    {

    }


}