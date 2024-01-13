using System;
using System.Collections.Generic;
using System.IO;
using JTI.Scripts.Common;
using UnityEngine;

namespace JTI.Scripts.Storage
{
    public class FileStorageEnum<T> where T : Enum
    {
        public class ValueChange
        {
            public object Was;
            public object Become;
            public T ValueType;
        }

        public Action<ValueChange> OnValueChangeEvent;

        protected class Variable
        {
            public string Name;
            public object Value;
            public bool Saved;
        }

        protected List<Variable> _values;
        protected List<string> _listNames;
        protected bool _noSave;
        protected virtual string _path { get; } = Application.persistentDataPath;
        public string Prefix => Application.isEditor ? ".txt" : "";

        public void UnLoad<T>(Enum type)
        {
            var n = Convert.ToInt32(type);

            if (IsExist(type))
            {
                _values.RemoveAt(n);
            }
        }

        public void SetSave(bool a)
        {
            _noSave = !a;
        }

        protected void SetUnSave(Variable aVariable)
        {
            if (aVariable != null)
            {
                aVariable.Saved = false;
            }
        }
        protected void SetUnSave(Enum type)
        {
            var n = Convert.ToInt32(type);

            if (_values[n] != null)
            {
                _values[n].Saved = false;
            }
        }

        public void Subscribe(Action<ValueChange> ev)
        {
            OnValueChangeEvent -= ev;
            OnValueChangeEvent += ev;
        }
        public void Unsubscribe(Action<ValueChange> ev)
        {
            OnValueChangeEvent -= ev;
        }

        public FileStorageEnum()
        {
            _values = new List<Variable>();
            _listNames = new List<string>();

            var names = Enum.GetNames(typeof(T));

            for (var i = 0; i < names.Length; i++)
            {
                _values.Add(null);
                _listNames.Add(names[i]);
            }

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

        public virtual string GetIndexName(int i)
        {
            return _listNames[i]; // TODO SUPER SLOW
        }


        protected virtual void SaveVariable(Variable n)
        {
            if (_noSave)
                return;

            var pos = _values.IndexOf(n);

            n.Saved = true;

            try
            {
                File.WriteAllText(_path + GetIndexName(pos) + Prefix, n.Value.ToJson());
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

        }

        public virtual void Save()
        {

            for (var index = 0; index < _values.Count; index++)
            {
                var variable = _values[index];
                if (variable == null || variable.Saved) continue;

                SaveVariable(variable);
            }
        }

        public void Set(Enum type, object val, bool save = true)
        {
            var n = Convert.ToInt32(type);

            var was = _values[n]?.Value;

            if (_values[n] == null)
            {
                _values[n] = new Variable() { Saved = false, Value = val };
                _values[n].Saved = false;
            }
            else
            {
                _values[n].Value = val;
                _values[n].Saved = false;
            }

            try
            {
                OnValueChangeEvent?.Invoke(new ValueChange()
                {
                    Become = val,
                    Was = was,
                    ValueType = (T)(object)n,
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }


            if (save)
            {
                SaveVariable(_values[n]);
            }
        }

        public void Destroy(Enum type)
        {
            var n = Convert.ToInt32(type);
            var p = $"{_path}" + type;

            var was = _values[n]?.Value;

            try
            {
                if (File.Exists(p + Prefix))
                {
                    Debug.Log("remove done " + type);
                    File.Delete(p + Prefix);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            try
            {
                OnValueChangeEvent?.Invoke(new ValueChange()
                {
                    Become = null,
                    Was = was,
                    ValueType = (T)(object)n,
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }


            _values[n] = null;
        }

        public TU Load<TU>(Enum type)
        {
            var n = Convert.ToInt32(type);

            var was = _values[n]?.Value;

            if (_values[n] != null)
            {
                return (TU)_values[n].Value;
            }

            try
            {
                try
                {
                    var v = File.ReadAllText(_path + type + Prefix).FromJson<TU>();
                    _values[n] = new Variable() { Saved = true, Value = v };
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
                OnValueChangeEvent?.Invoke(new ValueChange()
                {
                    Become = _values[n].Value,
                    Was = was,
                    ValueType = (T)(object)n,
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }


            return (TU)_values[n]?.Value;

        }

        public TU LoadDefault<TU>(Enum type, TU def)
        {
            var n = Convert.ToInt32(type);

            var was = _values[n]?.Value;

            if (_values[n] != null)
            {
                if (_values[n].Value == null)
                {
                    _values[n].Value = def;
                }
                else
                {
                    return (TU)_values[n].Value;
                }
            }

            try
            {
                if (File.Exists(_path + type + Prefix))
                {
                    var v = File.ReadAllText(_path + type + Prefix).FromJson<TU>();

                    if (v != null)
                    {
                        _values[n] = new Variable() { Saved = true, Value = v };
                    }
                }
                else
                {
                    _values[n] = new Variable() { Saved = true, Value = def };
                }
            }
            catch (Exception e)
            {
                // ignored
            }

            if (_values[n] == null)
            {
                Set(type, def, true);
            }

            try
            {
                OnValueChangeEvent?.Invoke(new ValueChange()
                {
                    Become = _values[n].Value,
                    Was = was,
                    ValueType = (T)(object)n,
                });

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }



            return (TU)_values[n].Value;
        }

        public bool IsExist(Enum type)
        {
            var n = Convert.ToInt32(type);

            if (_values[n] != null)
                return true;

            var res = false;

            try
            {
                res = File.Exists(_path + type + Prefix);
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
