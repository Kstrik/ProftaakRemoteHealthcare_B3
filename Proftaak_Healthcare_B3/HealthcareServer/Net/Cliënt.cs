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

        public HistoryData HistoryData;

        public Cliënt(string bsn, string clientId)
        {
            this.BSN = bsn;
            this.ClientId = clientId;
            this.HistoryData = FileHandler.GetHistoryData(bsn, "Test");
        }
    }
}
