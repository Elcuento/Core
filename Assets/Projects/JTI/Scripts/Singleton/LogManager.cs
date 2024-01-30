
using UnityEngine;

namespace JTI.Scripts.Managers
{
    public class LogManager : SingletonMono<LogManager>
    {
        public bool IsLogEnable;

        public static void EnableLog(bool a)
        {
            Instance.IsLogEnable = a;
        }

        public static void Log(string mes, bool force = false)
        {
            if (!Instance.IsLogEnable && !force) return;

            Debug.Log(mes);
        }

        public static void LogError(string mes)
        {
            if (!Instance.IsLogEnable) return;

            Debug.LogError(mes);
        }
    }
}
