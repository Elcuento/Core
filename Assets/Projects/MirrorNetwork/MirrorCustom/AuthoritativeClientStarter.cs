using System;
using UnityEngine;

namespace MirrorNetwork
{
    public class AuthoritativeNetworkClientStarter : MonoBehaviour
    {
        [SerializeField] private AuthoritativeClient _client;

        private void Start()
        {
            Setup();
        }


        public void Setup()
        {
            try
            {
                _client.ConnectClient();

                //  _client.NetworkHandlerController.AddHandler<GameNetworkHandler>();
            }
            catch (Exception e)
            {
                Debug.LogError(e);

            }
        }

    }
}