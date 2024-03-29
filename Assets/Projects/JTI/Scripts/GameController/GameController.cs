﻿using JTI.Scripts.Managers;
using UnityEngine;


namespace JTI.Scripts.GameControllers
{


    public abstract class GameController : GameManager.IController
    {
        protected GameController() { }
        public GameManager Manager { get; private set; }

        protected bool _installed;

        public class GameControllerSettings
        {

        }


        public void SetManager(GameManager manager)
        {
            Manager = manager;
        }

        public GameController SetSettings(GameControllerSettings a)
        {
            return this;
        }

        public GameController SetSettings()
        {
            return this;
        }

        public void Install()
        {
            _installed = true;

            OnInstall();
        }

        public void Initialize()
        {
            OnInitialize();
        }

        public void LateInitialize()
        {
            OnLateInitialize();
        }

        public void Destroy()
        {
        
        }

        public void OnDestroy()
        {
            OnOnDestroy();
        }
        protected virtual void OnInitialize()
        {

        }

        protected virtual void OnLateInitialize()
        {

        }

        protected virtual void OnOnDestroy()
        {

        }
        protected virtual void OnInstall()
        {

        }

    }
}
