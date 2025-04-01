using UnityEngine;

namespace GameLogic
{
    public class Blocker :TileBase
    {
        public BlockerType blockerType;
        public int blood = 1;
        public ParticleSystem effect;

        public virtual void ShowBlockerEffect()
        {
            
        }

        public virtual bool ClearBlockerEffect()
        {
            blood--;
            return false;
        }

        public virtual bool CanMove()
        {
            return blood <= 0;
        }
    }
}