using System;
using System.Collections.Generic;
using System.Linq;
using Assets.JTI.Scripts.Events.Singleton;
using JTI.Scripts.Common;
using JTI.Scripts.Managers;
using MirrorNetwork.NetworkHandlers;
using UnityEngine;

namespace MirrorNetwork
{
    public class AuthoritativeServer : SingletonMonoSimple<AuthoritativeServer>, INetworkAgent
    {
        public Action<AuthoritativeServerClient> OnClientDisconnectEvent;
        public Action<AuthoritativeServerClient> OnClientConnectEvent;
        public Action<NetworkMessage> OnGetMessageEvent;

        [System.Serializable]
        public class AuthoritativeServerClient
        {
            public Action OnDisconnectEvent;
            public Action<NetworkMessage> OnGetMessageEvent;

            public NetworkHandlerController NetworkHandlerController;

            public int ConnectionId;

            public void GetMessage(NetworkMessage message)
            {
                OnGetMessageEvent?.Invoke(message);
            }

            public AuthoritativeServerClient(int connectionId, INetworkAgent i)
            {
                ConnectionId = connectionId;
                NetworkHandlerController =
                    new GameObject(ConnectionId.ToString()).AddComponent<NetworkHandlerController>();
                NetworkHandlerController.Initialize(i);

            }

            public void Disconnected()
            {
                if (NetworkHandlerController != null)
                {
                    Destroy(NetworkHandlerController.gameObject);
                }
              
                OnDisconnectEvent?.Invoke();
            }
        }

        [SerializeField] private AuthoritativeNetworkManager _manager;

        public AuthoritativeNetworkManager Manager => _manager;

        public NetworkHandlerController NetworkHandlerController { get; private set; }
        public HashSet<AuthoritativeServerClient> Clients { get; private set; }

        private EventSubscriber _subscriber;

        private bool _connected;

#if UNITY_EDITOR
        public List<AuthoritativeServerClient> ClientsEditor;
#endif


        protected override void OnAwaken()
        {
            base.OnAwaken();

            _subscriber = new EventSubscriber(this);
            Clients = new HashSet<AuthoritativeServerClient>();
            NetworkHandlerController = gameObject.AddComponent<NetworkHandlerController>();
            NetworkHandlerController.Initialize(this);
        }

        private void OnDestroy()
        {
            _subscriber?.Destroy();
        }

#if UNITY_EDITOR

        private void EditorUpdate()
        {
            ClientsEditor = new List<AuthoritativeServerClient>();

            foreach (var authoritativeServerClient in Clients)
            {
                ClientsEditor.Add(authoritativeServerClient);
            }
        }
#endif
        private void Update()
        {
#if UNITY_EDITOR
            EditorUpdate();
#endif
        }

        public void SendToClient<T>(int id, T data) where T : Package
        {
            LogManager.Log("Send " + data + " to " + id);

            var str = System.Text.Encoding.UTF8.GetBytes(data.ToJson());

            _manager.Transport.ServerSend(id, str);
        }

        public void SendToClient<T>(AuthoritativeServerClient client, T data) where T : Package
        {
            if (client == null)
                return;

            LogManager.Log("Send " + data + " to " + client.ConnectionId);

            var str = System.Text.Encoding.UTF8.GetBytes(data.ToJson());

            _manager.Transport.ServerSend(client.ConnectionId, str);
        }

        public void SendToClients<T>(int id, T data) where T : Package
        {
            var str = System.Text.Encoding.UTF8.GetBytes(data.ToJson());

            _manager.Transport.ServerSend(id, str);
        }

        public void ConnectServer()
        {
            if (_connected) return;

            _connected = true;

            _manager.ConnectServer();

            _manager.Transport.OnServerDataReceived = OnServerDataReceived;
            _manager.Transport.OnServerConnected = OnClientConnected;
            _manager.Transport.OnServerDisconnected = OnClientDisconnected;
            _manager.Transport.OnServerDataSent = OnServerDataSent;
            _manager.Transport.OnServerError = OnServerError;

            AuthoritativeNetworkLoop.SetTransport(_manager.Transport);
        }

        private void OnServerError(int i, Exception e)
        {
            LogManager.LogError(i + ":" + e);
        }

        private void OnServerDataSent(int id, ArraySegment<byte> data, int channel)
        {

        }

        private void OnClientConnected(int i)
        {
            var client = new AuthoritativeServerClient(i, this);
            Clients.Add(client);
            OnClientConnectEvent?.Invoke(client);


        }

        private void OnClientDisconnected(int i)
        {
            var exist = Clients.FirstOrDefault(x => x.ConnectionId == i);

            if (exist != null)
            {
                exist.Disconnected();
                OnClientDisconnectEvent?.Invoke(exist);
                Clients.Remove(exist);
            }
        }

        private void OnServerDataReceived(int id, ArraySegment<byte> data, int channel)
        {
            var result = System.Text.Encoding.UTF8.GetString(data).FromJson<Package>();

            result.Data.SetClient(Clients.FirstOrDefault(x => x.ConnectionId == id));

            if (result.Data == null) return;

            LogManager.Log("On Get Data " + result?.Data + " from " + result.Data.GetClient().ConnectionId);

            result.Data.GetClient().GetMessage(result.Data);

            OnGetMessageEvent?.Invoke(result.Data);

        }


        public void Disconnect(AuthoritativeServerClient client)
        {
            _manager.Transport.ServerDisconnect(client.ConnectionId);
        }
    }
}