using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer
{
    
    struct Patient
    {
        int BSN { get; }
        string name { get; }

        public Patient(int BSN, string name)
        {
            this.BSN = BSN;
            this.name = name;
        }
    }
}
