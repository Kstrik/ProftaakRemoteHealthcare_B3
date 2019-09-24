using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.Server
{
    public interface IServerConnector
    {
        void OnClientConnected(ClientConnection connection);
        void OnClientDisconnected(ClientConnection connection);
    }
}
