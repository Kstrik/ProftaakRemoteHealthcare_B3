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

            switch ((Message.MessageTypes)message.GetPrefix())
            {
                case Message.MessageTypes.BIKEDATA:
                    {
                        break;
                    }
                case Message.MessageTypes.CHAT_MESSAGE:
                    {
                        break;
                    }
                case Message.MessageTypes.CLIENT_LOGIN:
                    {
                        break;
                    }
                case Message.MessageTypes.DOCTOR_LOGIN:
                    {
                        break;
                    }
                case Message.MessageTypes.CHANGE_RESISTANCE:
                    {
                        break;
                    }
                case Message.MessageTypes.SERVER_ERROR:
                    {
                        break;
                    }
                case Message.MessageTypes.SERVER_OK:
                    {
                        break;
                    }
            }
        }
    }
}
