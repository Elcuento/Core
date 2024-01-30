
using kcp2k;
using Mirror;
using UnityEngine;

namespace MirrorNetwork
{
    public class AuthoritativeNetworkManager : MonoBehaviour
    {
        [SerializeField] private string _address;
        [SerializeField] private Transport _transport;
        [SerializeField] private bool _dontDestroyOnLoad = true;
        public Transport Transport => _transport;

        private void Awake()
        {

            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            if (_transport == null)
            {
                _transport = gameObject.AddComponent<KcpTransport>();
            }
        }


        public void ConnectServer()
        {
            if (_transport.ServerActive()) return;

            _transport.ServerStart();
        }

        public void ConnectClient()
        {
            if (_transport.ClientConnected()) return;

            _transport.ClientConnect(_address);
        }
    }
}