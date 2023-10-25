using System;
using System.Collections.Generic;

namespace T0R1
{
    public class EventDispatcher
    {
        private static Dictionary<string, Action<object>> _eventDictionary = new Dictionary<string, Action<object>>();
        
        public static void AddEventListener(string eventName, Action<object> listener)
        {
            if (_eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
            {
                thisEvent += listener;
                _eventDictionary[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                _eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void RemoveEventListener(string eventName, Action<object> listener)
        {
            if (_eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
            {
                thisEvent -= listener;
                if (thisEvent == null)
                {
                    _eventDictionary.Remove(eventName);
                }
                else
                {
                    _eventDictionary[eventName] = thisEvent;
                }
            }
        }

        public static void DispatchEvent(string eventName, object payload = null)
        {
            if (_eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
            {
                thisEvent?.Invoke(payload);
            }
        }
    }
}