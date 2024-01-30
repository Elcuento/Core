
using UnityEngine;

namespace JTI.Scripts.Managers
{
    public class SingletonSettingsA :
        SingletonSettings
    {

    }
    public class SingletonSettings
	{
        public virtual bool IsAutoLoaded { get; set; }
		public virtual bool IsAutoCreated { get; set; }
        public virtual bool IsDontDestroyOnLoad { get; set; }

        public virtual string AutoLoadedPath => $"{Application.persistentDataPath}/Game/Prefabs/Singleton/";

        public static T Create<T>() where T : SingletonSettings, new()
        {
            return new T();
        }

        public SingletonSettings()
        {

        }
    }
}