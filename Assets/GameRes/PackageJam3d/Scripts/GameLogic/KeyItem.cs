using UnityEngine;

namespace GameLogic
{
    public enum KeysColor
    {
        Gold,
        Silver,
        Copper,
        Iron,
        Tin,
        Platinum,
    }
    
    
    public class KeyItem : TileItem
    {
        [HideInInspector]public LockItem lockItem;

        public KeysColor keysColor;
        public Animator animator;
        
        protected override void OnStateChanged(VirusState value)
        {
            if (value == VirusState.Reborn)
            {
                animator.Play("Idle");
            }
        }

        public override void AssignColor(int color, int order)
        {
            
        }

        public override void DoPath()
        {
            
        }

        public override void Unlock()
        {
            animator.Play("Key");
            VirusState = VirusState.Dead;
            
            var tile = JamManager.GetSingleton().GetTerrainTile(index);
            tile.ClearGridItem(false);
            lockItem.Unlock();
        }

        public override void Undo()
        {
            
        }

        public override bool IsUndoing()
        {
            return false;
        }
    }
}