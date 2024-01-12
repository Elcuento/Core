using UnityEngine;
using static JTI.Scripts.GameServices.GameServiceBase;

namespace JTI.Scripts.GameServices
{
    public class GameServiceSettings
    {

    }

    public abstract class GameServiceBase : MonoBehaviour
    {
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

    public abstract class GameService<T> : GameServiceBase where T : GameServiceSettings
    {
        public void Install(T a)
        {

        }
    }
}