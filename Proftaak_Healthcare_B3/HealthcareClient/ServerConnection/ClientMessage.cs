using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public byte[] toByteArray()
        {
            byte[] message = new byte[1];
            message[0] = 0b00000001; //code for client
            if(hasHeartbeat)
            {
                message.Append((byte)0b0000001);
                message.Append(Heartbeat);
            }
            if(hasPage16)
            {
                message.Append((byte)0b00000010);
                message.Append(Speed);
            }
            if(hasPage25)
            {
                message.Append((byte)0b00000011);
               
                message.Append(Distance);
                message.Append((byte)0b00000100);
                message.Append(Cadence);
            }
            return message;
        }
    }

    

}
