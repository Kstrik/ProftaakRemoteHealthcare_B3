using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.Actions
{
    public class ActionRequest : IEquatable<ActionRequest>
    {
        public string TunnelAction { get; }
        public string VRAction { get; }
        public IResponseValidator Validator { get; }

        public ActionRequest(string tunnelAction, string vrAction = "", IResponseValidator validator = null)
        {
            this.TunnelAction = tunnelAction;
            this.VRAction = vrAction;
            this.Validator = validator;
        }

        public static JObject GetActionData(string jsonTunnelData)
        {
            JObject jsonData = JObject.Parse(jsonTunnelData);
            JObject actionData = new JObject();

            if(jsonData.ContainsKey("data"))
            {
                JObject tunnelData = jsonData.GetValue("data").ToObject<JObject>();
                if(tunnelData.ContainsKey("data"))
                {
                    return tunnelData.GetValue("data").ToObject<JObject>();
                }
            }

            return actionData;
        }

        public bool Equals(ActionRequest other)
        {
            return (this.TunnelAction == other.TunnelAction && this.VRAction == other.VRAction);
        }
    }
}
