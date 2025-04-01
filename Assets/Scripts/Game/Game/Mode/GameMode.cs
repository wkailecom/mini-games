using UnityEngine;

public abstract class GameMode
{
    protected virtual string ARCHIVE_KEY => "ArchiveKey_" + ModeType;

    public abstract GameModeType ModeType { get; }
    public virtual void EnterMode() { }
    public virtual void ExitMode() { }
    public virtual bool StartGame(int pLevelID) { return true; }
    public virtual void FinishGame() { Reset(); }
    public virtual void Reset() { ClearArchive(); }
    public virtual bool HasArchive => !string.IsNullOrEmpty(PlayerPrefs.GetString(ARCHIVE_KEY));
    public abstract void Archive();
    public abstract void LoadArchive();
    public abstract bool IsGameSuccess();
    public virtual bool IsGameFailed() { return false; }
    protected void ClearArchive() { PlayerPrefs.SetString(ARCHIVE_KEY, string.Empty); }

}