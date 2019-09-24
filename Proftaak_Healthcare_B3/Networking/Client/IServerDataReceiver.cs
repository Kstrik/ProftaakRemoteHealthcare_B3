using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.Client
{
    public interface IServerDataReceiver
    {
        void OnDataReceived(byte[] data);
    }
}
