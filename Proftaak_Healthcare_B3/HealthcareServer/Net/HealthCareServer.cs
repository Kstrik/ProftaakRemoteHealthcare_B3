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
            byte[] decryptedData = DataEncryptor.Decrypt(data, "Test");
            Message message = Message.ParseMessage(decryptedData);

            switch (message.messageType)
            {
                case Message.MessageType.BIKEDATA:
                    {
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
                            this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.CHAT_MESSAGE })), clientId);
                        break;
                    }
                case Message.MessageType.CLIENT_LOGIN:
                    {
                        BroadcastToDoctors(message.GetBytes());
                        List<byte> bytes = new List<byte>(message.Content);
                        string bsn = Encoding.UTF8.GetString(bytes.GetRange(1, bytes[0]).ToArray());
                        string name = Encoding.UTF8.GetString(bytes.GetRange(bytes[0] + 1, bytes.Count() - (bytes[0] + 1)).ToArray());
                        HandleClientLogin(bsn, name, clientId);
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
                            this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.CHANGE_RESISTANCE })), clientId);
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
                            this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.GET_CLIENT_HISTORY })), clientId);
                        break;
                    }
                case Message.MessageType.GET_CLIENTS:
                    {
                        if (this.doctors.Where(d => d.ClientId == clientId).First().IsAuthorized)
                            HandleGetClients(clientId);
                        else
                            this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.GET_CLIENTS })), clientId);
                        break;
                    }
                case Message.MessageType.START_SESSION:
                    {
                        if (this.doctors.Where(d => d.ClientId == clientId).First().IsAuthorized)
                            HandleStartSession(Encoding.UTF8.GetString(message.Content), clientId);
                        break;
                    }
                case Message.MessageType.STOP_SESSION:
                    {
                        if (this.doctors.Where(d => d.ClientId == clientId).First().IsAuthorized)
                            HandleStopSession(Encoding.UTF8.GetString(message.Content), clientId);
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
            Cliënt cliënt = this.cliënts.Where(c => c.ClientId == clientId).First();

            List<byte> bikeDataBytes = new List<byte>();
            bikeDataBytes.Add((byte)cliënt.BSN.Length);
            bikeDataBytes.AddRange(Encoding.UTF8.GetBytes(cliënt.BSN));
            bikeDataBytes.Add((byte)cliënt.Name.Length);
            bikeDataBytes.AddRange(Encoding.UTF8.GetBytes(cliënt.Name));
            bikeDataBytes.AddRange(bikeData);
            BroadcastToDoctors(new Message(false, Message.MessageType.BIKEDATA, bikeDataBytes.ToArray()).GetBytes());

            List<byte> bytes = new List<byte>(bikeData);

            for (int i = 0; i < bytes.Count; i += 2)
            {
                Message.ValueId valueType = (Message.ValueId)bytes[i];
                int value = bytes[i + 1];
                DateTime dateTime = DateTime.Now;

                switch (valueType)
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

        private void HandleClientLogin(string bsn, string name, string clientId)
        {
            if (this.cliënts.Where(c => c.BSN == bsn).Count() == 0)
            {
                if (!Authorizer.ClientExists(bsn))
                    Authorizer.AddClient(bsn);

                this.cliënts.Add(new Cliënt(bsn, name, clientId));
                this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_OK, new byte[1] { (byte)Message.MessageType.CLIENT_LOGIN })), clientId);
            }
            else
                this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.CLIENT_LOGIN })), clientId);
        }

        private void HandleDoctorLogin(string username, string password, string clientId)
        {
            if (this.doctors.Where(c => c.Username == username).Count() == 0)
            {
                if (Authorizer.CheckDoctorAuthorization(username, password, "Test"))
                {
                    Doctor doctor = new Doctor(username, clientId);
                    this.doctors.Add(doctor);
                    doctor.IsAuthorized = true;
                    this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_OK, new byte[1] { (byte)Message.MessageType.DOCTOR_LOGIN })), clientId);
                }
                else
                    this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.DOCTOR_LOGIN })), clientId);
            }
            else
                this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.DOCTOR_LOGIN })), clientId);
        }

        private void HandleChangeResistance(string bsn, byte resistance, string clientId)
        {
            if(this.cliënts.Where(c => c.BSN == bsn).Count() != 0)
            {
                string cliëntId = this.cliënts.Where(c => c.BSN == bsn).First().ClientId;
                this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.CHANGE_RESISTANCE, new byte[1] { resistance })), cliëntId);
            }
            else
                this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.CHANGE_RESISTANCE })), clientId);
        }

        private void HandleChatMessage(string bsn, byte[] chatMessage, string clientId)
        {
            if (this.cliënts.Where(c => c.BSN == bsn).Count() != 0)
            {
                string cliëntId = this.cliënts.Where(c => c.BSN == bsn).First().ClientId;
                this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.CHAT_MESSAGE, chatMessage)), cliëntId);
            }
            else
                this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.CHAT_MESSAGE })), clientId);
        }

        private void HandleGetClientHistory(string bsn, string clientId)
        {
            if (Authorizer.ClientExists(bsn))
            {
                HistoryData historyData = null;

                historyData = FileHandler.GetHistoryData(bsn, "Test");

                if(historyData != null)
                    historyData.Transmit(this.server.GetConnection(clientId), bsn);
                else
                    this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.GET_CLIENT_HISTORY })), clientId);
            }
            else
                this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.GET_CLIENT_HISTORY })), clientId);
        }

        private void HandleGetClients(string clientId)
        {
            List<string> bsns = FileHandler.GetAllClientBSNS();

            if(bsns.Count() != 0)
            {
                foreach(string bsn in bsns)
                {
                    List<byte> bytes = new List<byte>();
                    bytes.Add((byte)bsn.Length);
                    bytes.AddRange(Encoding.UTF8.GetBytes(bsn));
                    this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.CLIENT_DATA, bytes.ToArray())), clientId);
                }
            }
            else
                this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_ERROR, new byte[1] { (byte)Message.MessageType.GET_CLIENT_HISTORY })), clientId);
        }

        private void HandleStartSession(string bsn, string clientId)
        {
            if (this.cliënts.Where(c => c.BSN == bsn).Count() != 0)
            {
                string cliëntId = this.cliënts.Where(c => c.BSN == bsn).First().ClientId;
                this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.START_SESSION, null)), cliëntId);
            }
            else
            {
                List<byte> bytes = new List<byte>();
                bytes.Add((byte)Message.MessageType.START_SESSION);
                bytes.AddRange(Encoding.UTF8.GetBytes(bsn));
                this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_ERROR, bytes.ToArray())), clientId);
            }
        }

        private void HandleStopSession(string bsn, string clientId)
        {
            if (this.cliënts.Where(c => c.BSN == bsn).Count() != 0)
            {
                string cliëntId = this.cliënts.Where(c => c.BSN == bsn).First().ClientId;
                this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.STOP_SESSION, null)), cliëntId);
            }
            else
            {
                List<byte> bytes = new List<byte>();
                bytes.Add((byte)Message.MessageType.STOP_SESSION);
                bytes.AddRange(Encoding.UTF8.GetBytes(bsn));
                this.server.Transmit(EncryptMessage(new Message(false, Message.MessageType.SERVER_ERROR, bytes.ToArray())), clientId);
            }
        }

        public void OnClientDisconnected(ClientConnection connection)
        { 
            if(this.cliënts.Where(c => c.ClientId == connection.Id).Count() != 0)
            {
                Cliënt cliënt = this.cliënts.Where(c => c.ClientId == connection.Id).First();
                this.cliënts.Remove(cliënt);
                BroadcastToDoctors(new Message(false, Message.MessageType.REMOVE_CLIENT, Encoding.UTF8.GetBytes(cliënt.BSN)).GetBytes());
                FileHandler.SaveHistoryData(cliënt.BSN, cliënt.HistoryData, "Test");
            }
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
                    this.server.Transmit(DataEncryptor.Encrypt(data, "Test"), doctor.ClientId);
            }
        }

        public byte[] EncryptMessage(Message message)
        {
            return DataEncryptor.Encrypt(message.GetBytes(), "Test");
        }
    }
}
