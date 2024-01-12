using System.Collections;
using System.Collections.Generic;
using JTI.Scripts.Storage;
using UnityEngine;

namespace JTI.Scripts.GameServices
{
    public class GameServiceTempStorage : GameService
    {
        public TempStorage TempStorage { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            TempStorage = new TempStorage();
        }
    }
}