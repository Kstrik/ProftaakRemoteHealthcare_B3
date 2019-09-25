using Networking.HealthCare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareDoctor
{
    class DataManager
    {
        public DataManager()
        {

        }

        public void SendLogin(string username, string password)
        {
            byte[] usernameBytes = Encoding.UTF8.GetBytes(username.PadRight(16));
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password.PadRight(16));
            byte[] messageContent = new byte[32];
            Buffer.BlockCopy(usernameBytes, 0, messageContent, 0, 16);
            Buffer.BlockCopy(passwordBytes, 0, messageContent, 0, 16);
            Message message = new Message(true,(byte)Message.MessageTypes.DOCTOR_LOGIN, messageContent);
        }
    }
}
