using UnityEngine;

namespace GameLogic
{
    public enum TileItemType
    {
        /// <summary>
        /// 装饰植物 其他无意义摆件
        /// </summary>
        Foliage,

        /// <summary>
        /// 桶或者其他伪装
        /// </summary>
        Fake,

        /// <summary>
        /// 销子
        /// </summary>
        Pin,

        /// <summary>
        /// 可以产出消除物的道具
        /// </summary>
        Spawn,

        /// <summary>
        /// 钥匙，和锁成对儿出现
        /// </summary>
        Key,
        
        /// <summary>
        /// 锁，和钥匙成对儿出现
        /// </summary>
        Lock,
        
        /// <summary>
        /// 消除物
        /// </summary>
        Virus,
    }

    public enum VirusState
    {
        Hide = 0,
        Fake = 1,
        Born,
        Idle,
        Shuffle,
        Moving,
        WaitToPlace,
        Place,
        Replace,
        Disappear,
        Dead,
        Reborn,
    }

    public enum SourceType
    {
        Independence,
        Fake,
        Spawn,
    }

    /// <summary>
    /// gameplay object
    /// </summary>
    public abstract class TileItem : TileBase
    {
        public TileItemType tileItemType;

        public int virusColor;

        public int capacity = 1;

        public SourceType sourceType;

        private VirusState _virusState;
        public VirusState VirusState
        {
            get => _virusState;
            set
            {
                if (Application.isPlaying)
                {
                    _virusState = value;
                    OnStateChanged(value);
                }
            }
        }
        
        protected abstract void OnStateChanged(VirusState value);

        public abstract void AssignColor(int color,int order);

        public abstract void DoPath();
        
        public abstract void Unlock();
        
        public abstract void Undo();
        
        public abstract bool IsUndoing();
    }
}