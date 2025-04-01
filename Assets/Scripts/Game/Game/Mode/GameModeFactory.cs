using LLFramework;
using System.Collections.Generic;

public class GameModeFactory : Singleton<GameModeFactory>
{
    readonly Dictionary<int, GameMode> mGameModes = new();

    public GameMode GetGameMode(GameModeType pType)
    {
        if (mGameModes.TryGetValue((int)pType, out var tGameMode))
        {
            return tGameMode;
        }

        tGameMode = pType switch
        {
            GameModeType.Endless => new GameMode_Endless(),
            GameModeType.Daily => new GameMode_Daily(),
            GameModeType.Novel => new GameMode_Novel(),
            _ => null,
        };
        if (tGameMode == null)
        {
            LogManager.LogError($"GameModeFactory.GetGameMode: Invalid GameModeType {pType}");
            return GetGameMode(GameModeType.Endless);
        }

        mGameModes.Add((int)pType, tGameMode);
        return tGameMode;
    }
}


public enum GameModeType
{
    Endless,
    Daily,
    Novel,

    Guide,
}