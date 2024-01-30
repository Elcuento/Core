using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MirrorNetwork
{
    public class NetworkMessage
    {
        [JsonIgnore]
        private AuthoritativeServer.AuthoritativeServerClient _client;

        public AuthoritativeServer.AuthoritativeServerClient GetClient()
        {
            return _client;
        }
        public void SetClient(AuthoritativeServer.AuthoritativeServerClient client)
        {
            _client = client;
        }
    }
}
