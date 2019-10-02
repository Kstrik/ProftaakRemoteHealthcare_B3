using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer
{
    interface IMessageHandler
    {

        void HandleMessage(Byte[] data);

    }
}
