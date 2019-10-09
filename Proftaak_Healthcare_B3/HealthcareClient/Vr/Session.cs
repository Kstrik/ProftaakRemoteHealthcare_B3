using HealthcareServer.Vr.Actions;
using HealthcareServer.Vr.World;
using Networking.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HealthcareServer.Vr
{
    public class Session : IResponseValidator
    {
        public List<ActionRequest> ActionRequests;
        public Dictionary<ActionRequest, string> ActionsCache;

        private Tunnel tunnel;
        private Scene scene;

        private Client client;

        public string Id;
        private string key;

        private string hostname;

        public Session(ref Client client)
        {
            this.ActionRequests = new List<ActionRequest>();
            this.ActionsCache = new Dictionary<ActionRequest, string>();

            this.tunnel = new Tunnel(this);
            this.scene = new Scene(this);

            this.client = client;
        }

        public async Task Create(string hostname, string key)
        {
            if (!String.IsNullOrEmpty(hostname))
            {
                this.hostname = hostname;
                this.key = key;
                await Task.Run(() => RequestSession());
                await this.tunnel.Create(this.Id, this.key);
            }
        }

        public void Destroy()
        {

        }

        private async Task RequestSession()
        {
            Response response = await Task.Run(() => SendAction(GetSessionsListRequest(), new ActionRequest("session/list", "", this)));
            this.Id = (response.Status == Response.ResponseStatus.SUCCES) ? (string)response.Value : "";
        }

        public async Task<Response> SendAction(JObject action)
        {
            return await SendAction(action, null);
        }

        public async Task<Response> SendAction(JObject action, ActionRequest actionRequest)
        {
            if (actionRequest != null)
                this.ActionRequests.Add(actionRequest);

            this.client.Transmit(Encoding.UTF8.GetBytes(action.ToString()));

            string jsonResponse = "";

            if (actionRequest != null)
            {
                while (true)
                {
                    await Task.Delay(200);
                    if (this.ActionsCache.Keys.Where(a => a.Equals(actionRequest)).Count() > 0)
                    {
                        this.ActionRequests.Remove(actionRequest);

                        ActionRequest key = this.ActionsCache.Where(m => m.Key.Equals(actionRequest)).First().Key;
                        this.ActionsCache.TryGetValue(key, out jsonResponse);
                        this.ActionsCache.Remove(key);
                        break;
                    }
                }

                if (actionRequest.Validator != null)
                    return await actionRequest.Validator.GetResponse(jsonResponse);
            }

            return new Response(Response.ResponseStatus.ERROR, null);
        }

        public void OnDataReceived(byte[] data)
        {
            try
            {
                string jsonString = Encoding.UTF8.GetString(data);
                JObject jsonActionResponse = JObject.Parse(jsonString);

                string tunnelAction = jsonActionResponse.GetValue("id").ToString();
                string vrAction = "";

                if (jsonActionResponse.ContainsKey("data"))
                {
                    if (jsonActionResponse.GetValue("data").GetType() != typeof(JArray))
                    {
                        JObject tunnelData = jsonActionResponse.GetValue("data").ToObject<JObject>();
                        if (tunnelData.ContainsKey("data"))
                        {
                            JObject actionData = tunnelData.GetValue("data").ToObject<JObject>();
                            if (actionData.ContainsKey("id"))
                            {
                                vrAction = actionData.GetValue("id").ToString();
                            }
                        }
                    }
                }

                ActionRequest actionRequest = new ActionRequest(tunnelAction, vrAction);
                if (this.ActionRequests.Contains(actionRequest))
                    this.ActionsCache.Add(actionRequest, jsonString);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error!");
            }
        }

        public static JObject GetSessionsListRequest()
        {
            JObject request = new JObject();
            request.Add("id", "session/list");

            return request;
        }

        public JObject GetTunnelSendRequest(JObject action)
        {
            JObject tunnelData = new JObject();
            tunnelData.Add("dest", this.tunnel.GetId());
            tunnelData.Add("data", action);

            JObject request = new JObject();
            request.Add("id", "tunnel/send");
            request.Add("data", tunnelData);

            return request;
        }

        #region Reponse Handling

        public async Task<Response> GetResponse(string jsonResponse)
        {
            string sessionId = "";
            JObject jsonData = JObject.Parse(jsonResponse);

            await Task.Run(() =>
            {
                foreach (JObject session in jsonData.GetValue("data").ToObject<JToken>().Children())
                {
                    JObject clientInfo = session.GetValue("clientinfo").ToObject<JObject>();

                    if (clientInfo.GetValue("host").ToString() == this.hostname)
                    {
                        sessionId = session.GetValue("id").ToString();
                        break;
                    }
                }
            });

            return new Response((!String.IsNullOrEmpty(sessionId)) ? Response.ResponseStatus.SUCCES : Response.ResponseStatus.ERROR, sessionId);
        }

        #endregion

        public string GetId()
        {
            return this.Id;
        }

        public Scene GetScene()
        {
            return this.scene;
        }
    }
}
