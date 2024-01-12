using System.Collections.Generic;
using UnityEngine;


namespace JTI.Scripts.GameControllers
{
    public class GameControllerSettings
    {

    }
    public abstract class GameControllerBase : MonoBehaviour
    {
        [SerializeField] private GameControllerSettings _settings;

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
            DontDestroyOnLoad(gameObject);
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
    }


    public abstract class GameController<TA> : GameControllerBase where TA : GameControllerSettings
    {
        public virtual void Install(TA a)
        {

        }

    }
}
