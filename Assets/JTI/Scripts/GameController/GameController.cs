using UnityEngine;

namespace JTI.Scripts.GameControllers
{
    public class GameController: MonoBehaviour
    {
        public class GameControllerSettings
        {

        }

        public virtual void Install<T>(T a) where T : GameControllerSettings
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
            DontDestroyOnLoad(gameObject);
            OnAwaken();
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
    }
}
