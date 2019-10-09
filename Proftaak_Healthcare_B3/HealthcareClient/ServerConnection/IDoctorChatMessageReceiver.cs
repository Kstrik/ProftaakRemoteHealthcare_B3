using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareClient.ServerConnection
{
    interface IDoctorChatMessageReceiver
    {
        void handleChatMessage(string message);
    }
}
