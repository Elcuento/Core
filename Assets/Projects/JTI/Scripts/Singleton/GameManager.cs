using System;
using JTI.Scripts.GameControllers;
using System.Collections.Generic;
using System.Linq;
using Assets.JTI.Scripts.Events.Game;
using JTI.Scripts.Events;
using JTI.Scripts.GameServices;
using UnityEngine;

namespace JTI.Scripts.Managers
{
    public class SingletonSettingsGameManager : SingletonSettings
    {
        public override bool IsAutoLoaded { get; set; } = true;
        public override bool IsAutoCreated { get; set; } = true;
        public override bool IsDontDestroyOnLoad { get; set; } = true;

        public override string AutoLoadedPath => $"{Application.persistentDataPath}/Game/Prefabs/Singleton/";
    }

    public class GameManager : Singleton<GameManager, SingletonSettingsGameManager>
    {
        public class GameManagerSettings
        {

        }

        public List<GameControllerWrapper> GameControllers { get; private set; }
        public List<GameServiceBase> GameServices { get; private set; }

        public EventManagerLocal<GameEvent> GameEvents { get; private set; }

        /*   public void InstallController(GameControllerWrapper a)
           {
               try
               {
                   a.Initialize<>();
                   GameControllers.Add(a);
               }
               catch (Exception e)
               {
                   Debug.LogError("Error on install controller " + a.GetType().Name + "\n" + e);
               }
           }*/
        public T InstallController<T>(GameControllerWrapper view, T controller) where T : GameController
        {
            try
            {
                controller.SetWrapper(view);
                var wrapper = controller.View;
                controller.Install();
                GameControllers.Add(wrapper);
            }
            catch (Exception e)
            {
                Debug.LogError("Error on install controller " + typeof(T).Name + "\n" + e);
            }

            return controller;
        }
        public T InstallController<T>(T controller) where T : GameController
        {
            try
            {
                var wrapper = controller.View;
                controller.Install();
                GameControllers.Add(wrapper);
            }
            catch (Exception e)
            {
                Debug.LogError("Error on install controller " + typeof(T).Name + "\n" + e);
            }

            return controller;
        }

        public void InstallService<T, TU>(TU settings) where TU : GameServiceSettings where T : GameService<TU>
        {
            try
            {
                var c = new GameObject(typeof(T).Name).AddComponent<T>();
                c.Install(settings);
                GameServices.Add(c);
            }
            catch (Exception e)
            {
                Debug.LogError("Error on install service " + typeof(T).Name + "\n" + e);
            }
        }

        public void InstallService<T, TU>() where TU : GameServiceSettings where T : GameService<TU>
        {
            try
            {
                var c = new GameObject(typeof(T).Name).AddComponent<T>();
                c.Install(default(TU));
                GameServices.Add(c);
            }
            catch (Exception e)
            {
                Debug.LogError("Error on install service " + typeof(T).Name + "\n" + e);
            }
        }

        protected override void OnAwaken()
        {
            base.OnAwaken();

            GameControllers = new List<GameControllerWrapper>();
            GameEvents = new EventManagerLocal<GameEvent>();
            GameServices = new List<GameServiceBase>();
        }

        public virtual void Install<T>() where T : GameManagerSettings
        {

        }

        public void Initialize()
        {
            InitializeServices();
            InitializeControllers();

            LateInitialize();
        }

        private void LateInitialize()
        {
            LateInitializeServices();
            LateInitializeControllers();

            InitializeFinish();
        }

        private void InitializeFinish()
        {

        }

        private void InitializeServices()
        {
            foreach (var service in GameServices)
            {
                service.Initialize();
            }
        }
 
        public TU GetController<TU>() where TU : GameController
        {
            return null;// (TU)GameControllers.FirstOrDefault(x => x is TU);
        }

        public TU GetService<TU,TE>() where TE : GameServiceSettings where TU : GameService<TE>
        {
            return (TU)GameServices.FirstOrDefault(x => x is TU);
        }

        private void LateInitializeServices()
        {
            foreach (var service in GameServices)
            {
                service.LateInitialize();
            }
        }

        private void InitializeControllers()
        {
            foreach (var gameController in GameControllers)
            {
               // gameController.Initialize();
            }

        }
        private void LateInitializeControllers()
        {
            foreach (var gameController in GameControllers)
            {
               // gameController.LateInitialize();
            }
        }
    }
}