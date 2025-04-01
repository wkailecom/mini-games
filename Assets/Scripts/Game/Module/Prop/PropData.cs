using Config;

public class PropData
{
    public PropID ID;
    public int Count;

    public PropData(PropID pID, int pCount = 0)
    {
        ID = pID;
        Count = pCount;
    }

    public void AddCount(int pCount)
    {
        Count += pCount;
    }

    public bool Expend(int pCount)
    {
        if (pCount <= 0)
        {
            LogManager.LogError("Expend prop count < 0");
            return false;
        }

        if (Count < pCount)
        {
            return false;
        }

        Count -= pCount;
        return true;
    }
}