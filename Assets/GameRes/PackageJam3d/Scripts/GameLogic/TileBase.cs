using UnityEngine;

namespace GameLogic
{
    public abstract class TileBase :  MonoBehaviour
    {
        /// <summary>
        /// grid index row * gridSize + col
        /// </summary>
        public int index;

        /// <summary>
        /// navigation block 
        /// </summary>
        public bool walkable = true ;

        public static explicit operator int(TileBase t) => t.index;
    }
}