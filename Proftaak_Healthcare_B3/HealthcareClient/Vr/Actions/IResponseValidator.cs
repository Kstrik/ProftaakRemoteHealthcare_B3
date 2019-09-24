using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.Actions
{
    public interface IResponseValidator
    {
        Task<Response> GetResponse(string jsonResponse);
    }
}
