using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JTI.Scripts.Storage
{
    public class TempStorage
    {
        private Dictionary<object, object> _storage;

        public TempStorage()
        {
            _storage = new Dictionary<object, object>();
        }

        public enum TempStorageVariableType
        {
            None,
            InterstitialShowed
        }

        public void Set(Enum type, object val)
        {
            if (_storage.ContainsKey(type))
            {
                _storage[type] = val;
            }
            else
            {
                _storage.Add(type, val);
            }
        }

        public void Destroy(Enum type)
        {
            if (_storage.ContainsKey(type))
                _storage.Remove(type);
        }

        public TU Load<TU>(Enum type)
        {
            if (_storage.ContainsKey(type))
                return (TU)_storage[type];

            return default(TU);
        }

        public TU LoadDefault<TU>(Enum type, TU def)
        {
            if (_storage.ContainsKey(type))
                return (TU)_storage[type];

            _storage.Add(type, def);

            return (TU)_storage[type];
        }

        public bool IsExist(Enum type)
        {
            return _storage.ContainsKey(type);
        }

        public void Clear()
        {
            _storage.Clear();
        }
    }
}