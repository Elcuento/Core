using System;
using Assets.JTI.Scripts.Events.Singleton;
using JTI.Scripts.Common;
using JTI.Scripts.Managers;
using MirrorNetwork.NetworkHandlers;
using UnityEngine;

namespace MirrorNetwork
{
    public class AuthoritativeClient : SingletonMonoSimple<AuthoritativeClient>, INetworkAgent
    {
        public Action OnDisconnectedEvent;
        public Action OnConnectedEvent;
        public Action<NetworkMessage> OnGetMessageEvent;

        [SerializeField] private AuthoritativeNetworkManager _manager;
        public NetworkHandlerController NetworkHandlerController { get; private set; }
        public AuthoritativeNetworkManager Manager => _manager;

        private EventSubscriber _subscriber;
        public bool IsConnected { get; private set; }

        private bool _connected;

        protected override void OnAwaken()
        {
            base.OnAwaken();
            _subscriber = new EventSubscriber(this);
            NetworkHandlerController = gameObject.AddComponent<NetworkHandlerController>();
            NetworkHandlerController.Initialize(this);
        }


        private void OnDestroy()
        {
            _subscriber?.Destroy();
        }

        public void ConnectClient()
        {
            if (_connected) return;

            _connected = true;

            _manager.ConnectClient();

            _manager.Transport.OnClientDataReceived = OnClientDataReceived;
            _manager.Transport.OnClientConnected = OnClientConnected;
            _manager.Transport.OnClientDisconnected = OnClientDisconnected;
            _manager.Transport.OnClientDataSent = OnServerDataSent;
            _manager.Transport.OnClientError = OnServerError;

            AuthoritativeNetworkLoop.SetTransport(_manager.Transport);
        }

        public void SendToClient<T>(int id, T data) where T : Package
        {
            var str = System.Text.Encoding.UTF8.GetBytes(data.ToJson());

            _manager.Transport.ServerSend(id, str);
        }

        public void SendToServer<T>(T data) where T : Package
        {
            LogManager.Log("Send " + data);

            var str = System.Text.Encoding.UTF8.GetBytes(data.ToJson());

            _manager.Transport.ClientSend(str);
        }

        private void OnClientDataReceived(ArraySegment<byte> data, int channel)
        {
            var result = System.Text.Encoding.UTF8.GetString(data).FromJson<Package>();

            LogManager.Log("On Get Data " + result.Data);

            OnGetMessageEvent?.Invoke(result.Data);
        }

        private void OnServerError(Exception e)
        {
            LogManager.LogError(":" + e);
        }

        private void OnServerDataSent(ArraySegment<byte> data, int channel)
        {

        }

        private void OnClientConnected()
        {
            IsConnected = true;
            OnConnectedEvent?.Invoke();
        }

        private void OnClientDisconnected()
        {
            _connected = false;
            IsConnected = false;
            OnDisconnectedEvent?.Invoke();
        }

        public void Disconnect()
        {
            _connected = false;
            _manager.Transport.ClientDisconnect();
        }
    }
}
