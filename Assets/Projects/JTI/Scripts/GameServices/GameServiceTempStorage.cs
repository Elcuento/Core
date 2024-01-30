using System.Collections;
using System.Collections.Generic;
using JTI.Scripts.Storage;
using UnityEngine;

namespace JTI.Scripts.GameServices
{
    public class GameServiceTempStorage : GameService<GameServiceSettings>
    {
        public TempStorage TempStorage { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            TempStorage = new TempStorage();
        }
    }
}