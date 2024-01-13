using System.Collections.Generic;
using UnityEngine;


namespace JTI.Scripts.GameControllers
{
    [System.Serializable]
    public class GameControllerSettings
    {

    }
    
    public abstract class GameController<TA> where TA : GameControllerSettings
    {
        public GameControllerWrapper Wrapper;

        [SerializeField] private GameControllerSettings _settings;

        protected GameController()
        {
            CreateWrapper();
        }

        protected virtual void CreateWrapper()
        {
            Wrapper = new GameObject(typeof(TA).Name)
                .AddComponent<GameControllerWrapper>();
        }
        public virtual void Install()
        {

        }

        public void Initialize()
        {
            OnInitialize();
        }

        public void LateInitialize()
        {
            OnLateInitialize();
        }

        private void Awake()
        {
            //DontDestroyOnLoad(gameObject);
            OnAwaken();
        }

        private void OnDestroy()
        {
            OnOnDestroy();
        }

        protected virtual void OnAwaken()
        {

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
        public virtual void Install(TA a)
        {

        }

    }
}
