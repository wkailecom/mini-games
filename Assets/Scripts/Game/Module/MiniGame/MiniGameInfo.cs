 
public class MiniGameData
{
    public int IssueNum { get; set; }
    public int CurLevel { get; set; } //当前关
    public int RecLevel { get; set; } //记录关 
    public int RetryCount { get; set; }//关卡重试数
    public bool IsNewGame { get; set; }//是否新游戏

    public MiniGameData()
    {
        IssueNum = 1;
        CurLevel = 1;
        RecLevel = 1;
        RetryCount = 0;
        IsNewGame = true;
    }
}