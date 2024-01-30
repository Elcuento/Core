using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorNetwork
{
    public interface INetworkHandler
    {
        public void Initialize(INetworkAgent g);
    }
}
