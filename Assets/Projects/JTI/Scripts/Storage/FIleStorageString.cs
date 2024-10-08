﻿using System;
using System.Collections.Generic;
using System.IO;
using JTI.Scripts.Common;
using UnityEngine;

namespace JTI.Scripts.Storage
{
    public class FileStorageString
    {
        public class ValueChangeConverted<T>
        {
            public T Was;
            public T Become;
            public string Key;
        }
        public class ValueChange
        {
            public object Was;
            public object Become;
            public string Key;
        }

        public string Prefix => Application.isEditor ? ".txt" : "";

        protected class Variable
        {
            public string Key;
            public object Value;
            public bool Saved;
        }

        protected Dictionary<string,Variable> _values;

        protected bool _noSave;
        protected bool _autoSave;
        protected Dictionary<string, Action<ValueChange>> _variableUpdateDictionary;
        protected virtual string _path { get; } = Application.persistentDataPath +"/Saves/";


        public void UnLoad<T>(string key)
        {
            if (IsExist(key))
            {
                _values.Remove(key);
            }
        }

        public void SetSave(bool a)
        {
            _noSave = !a;
        }

        public void SetAutosave( bool a)
        {
            _autoSave = a;
        }
        protected void SetUnSave(Variable aVariable)
        {
            if (aVariable != null)
            {
                aVariable.Saved = false;
            }
        }
        protected void SetUnSave(string key)
        {
            if (_values.ContainsKey(key))
            {
                _values[key].Saved = false;
            }
        }

        public ValueChangeConverted<TV> GetValueConverted<TV>(ValueChange a)
        {
            var vChange = new ValueChangeConverted<TV>();

            if (a == null)
            {
                return vChange;
            }
            try
            {
                vChange.Key = a.Key;
                vChange.Become = (TV)a.Become;
                vChange.Was = (TV)a.Was;
            }
            catch (Exception e)
            {
                return vChange;
            }

            return vChange;
        }

        public void FireVariableChange(ValueChange aChange)
        {
            if (_variableUpdateDictionary.ContainsKey(aChange.Key))
            {
                _variableUpdateDictionary[aChange.Key]?.Invoke(aChange);
            }
        }
        public void Subscribe(string key, Action<ValueChange> a)
        {
            if (_variableUpdateDictionary.ContainsKey(key))
            {
                _variableUpdateDictionary[key] += a;
            }
            else
            {
                _variableUpdateDictionary.Add(key, null);
                _variableUpdateDictionary[key] += a;
            }
        }
        public void Unsubscribe(string key, Action<ValueChange> a)
        {
            if (_variableUpdateDictionary.ContainsKey(key))
            {
                _variableUpdateDictionary[key] -= a;
            }
        }

        public FileStorageString()
        {
            _autoSave = true;
            _values = new Dictionary<string, Variable>();
            _variableUpdateDictionary = new Dictionary<string, Action<ValueChange>>();

            try
            {
                if (!Directory.Exists(_path))
                    Directory.CreateDirectory(_path);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }


        protected virtual void SaveVariable(Variable n)
        {
            if (_noSave)
                return;

            n.Saved = true;

            try
            {
                File.WriteAllText(_path + n.Key + Prefix, n.Value.ToJson());
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

        }

        public virtual void Save()
        {
            foreach (var variable in _values)
            {
                if (variable.Value == null || variable.Value.Saved) continue;
                SaveVariable(variable.Value);
            }
        }

        public void Set(string key, object val, bool save = true)
        {
            var was = _values.ContainsKey(key) ? _values[key] : null;

            if (!_values.ContainsKey(key))
            {
                _values.Add(key, new Variable()
                {
                    Saved = false,
                    Value = val,
                    Key = key
                });
            }



            if (_values[key] == null)
            {
                _values[key] = new Variable() { Saved = false, Value = val, Key = key};
                _values[key].Saved = false;
            }
            else
            {
                _values[key].Value = val;
                _values[key].Saved = false;
            }

            try
            {
                FireVariableChange(new ValueChange()
                {
                    Become = val,
                    Was = was,
                    Key = key
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }


            if (save)
            {
                SaveVariable(_values[key]);
            }
        }

        public void Destroy(string key)
        {
            var p = $"{_path}" + key;

            var was = _values[key]?.Value;

            try
            {
                if (File.Exists(p + Prefix))
                {
                    File.Delete(p + Prefix);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            try
            {
                FireVariableChange(new ValueChange()
                {
                    Become = null,
                    Was = was,
                    Key = key
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }


            _values[key] = null;
        }

        public TU Load<TU>(string key)
        {
            var was = _values[key]?.Value;

            if (_values[key] != null)
            {
                return (TU)_values[key].Value;
            }

            try
            {
                try
                {
                    var v = File.ReadAllText(_path + key + Prefix).FromJson<TU>();
                    _values[key] = new Variable() { Saved = true, Value = v };
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

            }
            catch (Exception e)
            {
                // ignored
            }

            try
            {
                FireVariableChange(new ValueChange()
                {
                    Become = _values[key].Value,
                    Was = was,
                    Key = key
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }


            return (TU)_values[key]?.Value;

        }

        public TU LoadDefault<TU>(string key, TU def)
        {
            var was = _values.ContainsKey(key) ? _values[key]?.Value : null;

            if (_values.ContainsKey(key) && _values[key] != null)
            {
                if (_values[key].Value == null)
                {
                    _values[key].Value = def;
                }
                else
                {
                    return (TU)_values[key].Value;
                }
            }

            try
            {
                if (File.Exists(_path + key + Prefix))
                {
                    var v = File.ReadAllText(_path + key + Prefix).FromJson<TU>();

                    if (v != null)
                    {
                        Set(key, v);
                        return v;
                    }
                }
                else
                {
                    Set(key, def);
                    return def;
                }
            }
            catch (Exception e)
            {
                // ignored
            }

            if (_values[key] == null)
            {
                Set(key, def, true);
            }

            try
            {
                FireVariableChange(new ValueChange()
                {
                    Become = _values[key].Value,
                    Was = was,
                    Key = key
                });

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }



            return (TU)_values[key].Value;
        }

        public bool IsExist(string key)
        {
            if (_values.ContainsKey(key))
                return true;

            var res = false;

            try
            {
                res = File.Exists(_path + key + Prefix);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return res;

        }

        public void Clear()
        {
            try
            {
                var di = new DirectoryInfo(_path);

                foreach (var file in di.GetFiles())
                {
                    file.Delete();
                }

                foreach (var dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

        }
    }
}
