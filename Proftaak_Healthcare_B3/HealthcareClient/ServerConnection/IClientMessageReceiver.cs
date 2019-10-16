using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthcareClient.BikeConnection;

namespace HealthcareClient.ServerConnection
{
    public interface IClientMessageReceiver
    {
        void HandleClientMessage(ClientMessage clientMessage);
    }
}
