using UnityEngine;


namespace JTI.Scripts.GameControllers
{


    public abstract class GameController
    {
        public class GameControllerSettings
        {

        }

        public virtual GameControllerWrapper View { get; protected set; }

        protected GameController(GameControllerSettings settings)
        {
           
        }

        public void SetWrapper(GameControllerWrapper a)
        {
            View = a;
        }

        public void Install()
        {
            if (View != null) return;

            CreateWrapper();

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

        private void OnDestroy()
        {
            OnOnDestroy();
        }

        protected virtual void CreateWrapper()
        {
            if (View != null) return;

            View = new GameObject(GetType().Name)
                .AddComponent<GameControllerWrapper>();
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
