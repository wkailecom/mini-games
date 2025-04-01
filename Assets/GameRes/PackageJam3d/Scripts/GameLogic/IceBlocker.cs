namespace GameLogic
{
    public class IceBlocker :Blocker
    {
        public override bool ClearBlockerEffect()
        {
            blood--;
            
            return false;
        }
    }
}