using System.Collections;
using System.Collections.Generic;
using JTI.Scripts.Storage;
using UnityEngine;

namespace JTI.Scripts.GameServices
{
    public class GameServiceFileStorage : GameService
    {
        public enum ExampleType
        {
            One,Two, Three
        }

        public FileStorage<ExampleType> Storage { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Storage = new FileStorage<ExampleType>();
        }
    }
}