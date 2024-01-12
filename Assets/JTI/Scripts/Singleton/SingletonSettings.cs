
using UnityEngine;

namespace JTI.Scripts.Managers
{
    public class SingletonSettings
	{
        public virtual bool IsAutoLoaded { get; set; }
		public virtual bool IsAutoCreated { get; set; }
        public virtual bool IsDontDestroyOnLoad { get; set; }

        public virtual string AutoLoadedPath => $"{Application.persistentDataPath}/Game/Prefabs/Singleton/";
    }
}