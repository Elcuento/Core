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

        public List<GameControllerBase> GameControllers { get; private set; }
        public List<GameServiceBase> GameServices { get; private set; }

        public EventManagerLocal<EventGame> GameEvents { get; private set; }

        public void InstallController<T, TU>() where TU : GameControllerBase.GameControllerSettings where T : GameController<TU>
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

 
        public void InstallController<T, TU>(TU settings) where TU : GameControllerSettings where T : GameController<TU>
        {
            try
            {
                var c = new GameObject(typeof(T).Name).AddComponent<T>();
                c.Install(settings);
                GameControllers.Add(c);
            }
            catch (Exception e)
            {
                Debug.LogError("Error on install controller " + typeof(T).Name + "\n" + e);
            }
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
 
        public TU GetController<TU, TE>() where TE : GameControllerSettings where TU : GameController<TE>
        {
            return (TU)GameControllers.FirstOrDefault(x => x is TU);
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