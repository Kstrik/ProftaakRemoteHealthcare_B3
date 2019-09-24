using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.HealthCare
{
    public class Message
    {
        public byte MessageLength { get; }
        public byte IdPrefix { get; }
        public byte[] ContentMessage { get; }

        public Message(bool isDoctor, byte prefix, byte[] message)
        {
            this.MessageLength = (byte)message.Length;
            this.ContentMessage = message;
            this.IdPrefix = (isDoctor) ? (byte)1 : (byte)0;
            this.IdPrefix = (byte)(this.IdPrefix << 7);
            this.IdPrefix += prefix;
        }

        public static Message ParseMessage(byte[] messageData)
        {
            List<byte> bytes = new List<byte>(messageData);
            return new Message((bytes[0] == 1), bytes[1], bytes.GetRange(2, (int)bytes[0]).ToArray());
        }

        private byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>(){
                this.MessageLength,
                this.IdPrefix,
            };
            bytes.AddRange(this.ContentMessage);
            return bytes.ToArray();
        }
    }
}
