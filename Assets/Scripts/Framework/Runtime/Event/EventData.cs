using LLFramework;


public interface IEventData
{

}

public class EventData : PoolObjectBase
{
    public int EventID { get; private set; }

    public EventData(int pEventID)
    {
        EventID = pEventID;
    }
}
