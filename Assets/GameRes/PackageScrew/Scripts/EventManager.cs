using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrewJam
{
    public class EventManager : Singleton<EventManager>
    {
        public ScrewTriggerEvents triggerEvents;

        public void RegisterTriggerEvents(ScrewTriggerEvents triggerEvents)
        {
            this.triggerEvents = triggerEvents;
        }

        public Action<int> OnHoleSlotRemove;
        public void RemoveSlot(int slotIndex)
        {
            OnHoleSlotRemove?.Invoke(slotIndex);
        }

        public Action<SourceLocation, int, bool> OnBoxAddScrew;
        public void BoxAddScrew(SourceLocation source, int boxIndex, bool isExtra)
        {
            OnBoxAddScrew?.Invoke(source, boxIndex, isExtra);
        }

        public Action<int, int, int> OnHoleSlotAddScrew;
        public void HoleSlotAddScrew(int screwIndex, int slotIndex, int colorIndex)
        {
            OnHoleSlotAddScrew?.Invoke(screwIndex, slotIndex, colorIndex);
        }

        public Action<bool, int> OnBoardJointEnabled;
        public void SetJointState(bool isEnabled, int boardIndex)
        {
            OnBoardJointEnabled?.Invoke(isEnabled, boardIndex);
        }

        public Action<int, int> OnBoxRefresh;
        public void RefreshBox(int curColor, int nextColor)
        {
            OnBoxRefresh?.Invoke(curColor, nextColor);
        }

        #region View
        public Func<int, int, bool> OnEliminateScrew;
        public bool EliminateScrew(int screwIndex, int boardIndex)
        {
            return OnEliminateScrew.Invoke(screwIndex, boardIndex);
        }

        public Func<bool> OnAddHoleSlot;
        public bool AddHoleSlot()
        {
            return OnAddHoleSlot.Invoke();
        }

        public Action OnAddBox;
        public void AddBox()
        {
            OnAddBox?.Invoke();
        }

        public Action OnBoxAnimationDone;
        public void BoxAnimationDone()
        {
            OnBoxAnimationDone?.Invoke();
        }

        public Action<int> OnHoleSlotAnimationDone;
        public void HoleSlotAnimationDone(int slotIndex)
        {
            OnHoleSlotAnimationDone?.Invoke(slotIndex);
        }

        public Action OnBoxEntryAnimationDone;
        public void BoxEntryAnimationDone()
        {
            OnBoxEntryAnimationDone?.Invoke();
        }
        #endregion
        /// <summary>
        /// 点击添加孔道具
        /// </summary>
        public Action OnClickAddHoleSlot;

        /// <summary>
        /// 点击锤子道具
        /// </summary>
        public Action OnClickHammer;

        /// <summary>
        /// 点击添加箱子道具
        /// </summary>
        public Action OnClickToolbox;

        /// <summary>
        /// 可点击状态切换
        /// </summary>
        public Action<bool> OnChangeClickState;

        /// <summary>
        /// 失败续玩
        /// </summary>
        public Action OnTriggerReplay;

        /// <summary>
        /// 添加孔道具是否可用
        /// </summary>
        public Func<bool> CheckCanExtraSlotUse;

        /// <summary>
        /// 获取一组螺丝
        /// </summary>
        public Func<Dictionary<int, Transform>> GetScrews;

        /// <summary>
        /// 通过Index直接消除螺丝
        /// </summary>
        public Action<int> EliminateScrewDirectly;
    }

    public class ScrewTriggerEvents
    {
        public Action OnGameSuccess { get; private set; }
        public Action OnGameFailed { get; private set; }
        public Action OnUseExtraSlot { get; private set; }
        public Action OnClickHammer { get; private set; }
        public Action OnHammerCancel { get; private set; }
        public Action OnUseExtraBox { get; private set; }
        public Action ExtraSlotComplete { get; private set; }
        public Action HammerComplete { get; private set; }
        public Action ExtraBoxComplete { get; private set; }
        public Action ReadyToSuccess { get; private set; }
        public Action<string> PlaySound { get; private set; }

        public ScrewTriggerEvents(Action onGameSuccess, Action onGameFailed, Action onUseExtraSlot, Action onClickHammer, Action onHammerCancel, Action onUseExtraBox, Action extraSlotComplete, Action hammerComplete, Action extraBoxComplete, Action readyToSuccess, Action<string> playSound)
        {
            OnGameSuccess = onGameSuccess;
            OnGameFailed = onGameFailed;
            OnUseExtraSlot = onUseExtraSlot;
            OnClickHammer = onClickHammer;
            OnHammerCancel = onHammerCancel;
            OnUseExtraBox = onUseExtraBox;
            ExtraSlotComplete = extraSlotComplete;
            HammerComplete = hammerComplete;
            ExtraBoxComplete = extraBoxComplete;
            ReadyToSuccess = readyToSuccess;
            PlaySound = playSound;
        }
    }
}

