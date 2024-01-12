using UnityEngine;

namespace JTI.Scripts.Managers
{

    public class SingletonMonoSimple<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<T>();

                return _instance;
            }
        }

        protected void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                OnAwaken();
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        protected void Start()
        {
            OnStart();
        }

        public static bool IsExist()
        {
            return _instance != null;
        }

        protected virtual void OnAwaken()
        {
            transform.SetParent(null);
        }

        protected virtual void OnStart()
        {
           
        }



    }
}