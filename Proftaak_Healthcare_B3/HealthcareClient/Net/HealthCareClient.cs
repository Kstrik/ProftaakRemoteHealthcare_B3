using Networking;
using Networking.Client;
using Networking.HealthCare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareClient.Net
{
    public class HealthCareClient : IServerDataReceiver
    {
        Client client;
        IMessageReceiver receiver;

        public HealthCareClient(string ip, int port, IMessageReceiver receiver)
        {
            client = new Client(ip, port, this, null);
            this.receiver = receiver;
        }

        public void OnDataReceived(byte[] data)
        {
            byte[] decryptedData = DataEncryptor.Decrypt(data, "Test");
            this.receiver?.OnMessageReceived(Message.ParseMessage(decryptedData));
        }

        public bool Connect()
        {
            return this.client.Connect();
        }

        public void Disconnect()
        {
            this.client.Disconnect();
        }

        public void Transmit(Message message)
        {
            byte[] encryptedMessage = DataEncryptor.Encrypt(message.GetBytes(), "Test");
            this.client.Transmit(encryptedMessage);
        }

        public void SetReciever(IMessageReceiver receiver)
        {
            this.receiver = receiver;
        }
    }
}
