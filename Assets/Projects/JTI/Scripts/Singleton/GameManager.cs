using System;
using System.Collections.Generic;
using System.Linq;
using JTI.Scripts.Events;
using JTI.Scripts.Events.Game;
using JTI.Scripts.GameControllers;
using JTI.Scripts.GameServices;
using JTI.Scripts.Level;
using UnityEngine;

namespace JTI.Scripts.Managers
{
    public class GameManager : SingletonMonoSimple<GameManager>
    {
        [SerializeField] private string _resourcesPath = "Game/";

        public string ResourcesPath => _resourcesPath;

        public interface IController
        {
            public void Install();
            public void SetManager(GameManager manager);
            public void Initialize();
            public void LateInitialize();
            public void Destroy();
        }

        [SerializeField] private GameControllerMono[] _controllersToInstall;

        public List<LevelMaster> LevelMasters { get; private set; } = new List<LevelMaster>();
        public List<IController> GameControllers { get; private set; }
        public List<GameServiceBase> GameServices { get; private set; }
        public EventManagerLocal<GameEvent> GameEvents { get; private set; }

        public T GetFirstLevelMasterAs<T>() where T : LevelMaster
        {
            return LevelMasters.FirstOrDefault(x => x is T) as T;
        }

        public void AddLevelMaster(LevelMaster levelMaster)
        {
            if (!LevelMasters.Contains(levelMaster))
            {
                LevelMasters.Add(levelMaster);
            }
        }
        public void RemoveLevelMaster(LevelMaster levelMaster)
        {
            LevelMasters.Remove(levelMaster);
        }

        public T GetInstance<T>() where T : GameManager
        {
            return this as T;
        }
        private bool IsMonoBehavior<T>()
        {
            var type = typeof(T);
            while (type != null )
            {
                if(type == typeof(MonoBehaviour)) return true;

                if (type.BaseType == null ) return false;

                type = type.BaseType;
            }
            return false;
        }
        public T GetController<T>()
        {
            return (T)GameControllers.FirstOrDefault(x => x is T);
        }
        public T AddController<T>() where T : IController, new()
        {
            if (IsMonoBehavior<T>())
            {
                var a = new GameObject(typeof(T).Name);
                a.transform.SetParent(transform);
                var s = (a.AddComponent(typeof(T))) as IController;

                s.SetManager(this);
                GameControllers.Add(s);
                return (T)s;
            }

            var b = new T();
            b.SetManager(this);
            GameControllers.Add(b);
            return b;
        }

        public T AddController<T>(T c) where T : MonoBehaviour, IController
        {
            c.SetManager(this);
            GameControllers.Add(c);
            return c;
        }

        protected sealed override void OnAwaken()
        {
            DontDestroyOnLoad(gameObject);

            GameControllers = new List<IController>();
            GameEvents = new EventManagerLocal<GameEvent>();
            GameServices = new List<GameServiceBase>();

            Initialize();
        }

        protected virtual void InstallControllers()
        {
           
        }

        private void Initialize()
        {
            InstallSettingsControllers();

            InstallControllers();

            InitializeControllers();

            OnInitialize();

            LateInitialize();
        }

        private void LateInitialize()
        {
            LateInitializeControllers();

            OnLateInitialize();

            InitializeFinish();
        }

        private void InitializeFinish()
        {
            OnInitializeFinish();
        }
        private void InstallSettingsControllers()
        {
            foreach (var controller in _controllersToInstall)
            {
                if (controller == null) continue;

                AddController(controller)
                    .SetSettings()
                    .Install();
            }

            _controllersToInstall = null;
        }

        private void InitializeControllers()
        {
            foreach (var gameController in GameControllers)
            {
                gameController.Initialize();
            }

        }
        private void LateInitializeControllers()
        {
            foreach (var gameController in GameControllers)
            {
                gameController.LateInitialize();
            }
        }

        public virtual void OnInitialize()
        {

        }

        public virtual void OnLateInitialize()
        {

        }

        public virtual void OnInitializeFinish()
        {

        }


    }
}