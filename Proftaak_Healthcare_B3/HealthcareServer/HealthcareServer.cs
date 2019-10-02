using Networking.Client;
using Networking.HealthCare;
using Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer
{
    public class HealthCareServer : IClientDataReceiver, IServerConnector
    {
        private Server server;
        private Client client;
        private ILog logger;

       

        public HealthCareServer(string ip, int host, ILog logger)
        {
            this.server = new Server(ip, host, this, this, null);
            this.server.Start();
            this.client = new Client("127.0.0.1", 1331, null, null);
            this.client.Connect();
            this.logger = logger;

            byte[] bytes = new byte[2];
            bytes[0] = 1;
            bytes[1] = 20;
            Message message = new Message(false, 1, bytes);
            this.client.Transmit(message.GetBytes());
        }

        public void OnDataReceived(byte[] data, string clientId)
        {
            Message message = Message.ParseMessage(data);

            switch ((Message.MessageTypes)message.GetPrefix())
            {
                case Message.MessageTypes.BIKEDATA:
                    {
                        RecieveBikeData(message.ContentMessage);
                        break;
                    }
                case Message.MessageTypes.CHAT_MESSAGE:
                    {
                        this.logger.Log($"Chatmessage: {Encoding.UTF8.GetString(message.ContentMessage)}");
                        break;
                    }
                case Message.MessageTypes.CLIENT_LOGIN:
                    {
                        this.logger.Log($"Cliënt login");
                        break;
                    }
                case Message.MessageTypes.DOCTOR_LOGIN:
                    {
                        this.logger.Log($"Doctor login");
                        break;
                    }
                case Message.MessageTypes.CHANGE_RESISTANCE:
                    {
                        this.logger.Log($"Change resistance");
                        break;
                    }
                default:
                    {
                        this.logger.Log($"Unkown message type");
                        break;
                    }
            }
        }

        private void RecieveBikeData(byte[] bikeData)
        {
           
            //TODO callhandler + handle function 
            //and send the unprocessed data to the dokter

        }


       


        public void OnClientDisconnected(ClientConnection connection)
        {
            

        }

        public void OnClientConnected(ClientConnection connection)
        {
            


        }
    }
}
