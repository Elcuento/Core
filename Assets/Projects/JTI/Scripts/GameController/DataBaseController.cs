using System;
using System.Collections.Generic;
using System.Linq;
using Assets.JTI.Scripts.Database;
using JTI.Scripts.GameControllers;
using UnityEngine;

namespace JTI.Scripts.Managers
{

    public class DataBaseController : GameController
    {
        public DataBaseController()
        {

        }

        [System.Serializable]
        public class DataBaseControllerSettings : GameControllerSettings
        {
            public List<DataBase> DataBases = new List<DataBase>();
            public string Path;

            public DataBaseControllerSettings(){}
            public DataBaseControllerSettings(string path, List<DataBase> data)
            {
                Path = path;
                DataBases = data;
            }
            public DataBaseControllerSettings(string path)
            {
                Path = path;
            }
        }

        private DataBaseControllerSettings _settings;

        private Dictionary<string, DataBase> _data;


        protected override void OnInstall()
        {
            _data = new Dictionary<string, DataBase>();

            foreach (var settingsDataBase in _settings.DataBases)
            {
                settingsDataBase.Init();
            }
        }
        public GameController SetSettings(DataBaseControllerSettings a)
        {
            _settings = a;

            return this;
        }

        public static T LoadData<T>(string path) where T : DataBase
        {
            var n = typeof(T).Name;

            var asset = Resources.Load<DataBase>(path + n);
            if (asset == null)
            {
                Debug.Log("Cant find data " + n);
                return default;
            }

            var d = UnityEngine.Object.Instantiate(asset);
            d.Init();

            return d as T;
        }

        private T LoadDataToList<T>() where T : DataBase
        {
            var n = typeof(T).Name;

            if (!_data.ContainsKey(n))
            {
                var asset = Resources.Load<DataBase>(_settings.Path + n);
                if (asset == null)
                {
                    Debug.Log("Cant find data " + n);
                    return default;
                }

                var d = UnityEngine.Object.Instantiate(asset);
                _data.Add(n, d);
                d.Init();
            }

            return _data[n] as T;
        }

        public T Get<T>() where T : DataBase
        {
            var n = typeof(T).Name;

            if (_data.ContainsKey(n))
            {
                return (T)_data[n];
            }
            return LoadDataToList<T>();
        }

    }
}
