using HealthcareServer.Vr.Actions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthcareServer.Vr
{
    public class Tunnel : IResponseValidator
    {
        private Session session;
        public string Id;

        public Tunnel(Session session)
        {
            this.session = session;
        }

        public async Task Create(string sessionId, string key)
        {
            Response response = await Task.Run(() => this.session.SendAction(RequestTunnel(sessionId, key), new ActionRequest("tunnel/create", "", this)));
            this.Id = (response.Status == Response.ResponseStatus.SUCCES) ? (string)response.Value : "";
        }

        private JObject RequestTunnel(string sessionId, string key)
        {
            JObject data = new JObject();
            data.Add("session", sessionId);
            data.Add("key", key);

            JObject tunnelCreate = new JObject();
            tunnelCreate.Add("id", "tunnel/create");
            tunnelCreate.Add("data", data);

            return tunnelCreate;
        }

        public async Task<Response> GetResponse(string jsonResponse)
        {
            string tunnelId = "";
            string status = "";
            JObject jsonData = JObject.Parse(jsonResponse);

            await Task.Run(() =>
            {
                if (jsonData.ContainsKey("data"))
                {
                    JObject data = jsonData.GetValue("data").ToObject<JObject>();

                    status = data.GetValue("status").ToString();

                    if (data.ContainsKey("id"))
                        tunnelId = data.GetValue("id").ToString();
                }
            });

            return new Response((!String.IsNullOrEmpty(status) && status.ToLower() == "ok") ? Response.ResponseStatus.SUCCES : Response.ResponseStatus.ERROR, tunnelId);
        }
    }
}
