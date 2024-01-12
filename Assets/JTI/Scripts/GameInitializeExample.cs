using System.Collections.Generic;
using JTI.Scripts.GameControllers;
using JTI.Scripts.GameServices;
using JTI.Scripts.Managers;
using UnityEngine;

namespace JTI.Scripts
{
    public class GameInitializeExample : MonoBehaviour
    {
        public List<GameControllerBase> A;
        public void Start()
        {
            GameManager.Instance.Install<GameManager.GameManagerSettings>();
            GameManager.Instance.InstallService<GameServiceFileStorage, GameServiceSettings>();
            GameManager.Instance.InstallController(A[0]);
            GameManager.Instance.InstallController<AudioController<AudioControllerSettings>, AudioControllerSettings>(new AudioControllerSettings());
            GameManager.Instance.Initialize();
        }
    }
}