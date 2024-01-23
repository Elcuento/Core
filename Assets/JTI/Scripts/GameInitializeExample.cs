using System;
using System.Collections.Generic;
using Assets.JTI.Scripts.Database;
using JTI.Scripts.GameControllers;
using JTI.Scripts.GameServices;
using JTI.Scripts.Localization.Data;
using JTI.Scripts.Managers;
using UnityEngine;

namespace JTI.Scripts
{
    public class GameInitializeExample : MonoBehaviour
    {
        public void Start()
        {
            GameManager.Instance.Install<GameManager.GameManagerSettings>();

            var dataBase = GameManager.Instance.InstallController(new DataBaseController(new DataBaseController.DataBaseControllerSettings("Data/")));
            GameManager.Instance.InstallController(new AudioController(new AudioController.AudioControllerSettings(dataBase.Get<AudioData>())));

        /*    GameManager.Instance.InstallController<TimeController<TimeControllerSettings>,
                TimeControllerSettings>(new TimeControllerSettings());
            GameManager.Instance.InstallController<CounterController<CounterControllerSettings>,
                CounterControllerSettings>(new CounterControllerSettings());*/
            GameManager.Instance.Initialize();
        }
    }
}