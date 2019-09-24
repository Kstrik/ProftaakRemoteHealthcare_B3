using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.Server
{
    public interface IClientDataReceiver
    {
        void OnDataReceived(byte[] data, string clientId);
    }
}
