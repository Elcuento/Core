using UnityEngine;

namespace JTI.Scripts.Managers
{

    public class Singleton<T,TS> : MonoBehaviour where T : Component  where TS : SingletonSettings, new()
    {
        private static T _instance;
        private static bool _isApplicationQuiting;
        private static bool _isDestroyed;
        private static SingletonSettings _settings;

        public static T Instance
        {
            get
            {

                if (_isApplicationQuiting || _isDestroyed) return null;

                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<T>();

                if (_instance != null)
                    return _instance;

                if (_settings.IsAutoLoaded)
                {
                    var asset = Resources.Load<T>(_settings.AutoLoadedPath + typeof(T).Name);

                    if (asset != null)
                        _instance = Instantiate(asset);
                }

                if (_instance != null)
                    return _instance;

                if (_settings.IsAutoCreated)
                {
                    _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }

                return _instance;
            }
            private set => Constructor();
        }

        private static void Constructor()
        {
            if (_settings == null)
            {
                _settings = new TS();
            }
        }

        protected void Awake()
        {
            if (_instance == null || _instance == this)
            {
                _instance = this as T;

                Constructor();

                if (_settings.IsDontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                    transform.SetParent(null);
                }

                OnAwaken();
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
        protected void OnDestroy()
        {
            _isDestroyed = true;
            OnOnDestroyed();
        }
        protected void OnApplicationQuit()
        {
            _isApplicationQuiting = true;
            OnOnApplicationQuit();
        }
        protected void Start()
        {
            OnStart();
        }

        public static bool IsExist()
        {
            return _instance != null;
        }

        public void Touch()
        {

        }

        protected virtual void OnAwaken()
        {
            transform.SetParent(null);
        }

        protected virtual void OnStart()
        {
           
        }

        protected virtual void OnOnApplicationQuit()
        {

        }
        protected virtual void OnOnDestroyed()
        {

        }
        


    }
}