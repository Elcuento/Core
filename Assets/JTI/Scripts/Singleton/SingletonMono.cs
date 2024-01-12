using UnityEngine;

namespace JTI.Scripts.Managers
{
    public class SingletonMono<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        private static bool _isApplicationQuiting;

        public static string Path
        {
            get { return "Game/Prefabs/Singleton/"; }
        }

        public static T Instance
        {
            get
            {
                if (_isApplicationQuiting)
                    return null;

                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<T>();


                if (_instance == null)
                {

                    var asset = Resources.Load<T>($"{Path}{typeof(T).Name}");

                    if (asset != null)
                        _instance = Instantiate(asset);

                    if (_instance != null)
                        return _instance;

                    _instance = new GameObject(typeof(T).Name)
                        .AddComponent<T>();
                }

                return _instance;
            }
        }

        protected void Start()
        {
            OnStart();
        }

        protected void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                transform.SetParent(null);
                OnAwaken();
            }
            else
            {
                DestroyImmediate(gameObject);
                return;
            }

        }
        public static bool IsExist()
        {
            return _instance != null;
        }

        public void Touch()
        {

        }

        protected virtual void OnStart()
        {

        }
        protected virtual void OnAwaken()
        {
            
        }

        private void OnApplicationQuit()
        {
            _isApplicationQuiting = true;
        }
    }
}