using UnityEngine;

namespace MirrorNetwork
{
    public class AuthoritativeServerStarter : MonoBehaviour
    {
        [SerializeField] private AuthoritativeServer _server;

        private void Start()
        {
            _server.ConnectServer();

            //  _server.NetworkHandlerController.AddHandler<LoginServerNetworkHandler>();
        }
    }
}
