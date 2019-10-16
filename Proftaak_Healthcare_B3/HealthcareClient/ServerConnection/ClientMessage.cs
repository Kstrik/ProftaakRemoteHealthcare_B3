using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking.HealthCare;

namespace HealthcareClient.BikeConnection
{
    public struct ClientMessage
    {
       
        public byte Distance { get; set; }
        public byte Cadence { get; set; }

        public byte Speed { get; set; }
        public byte Heartbeat { get; set; }

        public byte CheckBits { get; set; }

        public Boolean HasHeartbeat;
        public Boolean HasPage16;
        public Boolean HasPage25;

        public byte[] GetData()
        {
            List<byte> bytes = new List<byte>();
            //byte[] data = new byte[0];
            if(HasHeartbeat)
            {
                bytes.Add((byte)Message.ValueId.HEARTRATE);
                bytes.Add(Heartbeat);
                //data.Append((byte)Message.ValueId.HEARTRATE);
                //data.Append(Heartbeat);
            }
            if(HasPage16)
            {
                bytes.Add((byte)Message.ValueId.SPEED);
                bytes.Add(Speed);
                bytes.Add((byte)Message.ValueId.DISTANCE);
                bytes.Add(Distance);
                //data.Append((byte)Message.ValueId.SPEED);
                //data.Append(Speed);
                //data.Append((byte)Message.ValueId.DISTANCE);
                //data.Append(Distance);
            }
            if(HasPage25)
            {
                bytes.Add((byte)Message.ValueId.CYCLE_RHYTHM);
                bytes.Add(Cadence);
                //data.Append((byte)Message.ValueId.CYCLE_RHYTHM);
                //data.Append(Cadence);
            }
            return bytes.ToArray();
        }
    }
}
