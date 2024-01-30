using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.JTI.Scripts.Events.Singleton
{
    public class EventSubscriber
    {
        public class EventSubscribe
        {
            public Delegate Source;
            public Delegate Rise;
        }
        readonly Dictionary<Type, EventSubscribe> _events = new Dictionary<Type, EventSubscribe>();

        protected readonly Component Subscriber;
        private bool _isDestroyed;

        public EventSubscriber(Component s)
        {
            Subscriber = s;
        }
        public void Subscribe<TB>(global::JTI.Scripts.Events.EventHandler<TB> eventAction)
        {
            var t = typeof(TB);

            if (!_events.TryGetValue(t, out var rawList))
                _events.Add(t, new EventSubscribe());

            if ((_events[t].Source as global::JTI.Scripts.Events.EventHandler<TB>) == eventAction)
            {
                Debug.Log("Already unsubscribed");
                return;
            }

            void Ev(TB a)
            {
                if (Check()) eventAction?.Invoke(a);
            }

            _events[t].Rise = (global::JTI.Scripts.Events.EventHandler<TB>) Ev;
            _events[t].Source = eventAction;

            EventManager.Instance.Subscribe<TB>(Ev);

        }

        public void Unsubscribe<TB>()
        {
            var t = typeof(TB);

            if (_events.TryGetValue(t, out var thisEvent))
            {
                _events[t].Rise = null;
                _events[t].Source = null;
                EventManager.Instance.Unsubscribe((thisEvent.Rise as global::JTI.Scripts.Events.EventHandler<TB>));
            }
        }

        public bool Check()
        {
            if (EventManager.Instance == null)
                return false;

            if (_isDestroyed)
                return false;

            if (Subscriber == null || Subscriber.gameObject == null)
            {
                Debug.Log("Already destroyed object, clear events");
                Destroy();
                return false;
            }

            return true;
        }


        public void Destroy()
        {
            if (_isDestroyed) return;

            _isDestroyed = true;
            UnsubscribeAndClearAllEvents();
        }

        public void UnsubscribeAndClearAllEvents()
        {
            _events.Clear();
        }
    }
}
