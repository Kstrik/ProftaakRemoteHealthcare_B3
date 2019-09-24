using Networking.HealthCare;
using Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer
{
    public class HealthcareServer : IClientDataReceiver
    {
        private Server server;

        public HealthcareServer(string ip, int host)
        {
            this.server = new Server(ip, host, this, null, null);
        }

        public void OnDataReceived(byte[] data, string clientId)
        {
            Message message = Message.ParseMessage(data);

            //switch(message.GetPrefix())
            //{

            //}
        }
    }
}
