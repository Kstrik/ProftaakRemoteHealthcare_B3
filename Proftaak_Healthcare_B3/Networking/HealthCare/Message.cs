using System;
using System.Collections.Generic;

namespace Networking.HealthCare
{
    public class Message
    {
        public enum MessageType : byte
        {
            BIKEDATA,
            CHAT_MESSAGE,
            CLIENT_LOGIN,
            DOCTOR_LOGIN,
            CHANGE_RESISTANCE,
            SERVER_ERROR,
            SERVER_OK,
            GET_CLIENT_HISTORY,
            CLIENT_HISTORY_START,
            CLIENT_HISTORY_END,
            CLIENT_HISTORY_DATA
        }

        public enum ValueId : byte
        {
            HEARTRATE,
            DISTANCE,
            SPEED,
            CYCLE_RHYTHM
        }

        public bool isDoctor { get; }
        public MessageType messageType{ get; }
        public byte[] Content { get; }

        public Message(bool isDoctor, MessageType messageType, byte[] content)
        {
            this.isDoctor = isDoctor;
            this.messageType = messageType;

            if (content == null)
                content = new byte[0];

            this.Content = content;
        }
        /// <summary>
        /// This method allows the receiver of a byte array via TCP, to rebuild that into a Message class
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns></returns>
        public static Message ParseMessage(byte[] messageData) 
        {
            List<byte> bytes = new List<byte>(messageData);
            //decompress boolean and enum from one byte:
            bool isDoctor = bytes[1] >> 7 == 1;
            MessageType messageType = (MessageType) (bytes[1] & 127);
            //grab data according to message length in first byte:
            byte[] contentMessage = bytes.GetRange(2, (int)bytes[0]).ToArray(); 
            return new Message(isDoctor, messageType, contentMessage);
        }

        /// <summary>
        /// This method allows the sender of data to parse it into a byte array that conforms to our network protocol.
        /// This byte array is ready to be sent via TCP
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add((byte)this.Content.Length);
            //compress boolean and eum into one byte:
            byte IdPrefix = (isDoctor) ? (byte)1 : (byte)0;
            IdPrefix = (byte)(IdPrefix << 7);
            IdPrefix += (byte)messageType;
            bytes.Add(IdPrefix);

            bytes.AddRange(this.Content);
            return bytes.ToArray();
        }
    }
}
