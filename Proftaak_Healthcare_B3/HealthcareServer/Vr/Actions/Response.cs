using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.Actions
{
    /// <summary>
    /// Represents response from the VR engine
    /// </summary>
    public class Response
    {
        public enum ResponseStatus
        {
            SUCCES, ERROR
        }

        public ResponseStatus Status { get; }
        public object Value { get; }

        public Response(ResponseStatus status, object value)
        {
            this.Status = status;
            this.Value = value;
        }
    }
}
