using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Projects.JTI.Scripts.Common
{
    using System.Collections.Generic;
    using UnityEngine;

    namespace BlackTemple.Common
    {
        public class Pool : MonoBehaviour
        {
            public int PoolSize => _pool.Count;

            public List<MonoBehaviour> _pool = new List<MonoBehaviour>();

            private bool _isApplicationQuiting;


            protected virtual void OnApplicationQuit()
            {
                _isApplicationQuiting = true;
            }

            public static Pool CreatePool(string poolName)
            {
                return new GameObject($"{poolName}_pool").AddComponent<Pool>();
            }

            public static Pool CreatePool(string poolName, Transform parent)
            {
                var pool = new GameObject($"{poolName}_pool").AddComponent<Pool>();
                pool.transform.SetParent(parent);
                return pool;
            }

            public void Clear()
            {
                foreach (var o in _pool)
                {
                    if (o != null)
                        Destroy(o.gameObject);
                }
                _pool.Clear();
            }

            public void ToPool<T>(T obj) where T : MonoBehaviour
            {
                if (_isApplicationQuiting)
                    return;

                obj.transform.SetParent(transform);

                if (!_pool.Contains(obj))
                    _pool.Add(obj);


                obj.gameObject.SetActive(false);
            }

            public T FromPool<T>() where T : MonoBehaviour
            {
                for (var i = 0; i < _pool.Count; i++)
                {
                    if (_pool[i] == null)
                    {
                        _pool.Remove(_pool[i]);
                        i--;
                    }
                }

                var first = _pool.Find(x => !x.gameObject.activeSelf);
                if (first != null)
                {

                    first.gameObject.SetActive(true);
                    _pool.Remove(first);
                    return first.GetComponent<T>();
                }

                return null;
            }

            public GameObject FromPool()
            {
                for (var i = 0; i < _pool.Count; i++)
                {
                    if (_pool[i] == null)
                    {
                        _pool.Remove(_pool[i]);
                        i--;
                    }
                }

                var first = _pool.Find(x => !x.gameObject.activeSelf);
                if (first != null)
                {

                    first.gameObject.SetActive(true);
                    _pool.Remove(first);
                    return first.gameObject;
                }

                return null;
            }

            public void Initialize(int count, MonoBehaviour poolObj)
            {
                for (var i = 0; i < count; ++i)
                {
                    var item = Instantiate(poolObj);
                    ToPool(item);
                }
            }
        }
    }
}
