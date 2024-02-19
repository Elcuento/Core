using System;
using System.Collections.Generic;
using JTI.Scripts.Events.Level;
using UnityEngine;

namespace JTI.Scripts.Events
{
    public class EventSubscriberMonoLocal<TO>
    {
        public class EventSubscribe
        {
            public Delegate Source;
            public Delegate Rise;
        }

        readonly Dictionary<Type, EventSubscribe> _events = new Dictionary<Type, EventSubscribe>();

        protected readonly Component Subscriber;
        protected readonly EventManagerLocal<TO> _eventManager;

        private bool _isDestroyed;

        public EventSubscriberMonoLocal(Component s, EventManagerLocal<TO> eventManager)
        {
            Subscriber = s;
            _eventManager = eventManager;

        }
        public void Subscribe<TB>(JTI.Scripts.Events.EventHandler<TB> eventAction) where TB : TO
        {
            var t = typeof(TB);

            if (!_events.TryGetValue(t, out var rawList))
                _events.Add(t, new EventSubscribe());

            if ((_events[t].Source as JTI.Scripts.Events.EventHandler<TB>) == eventAction)
            {
                return;
            }

            void Ev(TB a)
            {
                if (Check()) eventAction?.Invoke(a);
            }

            _events[t].Rise = (JTI.Scripts.Events.EventHandler<TB>)Ev;
            _events[t].Source = eventAction;

            _eventManager.Subscribe<TB>(Ev);
        }

        public void Unsubscribe<T>(JTI.Scripts.Events.EventHandler<T> eventAction) where T : TO
        {
            _eventManager.Unsubscribe(eventAction);
        }

        public void Unsubscribe<TB>() where TB : TO
        {
            var t = typeof(TB);

            if (_events.TryGetValue(t, out var thisEvent))
            {
                _events[t].Rise = null;
                _events[t].Source = null;
                _eventManager.Unsubscribe((thisEvent.Rise as JTI.Scripts.Events.EventHandler<TB>));
            }
        }

        public void Publish<T>(T eventMessage) where T : TO
        {
            if (!Check())
                return;

            _eventManager.Publish(eventMessage);
        }

        public bool Check()
        {
            if (_eventManager == null)
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
