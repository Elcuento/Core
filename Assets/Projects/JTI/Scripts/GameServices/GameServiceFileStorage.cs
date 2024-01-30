using System;
using System.Collections;
using System.Collections.Generic;
using JTI.Scripts.Storage;
using UnityEngine;

namespace JTI.Scripts.GameServices
{
    public class GameServiceFileStorageGameServiceSettings
    {
        public bool AutoSave;
    }
    public class GameServiceFileStorage<T> : GameService<GameServiceSettings> where T : Enum
    {
        public FileStorageEnum<T> Storage { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Storage = new FileStorageEnum<T>();
        }

        public void Set(T a, object o) {
            Storage.Set(a, o);
        }

        public void Destroy(T a, object o)
        {
            Storage.Destroy(a);
        }
    }
}