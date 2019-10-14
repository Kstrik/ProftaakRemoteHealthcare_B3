using HealthcareServer.Files;
using Networking;
using Networking.Client;
using Networking.HealthCare;
using Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthcareServer.Net
{
    public class HealthCareServer : IClientDataReceiver, IServerConnector
    {
        private Server server;

        private List<Cliënt> cliënts;
        private List<Doctor> doctors;

        public HealthCareServer(string ip, int host, ILogger logger)
        {
            this.server = new Server(ip, host, this, this, logger);

            this.cliënts = new List<Cliënt>();
            this.doctors = new List<Doctor>();
        }

        public bool Start()
        {
            return this.server.Start();
        }

        public void Stop()
        {
            this.server.Stop();
        }

        public void OnDataReceived(byte[] data, string clientId)
        {
            string decryptedData = DataEncryptor.Decrypt(Encoding.UTF8.GetString(data), "Test");
            Message message = Message.ParseMessage(Encoding.UTF8.GetBytes(decryptedData));

            switch (message.messageType)
            {
                case Message.MessageType.BIKEDATA:
                    {
                        BroadcastToDoctors(data);
                        ReceiveBikeData(message.Content, clientId);
                        break;
                    }
                case Message.MessageType.CHAT_MESSAGE:
                    {
                        if (this.doctors.Where(d => d.ClientId == clientId).First().IsAuthorized)
                        {
                            List<byte> bytes = new List<byte>(message.Content);
                            string bsn = Encoding.UTF8.GetString(bytes.GetRange(1, bytes[0]).ToArray());
                            byte[] chatMessage = bytes.GetRange(bytes[0] + 1, bytes.Count - (bytes[0] + 1)).ToArray();
                            HandleChatMessage(bsn, chatMessage, clientId);
                        }
                        else
                            this.server.Transmit(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.CHAT_MESSAGE }).GetBytes(), clientId);
                        break;
                    }
                case Message.MessageType.CLIENT_LOGIN:
                    {
                        HandleClientLogin(Encoding.UTF8.GetString(message.Content), clientId);
                        break;
                    }
                case Message.MessageType.DOCTOR_LOGIN:
                    {
                        List<byte> bytes = new List<byte>(message.Content);
                        string username = Encoding.UTF8.GetString(bytes.GetRange(64, bytes.Count - 64).ToArray());
                        string password = Encoding.UTF8.GetString(bytes.GetRange(0, 64).ToArray());
                        HandleDoctorLogin(username, password, clientId);
                        break;
                    }
                case Message.MessageType.CHANGE_RESISTANCE:
                    {
                        if (this.doctors.Where(d => d.ClientId == clientId).First().IsAuthorized)
                        {
                            List<byte> bytes = new List<byte>(message.Content);
                            string bsn = Encoding.UTF8.GetString(bytes.GetRange(1, bytes.Count - 1).ToArray());
                            HandleChangeResistance(bsn, bytes[0], clientId);
                        }
                        else
                            this.server.Transmit(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.CHANGE_RESISTANCE }).GetBytes(), clientId);
                        break;
                    }
                case Message.MessageType.GET_CLIENT_HISTORY:
                    {
                        if (this.doctors.Where(d => d.ClientId == clientId).First().IsAuthorized)
                        {
                            string bsn = Encoding.UTF8.GetString(message.Content);
                            HandleGetClientHistory(bsn, clientId);
                        }
                        else
                            this.server.Transmit(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.GET_CLIENT_HISTORY }).GetBytes(), clientId);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void ReceiveBikeData(byte[] bikeData, string clientId)
        {
            List<byte> bytes = new List<byte>(bikeData);

            for(int i = 0; i < bytes.Count; i += 2)
            {
                Cliënt cliënt = this.cliënts.Where(c => c.ClientId == clientId).First();
                Message.ValueId valueType = (Message.ValueId)bytes[i];
                int value = bytes[i + 1];
                DateTime dateTime = DateTime.Parse(Encoding.UTF8.GetString(bytes.GetRange(i + 2, 19).ToArray()));

                switch(valueType)
                {
                    case Message.ValueId.HEARTRATE:
                        {
                            cliënt.HistoryData.HeartrateValues.Add((heartRate: value, time: dateTime));
                            break;
                        }
                    case Message.ValueId.DISTANCE:
                        {
                            cliënt.HistoryData.DistanceValues.Add((distance: value, time: dateTime));
                            break;
                        }
                    case Message.ValueId.SPEED:
                        {
                            cliënt.HistoryData.SpeedValues.Add((speed: value, time: dateTime));
                            break;
                        }
                    case Message.ValueId.CYCLE_RHYTHM:
                        {
                            cliënt.HistoryData.CycleRhythmValues.Add((cycleRhythm: value, time: dateTime));
                            break;
                        }
                }
            }
        }

        private void HandleClientLogin(string bsn, string clientId)
        {
            if (this.cliënts.Where(c => c.BSN == bsn).Count() == 0)
            {
                if (!Authorizer.ClientExists(bsn))
                    Authorizer.AddClient(bsn);

                this.cliënts.Add(new Cliënt(bsn, clientId));
                this.server.Transmit(new Message(false, Message.MessageType.SERVER_OK, new byte[1] { (byte)Message.MessageType.CLIENT_LOGIN }).GetBytes(), clientId);
            }
            else
                this.server.Transmit(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.CLIENT_LOGIN }).GetBytes(), clientId);
        }

        private void HandleDoctorLogin(string username, string password, string clientId)
        {
            if (this.doctors.Where(c => c.Username == username).Count() == 0)
            {
                if (!Authorizer.CheckDoctorAuthorization(username, password, "Test"))
                {
                    Doctor doctor = new Doctor(username, clientId);
                    this.doctors.Add(doctor);
                    doctor.IsAuthorized = true;
                    this.server.Transmit(new Message(false, Message.MessageType.SERVER_OK, new byte[1] { (byte)Message.MessageType.DOCTOR_LOGIN }).GetBytes(), clientId);
                }
                else
                    this.server.Transmit(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.DOCTOR_LOGIN }).GetBytes(), clientId);
            }
            else
                this.server.Transmit(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.DOCTOR_LOGIN }).GetBytes(), clientId);
        }

        private void HandleChangeResistance(string bsn, byte resistance, string clientId)
        {
            if(this.cliënts.Where(c => c.BSN == bsn).Count() != 0)
            {
                string cliëntId = this.cliënts.Where(c => c.BSN == bsn).First().ClientId;
                this.server.Transmit(new Message(true, Message.MessageType.CHANGE_RESISTANCE, new byte[1] { resistance }).GetBytes(), cliëntId);
            }
            else
                this.server.Transmit(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.CHANGE_RESISTANCE }).GetBytes(), clientId);
        }

        private void HandleChatMessage(string bsn, byte[] chatMessage, string clientId)
        {
            if (this.cliënts.Where(c => c.BSN == bsn).Count() != 0)
            {
                string cliëntId = this.cliënts.Where(c => c.BSN == bsn).First().ClientId;
                this.server.Transmit(new Message(true, Message.MessageType.CHAT_MESSAGE, chatMessage).GetBytes(), cliëntId);
            }
            else
                this.server.Transmit(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.CHAT_MESSAGE }).GetBytes(), clientId);
        }

        private void HandleGetClientHistory(string bsn, string clientId)
        {
            if (Authorizer.ClientExists(bsn))
            {
                HistoryData historyData = null;

                if (this.cliënts.Where(c => c.BSN == bsn).Count() != 0)
                    historyData = this.cliënts.Where(c => c.BSN == bsn).First().HistoryData;
                else
                    historyData = FileHandler.GetHistoryData(bsn, "Test");

                if(historyData != null)
                    historyData.Transmit(this.server.GetConnection(clientId));
                else
                    this.server.Transmit(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.GET_CLIENT_HISTORY }).GetBytes(), clientId);
            }
            else
                this.server.Transmit(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.GET_CLIENT_HISTORY }).GetBytes(), clientId);
        }

        public void OnClientDisconnected(ClientConnection connection)
        { 
            if(this.cliënts.Where(c => c.ClientId == connection.Id).Count() != 0)
                this.cliënts.Remove(this.cliënts.Where(c => c.ClientId == connection.Id).First());
            else if (this.doctors.Where(c => c.ClientId == connection.Id).Count() != 0)
                this.doctors.Remove(this.doctors.Where(c => c.ClientId == connection.Id).First());
        }

        public void OnClientConnected(ClientConnection connection)
        {    

        }

        public void BroadcastToDoctors(byte[] data)
        {
            foreach(Doctor doctor in this.doctors)
            {
                if (doctor.IsAuthorized)
                    this.server.Transmit(data, doctor.ClientId);
            }
        }
    }
}
