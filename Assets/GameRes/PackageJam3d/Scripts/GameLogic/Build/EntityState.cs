using System;

[Serializable]
public class EntityOpenState
{
    public int entryId;
    public int state;
}

[Serializable]
public class EntityOpenStateArray
{
    public EntityOpenState[] openStates;
}