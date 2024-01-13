using UnityEngine;


namespace JTI.Scripts.GameControllers
{
    [System.Serializable]
    public class GameControllerSettings
    {

    }
    
    public abstract class GameController<TA> where TA : GameControllerSettings
    {
        public GameControllerView View { get; protected set; }

        [SerializeField] private GameControllerSettings _settings;

        protected GameController()
        {
            CreateView();
        }

        protected virtual void CreateView()
        {
            View = new GameObject(typeof(TA).Name)
                .AddComponent<GameControllerView>();
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

        private void OnDestroy()
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
        public virtual void Install(TA a)
        {

        }

    }
}
