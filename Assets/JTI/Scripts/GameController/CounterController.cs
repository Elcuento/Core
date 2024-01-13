using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Assets.JTI.Scripts.Events.Game;
using DG.Tweening;
using JTI.Scripts.Events;
using JTI.Scripts.GameControllers;
using JTI.Scripts.Localization.Data;
using JTI.Scripts.Storage;
using JTI.Scripts.Timers.Core;
using UnityEngine;

namespace JTI.Scripts.Managers
{
    [System.Serializable]
    public class CounterControllerSettings : GameControllerSettings
    {
        public CounterControllerSettings()
        {
           
        }
    }
    public class CounterController<T> : GameController<T> where T : CounterControllerSettings
    {
        public List<Timer> Timers { get; set; }

        private EventSubscriberLocal<GameEvent> _eventSubscriberLocal;

        private T _settings;

        public override void Install(T a)
        {
            base.Install(a);

            _settings = a as T;

            _eventSubscriberLocal = new EventSubscriberLocal<GameEvent>(GameManager.Instance.GameEvents); 
            
            if (_settings == null)
            {
                Debug.LogError("No data was set !");
                return;
            }
        }

        public CounterController()
        {
            Timers = new List<Timer>();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Load();

        }

        public Timer SetTimer(string t, int period, int capacity)
        {
            var timer = Timers.Find(x => x.Type == t);

            if (timer != null)
            {
                timer.SetPeriod(period);
                timer.SetMaxCapacity(capacity);
                return timer;
            }

            timer = new Timer(t, period, capacity);
            Timers.Add(timer);

            Save();

            return timer;
        }

        public Timer AddGetTimer(string t, int period, int capacity)
        {

            var timer = Timers.Find(x => x.Type == t);

            if (timer != null)
            {
                timer.SetPeriod(period);
                timer.SetMaxCapacity(capacity);
                return timer;
            }

            timer = new Timer(t, period, capacity);
            Timers.Add(timer);

            Save();

            return timer;
        }

        public Timer GetTimer(string t)
        {
            return Timers.Find(x => x.Type == t);
        }

        public void Save()
        {

        }

        public void Load()
        {
            var timers = GameManager.Instance.GetController<FileStorage<>>()
            if (timers != null)
            {
                foreach (var timer in timers)
                {
                    Timers.Add(new Timer(timer));
                }
            }
        }
    }
}
