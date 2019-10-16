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
            CLIENT_HISTORY_DATA,
            START_SESSION,
            STOP_SESSION,
            REMOVE_CLIENT,
            GET_CLIENTS,
            CLIENT_DATA
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
        private byte[] contentLength;
        public byte[] Content { get; }

        public Message(bool isDoctor, MessageType messageType, byte[] content)
        {
            this.isDoctor = isDoctor;
            this.messageType = messageType;

            if (content == null)
                content = new byte[0];

            this.contentLength = new byte[4];
            contentLength[0] = (byte)content.Length;
            contentLength[1] = (byte)(content.Length >> 8);
            contentLength[2] = (byte)(content.Length >> 16);
            contentLength[3] = (byte)(content.Length >> 24);

            this.Content = content;
        }

        public Message(bool isDoctor, MessageType messageType, byte[] contentLength, byte[] content)
        {
            this.isDoctor = isDoctor;
            this.messageType = messageType;
            this.contentLength = contentLength;
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
            byte[] contentLength = new byte[4] { bytes[0], bytes[1], bytes[2], bytes[3] };

            //decompress boolean and enum from one byte:
            bool isDoctor = bytes[4] >> 7 == 1;
            MessageType messageType = (MessageType)(bytes[4] & 127);

            //grab data according to message length in first byte:
            byte[] contentMessage = bytes.GetRange(5, bytes.Count - 5).ToArray(); 
            return new Message(isDoctor, messageType, contentLength, contentMessage);
        }

        /// <summary>
        /// This method allows the sender of data to parse it into a byte array that conforms to our network protocol.
        /// This byte array is ready to be sent via TCP
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(this.contentLength);

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
