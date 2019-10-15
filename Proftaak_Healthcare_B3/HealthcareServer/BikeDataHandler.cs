using Networking.HealthCare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer
{
    class BikeDataHandler : IMessageHandler
    {

        public void HandleMessage(Byte[] data)
        {

            StringBuilder bikeData = new StringBuilder();

            int skip = 0;
            for (int i = 0; i < data.Length; i += skip)
            {
                byte valueId = data[i];
                byte lastvalue = data[i + 1];

                switch ((Message.ValueId)valueId)
                {
                    case Message.ValueId.HEARTRATE:
                        {
                            skip = 2;
                            bikeData.Append( $"Heartrate: {lastvalue}\r\n" );
                            break;
                        }
                    case Message.ValueId.DISTANCE:
                        {
                            skip = 2;
                            bikeData.Append( $"Power: {lastvalue}\r\n" );
                            break;
                        }
                    case Message.ValueId.SPEED:
                        {
                            skip = 2;
                            bikeData.Append( $"Speed: {lastvalue}\r\n" );
                            break;
                        }
                    case Message.ValueId.CYCLE_RHYTHM:
                        {
                            skip = 2;
                            bikeData.Append( $"Cycle rithm: {lastvalue}\r\n" );
                            break;
                        }
                }
            }

            WriteData(@"D:\Avans\jaar 2\Periode 1\proftaak\Code\ProftaakRemoteHealthcare_B3\Proftaak_Healthcare_B3\HealthcareServer\Data\Users\test.txt", bikeData.ToString() );

        }


        public void ReadData(string pathName, string data)
        {
            
        }


        public void WriteData(string pathName, string data)
        {

            System.IO.File.WriteAllText(pathName, data);

        }


    }
}
