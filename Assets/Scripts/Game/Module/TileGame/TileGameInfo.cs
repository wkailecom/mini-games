using System.Collections.Generic;
/// <summary>
/// Tile游戏信息
/// </summary>
public class TileInfo
{
    /// 本地当前期数
    /// </summary>
    public int IssueNum { get; set; }
    /// <summary>
    /// 体力恢复时间
    /// </summary>
    public long HeartRecoverTime { get; set; }
}

public class TileGameInfo
{
    public int IssueNum { get; internal set; }
    public int CurrentLevel { get; internal set; }
    public int CurCheckLevel { get; internal set; }
    public int RetryCount { get; internal set; }
    public long PlayTime { get; internal set; }
    public int MaxReachLevel { get; internal set; }
    public List<int> OpenedGifts { get; internal set; }
    public long EndTime { get; internal set; }
}