using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class LockItem : TileItem
    {
        [HideInInspector]
        public KeyItem keyItem;
        
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
            if(!keyItem)
                return;
            if (VirusState == VirusState.Dead || keyItem.VirusState != VirusState.Dead)
                return;

            animator.Play("Lock_Open");
            var tile = JamManager.GetSingleton().GetTerrainTile(index);
            var pathList = JamManager.GetSingleton().StartNavigation(index);
            if (pathList == null)
            {
                tile.OnPreUnlock(true);
                return;
            }
            tile.ClearGridItem(false);
            VirusState = VirusState.Dead;
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