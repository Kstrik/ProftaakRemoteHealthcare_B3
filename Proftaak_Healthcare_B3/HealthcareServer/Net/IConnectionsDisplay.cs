using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Net
{
    public interface IConnectionsDisplay
    {
        void OnClientConnected(string name);
        void OnDoctorConnected(string name);
        void OnClientDisconnected(string name);
        void OnDoctorDisconnected(string name);
    }
}
