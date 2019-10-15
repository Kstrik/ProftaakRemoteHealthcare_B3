using Networking.Client;
using Networking.HealthCare;
using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareDoctor.Net
{
    public class HealthCareDoctor : IServerDataReceiver
    {
        Client client;
        IMessageReceiver receiver;

        public HealthCareDoctor(string ip, int port, IMessageReceiver receiver)
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
