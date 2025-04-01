using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TileGameMode
{
    string ARCHIVE_KEY => "ArchiveKey_TileGame";

    public bool HasArchive => !string.IsNullOrEmpty(PlayerPrefs.GetString(ARCHIVE_KEY));
   
    public void Reset() { PlayerPrefs.SetString(ARCHIVE_KEY, string.Empty); }

    public void FinishGame() { Reset(); }

    public void Archive()
    {
        StringBuilder tStringBuilder = new StringBuilder();
        tStringBuilder.Append(TileGameManager.Instance.BaseData.Serialize()).Append('|')
                      .Append(TileGameManager.Instance.FloorViewData.Serialize()).Append('|')
                      .Append(TileGameManager.Instance.SlotViewData.Serialize());

        PlayerPrefs.SetString(ARCHIVE_KEY, tStringBuilder.ToString());
    }
    public void LoadArchive()
    {
        var tArchiveString = PlayerPrefs.GetString(ARCHIVE_KEY);
        if (string.IsNullOrEmpty(tArchiveString))
        {
            return;
        }

        var tSplitedStrings = tArchiveString.Split('|');
        if (tSplitedStrings.Length < 3)
        {
            LogManager.LogError($"TileGameMode.LoadArchive: invalid archiveString -- {tArchiveString}");
            return;
        }

        TileGameManager.Instance.BaseData.Deserialize(tSplitedStrings[0]);
        TileGameManager.Instance.FloorViewData.Deserialize(tSplitedStrings[1]);
        TileGameManager.Instance.SlotViewData.Deserialize(tSplitedStrings[2]);

    }

    public bool StartGame(bool pIsNewGame, string pGameParams)
    {
        int tLevel = pGameParams.ToInt();

        if (pIsNewGame)
        {
            TileGameManager.Instance.FloorViewData.GetFloorsData(tLevel);
        }

        return true;
    }

    public bool IsGameFailed()
    {
        return !TileGameManager.Instance.SlotViewData.IsCanPlace();
    }

    public bool IsGameSuccess()
    {
        var tFloorViewData = TileGameManager.Instance.FloorViewData.floorsData;
        foreach (var tFloorData in tFloorViewData)
        {
            foreach (var tCellData in tFloorData.floorData)
            {
                if (tCellData.State != TileCellState.Hide)
                {
                    return false;
                }
            }
        }
        return true;
    }

}
