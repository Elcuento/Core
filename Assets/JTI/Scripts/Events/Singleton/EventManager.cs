using System;
using System.Collections.Generic;
using JTI.Scripts.Managers;

namespace Assets.JTI.Scripts.Events.Singleton
{
    public class EventManager : SingletonMono<EventManager>
    {
        readonly Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();

        public int EventCounts => _events.Count;

        public void Subscribe<T>(global::JTI.Scripts.Events.EventHandler<T> eventAction)
        {
     
            var t = typeof(T);

            _events.TryGetValue(t, out var rawList);
            _events[t] = (rawList as global::JTI.Scripts.Events.EventHandler<T>) + eventAction;

        }

        public void Unsubscribe<T>(global::JTI.Scripts.Events.EventHandler<T> eventAction)
        {

            var t = typeof(T);

            if (_events.TryGetValue(t, out var thisEvent))
            {
                _events[t] = (thisEvent as global::JTI.Scripts.Events.EventHandler<T>) - eventAction;
            }
        }

        public void UnsubscribeAndClearAllEvents()
        {
            _events.Clear();
        }
        public void Publish<T>(T eventMessage)
        {

            var t = typeof(T);

            if (_events.TryGetValue(t, out var thisEvent))
            {
                if (thisEvent is global::JTI.Scripts.Events.EventHandler<T> ev)
                    ev(eventMessage);
            }
        }
 
    }
}