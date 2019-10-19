using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking.HealthCare;

namespace HealthcareClient.BikeConnection
{
    struct ClientMessage
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
            byte[] data = new byte[0];
            if(HasHeartbeat)
            {
                data.Append((byte)Message.ValueId.HEARTRATE);
                data.Append(Heartbeat);
            }
            if(HasPage16)
            {
                data.Append((byte)Message.ValueId.SPEED);
                data.Append(Speed);
                data.Append((byte)Message.ValueId.DISTANCE);
                data.Append(Distance);
            }
            if(HasPage25)
            {
                
                data.Append((byte)Message.ValueId._RHYTHM);
                data.Append(Cadence);
            }
            return data;
        }
    }
}
