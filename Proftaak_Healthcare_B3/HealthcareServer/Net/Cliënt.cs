using HealthcareServer.Files;
using Networking.HealthCare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Net
{
    public class Cliënt
    {
        public string BSN;
        public string ClientId;
        public string Name;

        public HistoryData HistoryData;

        public Cliënt(string bsn, string name, string clientId)
        {
            this.BSN = bsn;
            this.Name = name;
            this.ClientId = clientId;
            this.HistoryData = new HistoryData();
        }
    }
}
