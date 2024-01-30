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


    public class ProfileController : GameController
    {
        [System.Serializable]
        public class ProfileControllerSettings : GameControllerSettings
        {
            public bool Autosave;
            public ProfileControllerSettings()
            {
                Autosave = true;
            }
        }
        public EventManagerLocal<ProfileEvent> GameEvents { get; private set; }

        private EventSubscriberLocal<GameEvent> _eventSubscriberLocal;

        private ProfileControllerSettings _settings;

        private FileStorageString _storage;

 
        public void Get(Enum a, object def)
        {
            _storage.LoadDefault(a.ToString(), def);
        }

        public void Set(Enum a, object val)
        {
            _storage.Set(a.ToString(), val);
        }

        public void Remove(Enum a)
        {
            _storage.Destroy(a.ToString());
        }

        public bool IsExist(Enum a)
        {
           return _storage.IsExist(a.ToString());
        }

        public ProfileController(ProfileControllerSettings s) : base(s)
        {
            _settings = s;

            GameEvents = new EventManagerLocal<ProfileEvent>();

            _eventSubscriberLocal = new EventSubscriberLocal<GameEvent>(GameManager.Instance.GameEvents);

            _storage = new FileStorageString();
            _storage.SetAutosave(_settings.Autosave);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Load();

        } 
        void Save()
        {
            _storage.Save();
        }

        public void Load()
        {
     /*       var timers = GameManager.Instance.GetController<FileStorage<>>()
            if (timers != null)
            {
                foreach (var timer in timers)
                {
                    Timers.Add(new Timer(timer));
                }
            }*/
        }
    }
}
