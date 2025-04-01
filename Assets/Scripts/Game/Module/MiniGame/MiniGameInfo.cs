
public class MiniGameInfoData
{
    public int IssueNum { get; set; }
    public bool IsComplete { get; set; }
    public int CurrentLevel { get; set; }
    public int RecordLevel { get; set; }
    public int RetryCount { get; set; }

    public MiniGameInfoData()
    {
        IssueNum = 1;
        IsComplete = false;
        CurrentLevel = 1;
        RecordLevel = 1;
        RetryCount = 0;
    }
}
