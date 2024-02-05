using JTI.Scripts.Managers;
using UnityEngine;


namespace JTI.Scripts.GameControllers
{


    public abstract class GameControllerMono : MonoBehaviour, GameManager.IController
    {
        public GameManager Manager { get; private set; }

        protected GameControllerSettings Settings;

        [System.Serializable]
        public class GameControllerSettings
        {

        }

        protected void Awake()
        {
         
        }

        public void Install()
        {
            OnInstall();
        }

        public void Install(GameController.GameControllerSettings a)
        {
            
        }

        public void Initialize()
        {
            OnInitialize();
        }

        public void SetManager(GameManager manager)
        {
            Manager = manager;
        }

        public GameControllerMono SetSettings()
        {
            return this;
        }

        public void LateInitialize()
        {
            OnLateInitialize();
        }

        public void Destroy()
        {
            Destroy(gameObject);
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
