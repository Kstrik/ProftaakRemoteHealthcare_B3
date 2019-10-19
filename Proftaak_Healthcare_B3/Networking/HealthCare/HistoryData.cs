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

        }

        public void Transmit(ClientConnection connection, string bsn)
        {
            if(connection != null)
            {
                connection.Transmit(DataEncryptor.Encrypt(new Message(true, Message.MessageType.CLIENT_HISTORY_START, Encoding.UTF8.GetBytes(bsn)).GetBytes(), "Test"));

                int maxLength = GetMaxLength();
                for(int i = 0; i < maxLength; i++)
                {
                    List<byte> bytes = new List<byte>();
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
                        bytes.Add((byte)Message.ValueId._RHYTHM);
                        bytes.Add((byte)CycleRhythmValues[i].cycleRhythm);
                        bytes.AddRange(Encoding.UTF8.GetBytes(CycleRhythmValues[i].time.ToString()));
                    }

                    connection.Transmit(DataEncryptor.Encrypt(new Message(true, Message.MessageType.CLIENT_HISTORY_DATA, bytes.ToArray()).GetBytes(), "Test"));
                }

                connection.Transmit(DataEncryptor.Encrypt(new Message(true, Message.MessageType.CLIENT_HISTORY_END, Encoding.UTF8.GetBytes(bsn)).GetBytes(), "Test"));
            }
        }

        private int GetMaxLength()
        {
            return Math.Max(Math.Max(HeartrateValues.Count, DistanceValues.Count), Math.Max(SpeedValues.Count, CycleRhythmValues.Count));
        }
    }
}
