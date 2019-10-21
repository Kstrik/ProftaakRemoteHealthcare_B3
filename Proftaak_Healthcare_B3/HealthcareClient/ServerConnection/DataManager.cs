#define DEBUG 

using HealthcareClient.Bike;
using HealthcareClient.BikeConnection;
using HealthcareClient.Net;
using Networking;
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
    class DataManager: IBikeDataReceiver, IHeartrateDataReceiver, IClientMessageReceiver
    {
        private ClientMessage clientMessage;
        private HealthCareClient healthcareClient;

        private HeartrateMonitor heartrateMonitor;

        private IClientMessageReceiver receiver;

        public DataManager(HealthCareClient healthcareClient, IClientMessageReceiver receiver) //current observer is datamanager itself, rather than the client window
        {
            this.clientMessage = new ClientMessage();

            this.healthcareClient = healthcareClient;
            this.heartrateMonitor = new HeartrateMonitor(this);

            this.receiver = receiver;
        }

        public void AddPage25(int cadence)
        {
            if (this.clientMessage.HasPage25)
                PushMessage();

            this.clientMessage.Cadence = (byte)cadence;
            this.clientMessage.HasPage25 = true;
        }

        public void AddPage16(int speed, int distance)
        {
            if (this.clientMessage.HasPage16)
                PushMessage();

            this.clientMessage.Distance = distance;
            this.clientMessage.Speed = (byte)(speed * 0.001);
            this.clientMessage.HasPage16 = true;
        }

        public void AddHeartbeat(byte heartbeat)
        {
            if (this.clientMessage.HasHeartbeat)
                PushMessage();
            this.clientMessage.Heartbeat = heartbeat;
            this.clientMessage.HasHeartbeat = true;
        }

        private void PushMessage()
        {
#if DEBUG
            Console.WriteLine("Pushing message");
#endif
            this.receiver?.HandleClientMessage(this.clientMessage);
            //HandleClientMessage(this.clientMessage);
            this.clientMessage = new ClientMessage();
            this.clientMessage.HasHeartbeat = false;
            this.clientMessage.HasPage16 = false;
            this.clientMessage.HasPage25 = false;
        }

        //Upon receiving data from the bike and Heartbeat Sensor, try to place in a Struct. 
        //Once struct is full or data would be overwritten, it is sent to the server
        public void ReceiveBikeData(byte[] data, Bike.Bike bike)
        {
            Dictionary<string, int> translatedData = TacxTranslator.Translate(BitConverter.ToString(data).Split('-'));
            int PageID;
            translatedData.TryGetValue("PageID", out PageID); //hier moet ik van overgeven maar het kan niet anders
            if (25 == PageID)
            {    
                int cadence;
                translatedData.TryGetValue("InstantaneousCadence", out cadence);
                AddPage25(cadence);
            }
            else if (16 == PageID)
            {
                int speed;
                translatedData.TryGetValue("speed", out speed);
                int distance;
                translatedData.TryGetValue("distance", out distance);
                AddPage16(speed, distance);
            }
        }

        /// <summary>
        /// Parses a complete ClientMessage into a packet to be sent via TCP
        /// </summary>
        public void HandleClientMessage(ClientMessage clientMessage)
        {
            Message toSend = new Message(false, Message.MessageType.BIKEDATA, clientMessage.GetData());
            this.healthcareClient.Transmit(toSend);
        }

        public void ReceiveHeartrateData(byte heartrate)
        {
            AddHeartbeat(heartrate);
        }
    }
}
