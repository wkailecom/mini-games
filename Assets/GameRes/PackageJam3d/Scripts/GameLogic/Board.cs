using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [HideInInspector] public Slot[] slots;

    private readonly List<VirusItem> _waitingPlace = new List<VirusItem>();

    private const int MatchNum = 3;
    private const int MaxReplaceNum = 7;

    private readonly VirusItem[] _replace = new VirusItem[MaxReplaceNum];

    public bool playing;

    public int PurgeCount
    {
        get;
        private set;
    }

    public int HandCount()
    {
        return slots.TakeWhile(t => t.VirusItem != null).Count();
    }

    public int ReplaceCount()
    {
        return _replace.Count(t => t);
    }
    
    public int HandColorCount()
    {
        var count = 0;
        var colorList = new List<int>();
        foreach (var t in slots)
        {
            if(t.VirusItem == null)
                break;
            var color = t.VirusItem.virusColor;
            if (colorList.Contains(color)) continue;
            count++;
            colorList.Add(color);
        }
        return count;
    }

    

    private bool _isUndo = false;
    
    // Start is called before the first frame update
    private void Start()
    {
        slots = GetComponentsInChildren<Slot>();
        playing = true;
    }

    public void Clear()
    {
        PurgeCount = 0;
        playing = true;
        _waitingPlace.Clear();
        Array.Clear(_replace, 0, MaxReplaceNum);

        foreach (var slot in slots)
        {
            slot.VirusItem = null;
        }
    }

    public Slot GetPullPosition(int virusColor)
    {
        var index = ColorExisted(virusColor);
        if (index >= slots.Length)
        {
            Debug.LogWarning("out of slot range");
            return null;
        }

        var slot = slots[index];
        return slot;
    }

    private int ColorExisted(int virusColor)
    {
        var matchIndex = 0;
        var matched = false;
        for (var i = 0; i < slots.Length; i++)
        {
            if (!matched)
                matchIndex = i;
            var virusItem = slots[i].VirusItem;
            if (virusItem is null)
                break;
            if (virusItem.virusColor == virusColor)
            {
                matched = true;
                matchIndex = i + 1;
                continue;
            }

            if (matched)
                break;
        }

        if (!matched) return matchIndex;
        {
            BackMoveSlotItem(matchIndex);
        }
        return matchIndex;
    }

    private void BackMoveSlotItem(int startIndex)
    {
        for (var i = slots.Length - 1; i > startIndex; i--)
        {
            slots[i].VirusItem = slots[i - 1].VirusItem;
        }
    }


    private void FrontMoveSlotItem(int startIndex, int moveLength)
    {
        for (var i = startIndex; i < slots.Length; i++)
        {
            slots[i].VirusItem = i + moveLength >= slots.Length ? null : slots[i + moveLength].VirusItem;
        }
    }

    /// <summary>
    /// true == game end 
    /// </summary>
    /// <returns></returns>
    public bool DetectSlot()
    {
        var matched = Matched();
        if (matched)
        {
            JamManager.GetSingleton().PlaySoundAction("Tile_Brick_eliminate");
			//AudioManager.Instance.PlaySound(AudioName.Tile_Brick_eliminate);
            PurgeCount++;
        }
        return !matched && IsFull();
    }

    private bool Matched()
    {
        var result = false;
        var matchIndex = 0;
        for (; matchIndex < slots.Length; matchIndex++)
        {
            var last = matchIndex + MatchNum - 1;
            if (last >= slots.Length)
                break;

            var slot = slots[matchIndex];
            if (!slot.VirusItem)
                break;

            var referColor = slot.VirusItem.virusColor;
            var isMath = true;
            for (var j = 1; j < MatchNum; j++)
            {
                var nextSlot = slots[matchIndex + j];
                if (!nextSlot.VirusItem)
                {
                    isMath = false;
                    break;
                }

                var color = nextSlot.VirusItem.virusColor;
                if (color == referColor) continue;
                isMath = false;
                break;
            }

            if (!isMath) continue;
            for (var i = matchIndex; i < matchIndex + MatchNum; i++)
            {
                var virusItem = slots[i].VirusItem;
                for (var j = 0; j < _replace.Length; j++)
                {
                    if (_replace[j] != virusItem) continue;
                    _replace[j] = null;
                    break;
                }

                slots[i].RemoveVirusItem();
            }

            FrontMoveSlotItem(matchIndex, MatchNum);
            result = true;
            break;
        }

        return result;
    }

    public bool IsFull()
    {
        var item = slots[slots.Length - 1].VirusItem;
        return item && item.VirusState == VirusState.Place;
    }

    private int AvailableSlotCount()
    {
        return slots[slots.Length - 2].VirusItem ? 1 : 0;
    }

    /// <summary>
    /// add item to waiting queue
    /// </summary>
    /// <param name="virusItem"></param>
    public void PlaceColor(VirusItem virusItem)
    {
        _waitingPlace.Add(virusItem);
    }

    private void Update()
    {
        if (_waitingPlace.Count <= 0 || !playing || _isUndo) return;
        VirusItem virusItem = null;
        if (AvailableSlotCount() == 1)
        {
            virusItem = _waitingPlace[0];
            if (virusItem.VirusState != VirusState.WaitToPlace)
                return;
        }
        else
        {
            if (_waitingPlace.Any(t => t.VirusState == VirusState.Place))
            {
                return;
            }

            foreach (var t in _waitingPlace.Where(t => t.VirusState == VirusState.WaitToPlace))
            {
                virusItem = t;
                break;
            }
        }

        if (!virusItem)
            return;
        for (var i = 0; i < _replace.Length; i++)
        {
            if (_replace[i] != virusItem) continue;
            _replace[i] = null;
            break;
        }

        var doPlace = DoPlace(virusItem);
        if (doPlace)
            _waitingPlace.Remove(virusItem);
    }

    private bool DoPlace(VirusItem virusItem)
    {
        if (IsFull())
        {
            return false;
        }

        var index = ColorExisted(virusItem.virusColor);
        if (index >= slots.Length)
        {
            Debug.LogWarning("out of slot range");
            return false;
        }

        var slot = slots[index];

        slot.VirusItem = virusItem;
        return true;
    }

    public TileItem Undo()
    {
        if (_isUndo)
            return null;
        TileItem item = null;
        var max = -1;
        var index = 0;
        for (var i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];
            if (slot.VirusItem == null) continue;
            var introduction = slot.VirusItem.introduction;
            if(introduction <= max) continue;
            max = introduction;
            item = slot.VirusItem;
            index = i;
        }
        if (item)
        {
            _isUndo = true;
            Invoke(nameof(ResetUndo),0.5f);
            FrontMoveSlotItem(index, 1);
        }
        return item;
    }

    private void ResetUndo()
    {
        _isUndo = false;
    }

    public bool CanReplace()
    {
        return _replace.Any(t => t == null);
    }

    public int GetPullColor()
    {
        var info = new Dictionary<int, int>();
        foreach (var t in slots)
        {
            var item = t.VirusItem;
            if (!item) continue;
            if (!info.ContainsKey(item.virusColor))
            {
                info.Add(item.virusColor, 1);
            }
            else
            {
                info[item.virusColor]++;
            }
        }

        var color = 0;
        var count = 0;
        foreach (var colorInfo in info.Where(colorInfo => colorInfo.Value > count))
        {
            count = colorInfo.Value;
            color = colorInfo.Key;
        }

        return color;
    }

    public bool Replace()
    {
        var lastIndex = 0;
        for (; lastIndex < slots.Length; lastIndex++)
        {
            var slot = slots[lastIndex];
            if (slot.VirusItem == null)
                break;
        }

        if (lastIndex == 0)
            return false;
        var hasFree = false;
        var replaceNum = 0;
        for (var index = 0; index < _replace.Length; index++)
        {
            if (replaceNum >= MatchNum || lastIndex <= 0)
                break;
            if (_replace[index] != null) continue;
            hasFree = true;
            var slot = slots[lastIndex - 1];
            var virusItem = slot.VirusItem;
            _replace[index] = virusItem;
            virusItem.DoReplace(slots[index]);
            slot.VirusItem = null;
            lastIndex--;
            replaceNum++;
        }

        if (!hasFree)
        {
            return false;
        }
        //XLuaKit.CallLua("ReceiveGameBehaviour", (int)BehaviourType.click_props, (int)PropsType.Replace, 0);

        return true;
    }


    public List<int> GetPlaceColorInfo()
    {
        return (from t in slots where t.VirusItem select t.GetColor()).ToList();
    }
}