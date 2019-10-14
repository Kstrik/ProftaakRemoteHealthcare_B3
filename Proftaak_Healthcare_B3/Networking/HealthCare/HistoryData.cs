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

        public void Transmit(ClientConnection connection)
        {
            if(connection != null)
            {
                connection.Transmit(new Message(true, Message.MessageType.CLIENT_HISTORY_START, null).GetBytes());

                int maxLength = GetMaxLength();
                for(int i = 0; i < maxLength; i++)
                {
                    List<byte> bytes = new List<byte>();

                    if (HeartrateValues.Count - 1 > i)
                    {
                        bytes.Add((byte)Message.ValueId.HEARTRATE);
                        bytes.Add((byte)HeartrateValues[i].heartRate);
                        bytes.AddRange(bytes.GetRange(1, 19).ToArray());
                    }

                    if (DistanceValues.Count - 1 > i)
                    {
                        bytes.Add((byte)Message.ValueId.DISTANCE);
                        bytes.Add((byte)DistanceValues[i].distance);
                        bytes.AddRange(bytes.GetRange(1, 19).ToArray());
                    }

                    if (SpeedValues.Count - 1 > i)
                    {
                        bytes.Add((byte)Message.ValueId.SPEED);
                        bytes.Add((byte)SpeedValues[i].speed);
                        bytes.AddRange(bytes.GetRange(1, 19).ToArray());
                    }

                    if (CycleRhythmValues.Count - 1 > i)
                    {
                        bytes.Add((byte)Message.ValueId.CYCLE_RHYTHM);
                        bytes.Add((byte)CycleRhythmValues[i].cycleRhythm);
                        bytes.AddRange(bytes.GetRange(1, 19).ToArray());
                    }

                    connection.Transmit(new Message(true, Message.MessageType.CLIENT_HISTORY_DATA, bytes.ToArray()).GetBytes());
                }

                connection.Transmit(new Message(true, Message.MessageType.CLIENT_HISTORY_END, null).GetBytes());
            }
        }

        private int GetMaxLength()
        {
            return Math.Max(Math.Max(HeartrateValues.Count, DistanceValues.Count), Math.Max(SpeedValues.Count, CycleRhythmValues.Count));
        }
    }
}
