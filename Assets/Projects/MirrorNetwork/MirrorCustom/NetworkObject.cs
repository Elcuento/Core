using System;
using Mirror;
using UnityEngine;

namespace MirrorNetwork
{
    public class NetworkObject : NetworkBehaviour
    {
        public Action<string,string> OnChangeOwnerEvent;

        [SyncVar(hook = nameof(SyncId))] public string Id;
        [SyncVar(hook = nameof(SyncOwnerId))] public string OwnerId;

        public override void OnStartServer()
        {
            base.OnStartServer();
        }

        private void Awake()
        {
            Id = Guid.NewGuid().ToString();

            OnAwake();
        }

        protected virtual void OnAwake()
        {

        }

        private void SyncId(string was, string become)
        {
            Id = become;
        }

        protected virtual void SyncOwnerId(string was, string become)
        {
            OwnerId = become;
            OnChangeOwnerEvent?.Invoke(was, OwnerId);
        }
    }
}
