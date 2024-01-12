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
    public class GameManager : Singleton<GameManager, SingletonSettings>
    {
        public class GameManagerSettings
        {

        }

        public List<GameController> GameControllers { get; private set; }
        public List<GameService> GameServices { get; private set; }

        public EventManagerLocal<EventGame> GameEvents { get; private set; }

        public void InstallController<T, TU>() where T : GameController where TU : GameController.GameControllerSettings
        {
            try
            {
                var c = new GameObject(typeof(T).Name).AddComponent<T>();
                c.Install(default(TU));
                GameControllers.Add(c);
            }
            catch (Exception e)
            {
                Debug.LogError("Error on install controller " + typeof(T).Name + "\n" + e);
            }
        }
        public void InstallController<T>() where T : GameController
        {
            try
            {
                var c = new GameObject(typeof(T).Name).AddComponent<T>();

                GameControllers.Add(c);
            }
            catch (Exception e)
            {
                Debug.LogError("Error on install controller " + typeof(T).Name + "\n" + e);
            }
        }
        public void InstallService<T>() where T : GameService
        {
            try
            {
                var c = new GameObject(typeof(T).Name).AddComponent<T>();
                GameServices.Add(c);
            }
            catch (Exception e)
            {
                Debug.LogError("Error on install service " + typeof(T).Name + "\n" + e);
            }
        }

        public void InstallService<T, TU>() where T : GameService where TU : GameService.GameServiceSettings
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
            return (TU)GameControllers.FirstOrDefault(x => x is TU);
        }
        public TU GetService<TU>() where TU : GameService
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
    }
}