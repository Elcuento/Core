using JTI.Scripts.GameServices;
using JTI.Scripts.Managers;
using UnityEngine;

namespace JTI.Scripts
{
    public class GameInitializeExample : MonoBehaviour
    {
        public void Start()
        {
            GameManager.Instance.Install<GameManager.GameManagerSettings>();
            GameManager.Instance.InstallService<GameServiceFileStorage, GameService.GameServiceSettings>();
            GameManager.Instance.Initialize();
        }
    }
}