#define DEBUG 

using HealthcareClient.Bike;
using HealthcareClient.BikeConnection;
using Networking.Client;
using Networking.HealthCare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareClient.ServerConnection
{
    /// <summary>
    /// This class keeps track of data from bikes and Heartbeat Monitors. It's purpose is to make sure no data is lost
    /// 
    /// </summary>
    class DataManager: IServerDataReceiver, IBikeDataReceiver, IHeartrateDataReceiver, IClientMessageReceiver
    {
        private ClientMessage clientMessage;
        private Client DataServerClient;

        private IClientMessageReceiver observer;

        [Flags] public enum CheckBits { SESSIE = 0b0001000, BIKE_ERROR = 0b0000100, HEARTRATE_ERROR = 0b00000010, VRERROR = 0b00000001 };

        public DataManager(IClientMessageReceiver observer) //current observer is datamanager itself, rather than the client window
        {
            this.observer = this;
            DataServerClient = new Client("localhost", 80, this, null);
        }

        public void AddPage25(int cadence)
        {
            if (clientMessage.HasPage25)
                PushMessage();
            
            byte[] cadenceBytes = BitConverter.GetBytes(cadence);

            clientMessage.Cadence = cadenceBytes[3];
            clientMessage.HasPage25 = true;
        }

        public void AddPage16(int speed, int distance)
        {
            if (clientMessage.HasPage16)
                PushMessage();
            byte[] distanceBytes = BitConverter.GetBytes(distance);
            byte[] speedBytes = BitConverter.GetBytes(speed);

            clientMessage.Distance = distanceBytes[3];
            clientMessage.Speed = speedBytes[3];
            clientMessage.HasPage16 = true;
        }

        public void AddHeartbeat(byte heartbeat)
        {
            if (clientMessage.HasHeartbeat)
                PushMessage();
            clientMessage.Heartbeat = heartbeat;
            clientMessage.HasHeartbeat = true;
        }

        private void PushMessage()
        {
#if DEBUG
            Console.WriteLine("Pushing message");
#endif
            observer.handleClientMessage(clientMessage);
            clientMessage = new ClientMessage();
            clientMessage.HasHeartbeat = false;
            clientMessage.HasPage16 = false;
            clientMessage.HasPage25 = false;
        }

        //Upon receiving data from the bike and Heartbeat Sensor, try to place in a Struct. 
        //Once struct is full or data would be overwritten, it is sent to the server
        void IBikeDataReceiver.ReceiveBikeData(byte[] data, Bike.Bike bike)
        {
            Dictionary<string, int> translatedData = TacxTranslator.Translate(BitConverter.ToString(data).Split('-'));
            int PageID;
            translatedData.TryGetValue("PageID", out PageID); //hier moet ik van overgeven maar het kan niet anders
            if (25 == PageID)
            {
                
                int cadence; translatedData.TryGetValue("InstantaneousCadence", out cadence);
                AddPage25(cadence);
            }
            else if (16 == PageID)
            {
                int speed; translatedData.TryGetValue("speed", out speed);
                int distance; translatedData.TryGetValue("distance", out distance);
                AddPage16(speed, distance);
            }
        }

        /// <summary>
        /// Parses a complete ClientMessage into a packet to be sent via TCP
        /// </summary>
        void IClientMessageReceiver.handleClientMessage(ClientMessage clientMessage)
        {
            byte Checkbits = (byte)CheckBits.HEARTRATE_ERROR; //heartbeat not implemented yet
            byte[] message = clientMessage.GetData();
            message.Append(Checkbits);
            Message toSend = new Message(false, Message.MessageType.BIKEDATA, message);
            Send(toSend.GetBytes());
        }

        public void ReceiveHeartrateData(byte heartrate, HeartrateMonitor heartrateMonitor)
        {
            AddHeartbeat(heartrate);
        }

        private void Send(byte[] message)
        {
            DataServerClient.Transmit(message);
        }

        public void OnDataReceived(byte[] data)
        {
#if DEBUG
            Console.WriteLine("Received response from data server - response not handled");
#endif
            throw new NotImplementedException();

        }
    }
}
