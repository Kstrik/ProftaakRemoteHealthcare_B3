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

        public Boolean hasPage25;
        public Boolean hasPage16;
        public Boolean hasHeartbeat;

        public byte[] getData()
        {
            byte[] data = new byte[0];
            if(hasHeartbeat)
            {
                data.Append((byte)Message.ValueId.HEARTRATE);
                data.Append(Heartbeat);
            }
            if(hasPage16)
            {
                data.Append((byte)Message.ValueId.SPEED);
                data.Append(Speed);
                data.Append((byte)Message.ValueId.DISTANCE);
                data.Append(Distance);
            }
            if(hasPage25)
            {
                
                data.Append((byte)Message.ValueId.CYCLE_RHYTHM);
                data.Append(Cadence);
            }
            return data;
        }
    }

    

}
