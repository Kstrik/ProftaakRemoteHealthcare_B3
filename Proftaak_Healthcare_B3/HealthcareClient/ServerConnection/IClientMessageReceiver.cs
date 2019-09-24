using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthcareClient.BikeConnection;

namespace HealthcareClient.ServerConnection
{
    interface IClientMessageReceiver
    {
        void handleClientMessage(ClientMessage clientMessage);
    }
}
