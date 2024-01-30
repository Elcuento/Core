using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MirrorNetwork.NetworkHandlers
{
    public class NetworkHandlerController : MonoBehaviour
    {
        public List<INetworkHandler> Handlers;
        public INetworkAgent Agent;

        public void Initialize(INetworkAgent i)
        {
            Handlers = new List<INetworkHandler>();

            Agent = i;
        }

        public T GetHandler<T>() where T : MonoBehaviour, INetworkHandler
        {
            return Handlers.FirstOrDefault(x => x is T) as T;
        }

        public T AddHandler<T>(T t) where T : MonoBehaviour, INetworkHandler
        {
            Handlers.Add(t);

            t.Initialize(Agent);
            
            DontDestroyOnLoad(t.gameObject);

            t.transform.SetParent(transform);

            return t;

        }
        public T AddHandler<T>() where T : MonoBehaviour, INetworkHandler
        {
            var o = new GameObject(typeof(T).Name).AddComponent<T>();
            o.transform.SetParent(transform);

            DontDestroyOnLoad(o.gameObject);

            Handlers.Add(o);

            o.Initialize(Agent);

            return o;
        }

    }
}
