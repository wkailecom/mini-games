using DG.Tweening;
using UnityEngine;

namespace GameLogic
{
    public class Slot :MonoBehaviour
    {
        private VirusItem _virusItem;
        private Tween _moveTween;

        
        public VirusItem VirusItem
        {
            get => _virusItem;
            set
            {
                _virusItem = value;
                if (!_virusItem)
                    return;
                _virusItem.DoPlace(this);
            }
        }

        public void SetItem(VirusItem virusItem)
        {
            _virusItem = virusItem;
        }

        public void ItemDoPlace()
        {
            if(_virusItem != null)
                _virusItem.DoPlace(this);
        }
        
        public void RemoveVirusItem()
        {
            if (!VirusItem) return;
            VirusItem.VirusState = VirusState.Disappear;
            VirusItem = null;
        }

        public int GetColor()
        {
            if (VirusItem == null)
            {
                Debug.LogError("[Game] get slot color error ,virus item is null :" + transform.GetSiblingIndex());
            }
            return VirusItem.virusColor;
        }
    }
}