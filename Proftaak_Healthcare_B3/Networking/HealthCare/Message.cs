using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.HealthCare
{
    public class Message
    {
        [Flags] public enum MessageTypes
        {
            BIKEDATA = 0x01,
            CHAT_MESSAGE = 0x02,
            CLIENT_LOGIN = 0x03,
            DOCTOR_LOGIN = 0x04,
            CHANGE_RESISTANCE = 0x05,
            SERVER_ERROR = 0x06,
            SERVER_OK = 0x07
        }

        [Flags] public enum ValueIds
        {
            HEARTRATE = 0x01,
            DISNTANCE = 0x02,
            SPEED = 0x03,
            CYCLE_RITHM = 0x04
        }

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

        public byte GetPrefix()
        {
            return (byte)(this.IdPrefix & 127);
        }

        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(this.MessageLength);
            bytes.Add(this.IdPrefix);
            bytes.AddRange(this.ContentMessage);
            return bytes.ToArray();
        }
    }
}
