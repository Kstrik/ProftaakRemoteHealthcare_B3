using Networking.Client;
using Networking.HealthCare;
using Networking.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer
{
    public class HealthCareServer : IClientDataReceiver, IServerConnector
    {
        private Server server;
        private Client client;
        private ILog logger;
        private Dictionary<int, string> connectedClients;
        private List<Patient> patients;
       

        public HealthCareServer(string ip, int port, ILog logger)
        {
            this.server = new Server(ip, port, this, this, null);
            this.server.Start();
            /*this.client = new Client("127.0.0.1", 1331, null, null);
            this.client.Connect();*/
            this.logger = logger;

            byte[] bytes = new byte[2];
            bytes[0] = 1;
            bytes[1] = 20;
            connectedClients = new Dictionary<int, string>();
            patients = new List<Patient>();

            
           /* Message message = new Message(false, 1, bytes);
            this.client.Transmit(message.GetBytes());*/
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
                        //Todo: send message to proper client
                        int BSN = BitConverter.ToInt32(message.GetBytes(), 0);
                        byte[] chatBytes = message.GetBytes();
                        chatBytes.CopyTo(chatBytes, 4);
                        //string chatmessage = Encoding.UTF8.GetString(chatBytes);
                        Message chatMessage = new Message(true, (byte)Message.MessageTypes.CHAT_MESSAGE, chatBytes);
                        server.Transmit(chatMessage.GetBytes(), connectedClients[BSN]);
                        break;
                    }
                case Message.MessageTypes.CLIENT_LOGIN:
                    {
                        this.logger.Log($"Cliënt login");
                        int BSN = BitConverter.ToInt32(message.GetBytes(), 0);
                        byte[] nameBytes = message.GetBytes();
                        nameBytes.CopyTo(nameBytes, 4);
                        string name = Encoding.UTF8.GetString(nameBytes);
                        Patient newPatient = new Patient(BSN, name);
                        connectedClients.Add(BSN, clientId);
                        patients.Add(newPatient);
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
