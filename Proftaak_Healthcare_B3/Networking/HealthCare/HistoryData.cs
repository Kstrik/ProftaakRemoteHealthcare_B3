using Networking.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.HealthCare
{
    public class HistoryData
    {
        public List<(int heartRate, DateTime time)> HeartrateValues;
        public List<(int distance, DateTime time)> DistanceValues;
        public List<(int speed, DateTime time)> SpeedValues;
        public List<(int cycleRhythm, DateTime time)> CycleRhythmValues;

        public HistoryData()
        {
            this.HeartrateValues = new List<(int heartRate, DateTime time)>();
            this.DistanceValues = new List<(int distance, DateTime time)>();
            this.SpeedValues = new List<(int speed, DateTime time)>();
            this.CycleRhythmValues = new List<(int cycleRhythm, DateTime time)>();
        }

        public void Transmit(ClientConnection connection, string bsn)
        {
            if(connection != null)
            {
                List<byte> startBytes = new List<byte>();
                startBytes.Add((byte)Message.MessageType.CLIENT_HISTORY_START);
                startBytes.Add((byte)bsn.Length);
                startBytes.AddRange(Encoding.UTF8.GetBytes(bsn));
                connection.Transmit(DataEncryptor.Encrypt(new Message(false, Message.MessageType.SERVER_OK, startBytes.ToArray()).GetBytes(), "Test"));

                int maxLength = GetMaxLength();
                for(int i = 0; i < maxLength; i++)
                {
                    List<byte> bytes = new List<byte>();
                    bytes.Add((byte)Message.MessageType.CLIENT_HISTORY_DATA);
                    bytes.Add((byte)bsn.Length);
                    bytes.AddRange(Encoding.UTF8.GetBytes(bsn));

                    if (HeartrateValues.Count - 1 > i)
                    {
                        bytes.Add((byte)Message.ValueId.HEARTRATE);
                        bytes.Add((byte)HeartrateValues[i].heartRate);
                        bytes.AddRange(Encoding.UTF8.GetBytes(HeartrateValues[i].time.ToString()));
                    }

                    if (DistanceValues.Count - 1 > i)
                    {
                        bytes.Add((byte)Message.ValueId.DISTANCE);
                        bytes.Add((byte)DistanceValues[i].distance);
                        bytes.AddRange(Encoding.UTF8.GetBytes(DistanceValues[i].time.ToString()));
                    }

                    if (SpeedValues.Count - 1 > i)
                    {
                        bytes.Add((byte)Message.ValueId.SPEED);
                        bytes.Add((byte)SpeedValues[i].speed);
                        bytes.AddRange(Encoding.UTF8.GetBytes(SpeedValues[i].time.ToString()));
                    }

                    if (CycleRhythmValues.Count - 1 > i)
                    {
                        bytes.Add((byte)Message.ValueId.CYCLE_RHYTHM);
                        bytes.Add((byte)CycleRhythmValues[i].cycleRhythm);
                        bytes.AddRange(Encoding.UTF8.GetBytes(CycleRhythmValues[i].time.ToString()));
                    }

                    connection.Transmit(DataEncryptor.Encrypt(new Message(false, Message.MessageType.SERVER_OK, bytes.ToArray()).GetBytes(), "Test"));
                }

                List<byte> endBytes = new List<byte>();
                endBytes.Add((byte)Message.MessageType.CLIENT_HISTORY_END);
                endBytes.Add((byte)bsn.Length);
                endBytes.AddRange(Encoding.UTF8.GetBytes(bsn));
                connection.Transmit(DataEncryptor.Encrypt(new Message(false, Message.MessageType.SERVER_OK, endBytes.ToArray()).GetBytes(), "Test"));
            }
        }

        private int GetMaxLength()
        {
            return Math.Max(Math.Max(HeartrateValues.Count, DistanceValues.Count), Math.Max(SpeedValues.Count, CycleRhythmValues.Count));
        }
    }
}
