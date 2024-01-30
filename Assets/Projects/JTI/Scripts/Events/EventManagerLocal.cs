

using System;
using System.Collections.Generic;

namespace JTI.Scripts.Events
{
    public class EventManagerLocal<TO>
    {
        readonly Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();

        public int EventCounts => _events.Count;

        public void Subscribe<T>(JTI.Scripts.Events.EventHandler<T> eventAction) where T : TO
        {
       
            var t = typeof(T);

            _events.TryGetValue(t, out var rawList);
            _events[t] = (rawList as JTI.Scripts.Events.EventHandler<T>) + eventAction;

        }

        public void Unsubscribe<T>(JTI.Scripts.Events.EventHandler<T> eventAction) where T : TO
        {
            
            var t = typeof(T);

            if (_events.TryGetValue(t, out var thisEvent))
            {
                _events[t] = (thisEvent as JTI.Scripts.Events.EventHandler<T>) - eventAction;
            } 
        }

        public void UnsubscribeAndClearAllEvents()
        {
            _events.Clear();
        }

        public void Publish<T>(T eventMessage) where T : TO
        {
            var eventType = eventMessage.GetType();
            _events.TryGetValue(eventType, out var rawList);

            rawList?.DynamicInvoke(eventMessage);
        }
    }
}