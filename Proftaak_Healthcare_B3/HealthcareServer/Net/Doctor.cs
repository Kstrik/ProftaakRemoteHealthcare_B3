using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Net
{
    public class Doctor
    {
        public string Username;
        public string ClientId;

        public bool IsAuthorized;

        public Doctor(string username, string clientId)
        {
            this.Username = username;
            this.ClientId = clientId;
            this.IsAuthorized = false;
        }
    }
}
