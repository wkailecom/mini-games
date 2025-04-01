using System;
using System.Collections.Generic;

namespace LLFramework.Event
{
    public class EventManager : BaseManager<EventManager>
    {
        readonly Dictionary<int, List<Action<EventData>>> mEventHandlers = new();
        readonly Dictionary<int, EventData> mEventDataPool = new();

        public void Register(int pEventID, Action<EventData> pEventHandler, bool pPriority = false)
        {
            if (!mEventHandlers.TryGetValue(pEventID, out var tActionList))
            {
                tActionList = new List<Action<EventData>>();
                tActionList.Add(null);
                tActionList.Add(null);
                mEventHandlers.Add(pEventID, tActionList);
            }

            tActionList[pPriority ? 1 : 0] += pEventHandler;
        }

        public void Unregister(int pEventID, Action<EventData> pEventHandler, bool pPriority = false)
        {
            if (!mEventHandlers.TryGetValue(pEventID, out var tActionList))
            {
                LogManager.LogWarning($"EventManager.UnRegister : not registered Type: {pEventID}");
                return;
            }

            tActionList[pPriority ? 1 : 0] -= pEventHandler;
        }

        public void Trigger(int pEventID)
        {
            Trigger(GetEventData(pEventID));
        }

        public void Trigger(EventData pEventData)
        {
            if (pEventData == null)
            {
                LogManager.LogError("EventManager.Trigger : Param is Null!");
                return;
            }

            if (mEventHandlers.TryGetValue(pEventData.EventID, out var tActionList))
            {
                tActionList[1]?.Invoke(pEventData);
                tActionList[0]?.Invoke(pEventData);
            }

            pEventData.SetObjectFree();
        }

        public EventData GetEventData(int pEventID)
        {
            if (mEventDataPool.TryGetValue(pEventID, out EventData tEventData))
            {
                return tEventData;
            }

            LogManager.LogError($"EventManager.GetEventData: EventData {pEventID} is not defined!");
            return null;
        }

        public void AddEventData(int pEventID, EventData pEventData)
        {
            if (!mEventDataPool.ContainsKey(pEventID))
            {
                pEventData.SetObjectFree();
                mEventDataPool.Add(pEventID, pEventData);
            }
        }
    }
}