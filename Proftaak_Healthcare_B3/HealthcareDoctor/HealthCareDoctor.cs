﻿using Networking.Client;
using Networking.HealthCare;
using Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareDoctor
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
            byte[] decryptedData = Encoding.UTF8.GetBytes(DataEncryptor.Decrypt(Encoding.UTF8.GetString(data), "Test"));
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
            byte[] encryptedMessage = Encoding.UTF8.GetBytes(DataEncryptor.Encrypt(Encoding.UTF8.GetString(message.GetBytes()), "Test"));
            this.client.Transmit(encryptedMessage);
        }
    }
}
