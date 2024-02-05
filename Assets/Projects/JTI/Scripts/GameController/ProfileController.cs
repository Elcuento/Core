using System;
using Assets.JTI.Scripts.Events.Game;
using JTI.Scripts.Events;
using JTI.Scripts.GameControllers;
using JTI.Scripts.Storage;

namespace JTI.Scripts.Managers
{

    public class ProfileController : GameController
    {
        public ProfileController() { }

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

        protected override void OnInstall()
        {
            GameEvents = new EventManagerLocal<ProfileEvent>();

            _eventSubscriberLocal = new EventSubscriberLocal<GameEvent>(Manager.GameEvents);

            _storage = new FileStorageString();
            _storage.SetAutosave(_settings.Autosave);
        }

        public ProfileController SetSettings(ProfileControllerSettings a)
        {
            _settings = a;

            return this;
        }

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

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Load();

        } 
        public void Save()
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
