using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareClient.BikeConnection
{
    struct ClientMessage
    {
       
        public byte[] power { get; set; }
        public byte cadence { get; set; }

        public byte speed { get; set; }
        public byte heartbeat { get; set; }

        public byte checkBits { get; set; }

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
                message.Append(heartbeat);
            }
            if(hasPage16)
            {
                message.Append((byte)0b00000010);
                message.Append(speed);
            }
            if(hasPage25)
            {
                message.Append((byte)0b00000011);
                message.Append(power[0]);
                message.Append(power[1]);
                message.Append((byte)0b00000100);
                message.Append(cadence);
            }
            return message;
        }
    }

    

}
