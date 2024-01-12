using UnityEngine;

namespace JTI.Scripts.GameServices
{
    public class GameService : MonoBehaviour
    {
        public class GameServiceSettings
        {

        }
        public void Install<T>(T a) where T : GameServiceSettings
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