using Networking.Client;
using Networking.HealthCare;
using Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking.Client;

namespace HealthcareDoctor
{
    class DataManager
    {
        private List<TestClient> clients = new List<TestClient>();
        private Client TCPClient;
        public DataManager(Client TCPClient)
        {
            this.TCPClient = TCPClient;

        }

        public void SendLogin(string username, string password)
        {
            byte[] usernameBytes = Encoding.UTF8.GetBytes(username.PadRight(16)); //wat gebeurt er als de username langer dan 16 char is ?
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password.PadRight(16));
            byte[] messageContent = new byte[32];
            Buffer.BlockCopy(usernameBytes, 0, messageContent, 0, 16);
            Buffer.BlockCopy(passwordBytes, 0, messageContent, 0, 16);
            Message message = new Message(true, (byte)Message.MessageTypes.DOCTOR_LOGIN, messageContent);
            TCPClient.Transmit(message.GetBytes());
        }

        public void SendChatMessage(int clientID, string chatMessage)
        {
            byte[] chatMessageBytes = Encoding.UTF8.GetBytes(chatMessage);
            byte clientIDByte = (byte)clientID;
            chatMessageBytes.Prepend(clientIDByte);
            Message message = new Message(true, (byte)Message.MessageTypes.CHAT_MESSAGE, chatMessageBytes);
            TCPClient.Transmit(message.GetBytes());
        }

        public List<TestClient> GetClients()
        {
            //test waarden
            TestClient client1 = new TestClient(1, "levi");
            TestClient client2 = new TestClient(2, "test");
            TestClient client3 = new TestClient(3, "Kenley");

            clients.Add(client1);
            clients.Add(client2);
            clients.Add(client3);

            return clients;
        }

        public class TestClient
        {
            private int id;
            private string name;
           
            public TestClient(int id, string name)
            {
                this.id = id;
                this.name = name;
            }

            public int GetId()
            {
                return id;
            }

            public string GetName()
            {
                return name;
            }
        }
    }
}
