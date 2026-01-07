using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core.Scripts.Utils.DesignPattern.Observer
{
    public static class ObserverManager<T> where T : Enum
    {
        private static readonly Dictionary<T, Action<object>> _eventDictionary = new();

        public static void AddListener(T eventID, Action<object> callback)
        {
            if(callback == null)
            {
                Debug.LogWarning("Callback cannot be null.");
                return;
            }

            if (!_eventDictionary.ContainsKey(eventID))
            {
                _eventDictionary[eventID] = delegate { };
            }
            
            _eventDictionary[eventID] += callback;
        }

        public static void RemoveListener(T eventID, Action<object> callback)
        {
            if(callback == null) return;

            if (_eventDictionary.ContainsKey(eventID))
            {
                _eventDictionary[eventID] -= callback;
                
                if(_eventDictionary[eventID] == null || _eventDictionary[eventID].GetInvocationList().Length == 0)
                {
                    _eventDictionary.Remove(eventID);
                }
            }
        }
        
        public static void PostEvent(T eventID, object parameter = null)
        {
            if (_eventDictionary.TryGetValue(eventID, out var callback))
            {
                callback?.Invoke(parameter);
            }
            else
            {
                Debug.LogWarning($"No listeners for event: {eventID}");
            }
        }
        
        public static void ClearAllListeners()
        {
            _eventDictionary.Clear();
            Debug.Log("All observers cleared.");
        }
    }
}
