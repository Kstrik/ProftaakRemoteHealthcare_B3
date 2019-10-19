using HealthcareServer.Vr.Actions;
using HealthcareServer.Vr.VectorMath;
using HealthcareServer.Vr.World.Components;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.World
{
    public class Route : IResponseValidator
    {
        public class RouteNode
        {
            public Vector3 Position { get; set; }
            public Vector3 Direction { get; set; }

            public RouteNode(Vector3 position, Vector3 direction)
            {
                this.Position = position;
                this.Direction = direction;
            }
        }

        private Session session;

        public string Id { get; set; }
        public string Name { get; set; }
        private List<RouteNode> routeNodes;

        public Road Road { get; set; }

        public Route(Session session)
        {
            this.routeNodes = new List<RouteNode>();
            this.session = session;
        }

        public Route(List<RouteNode> routeNodes, Session session)
            : this(session)
        {
            this.routeNodes = routeNodes;
        }

        public void AddRouteNode(RouteNode routeNode)
        {
            if (routeNode != null)
                this.routeNodes.Add(routeNode);
        }

        public void SetRoad(Road road)
        {
            if (road != null)
            {
                this.Road = road;
                //await this.Road.Add();
            }
        }

        public async Task Add()
        {
            Response response = await this.session.SendAction(GetAddJsonObject(), new ActionRequest("tunnel/send", "route/add", this));
            this.Id = (response.Status == Response.ResponseStatus.SUCCES) ? (string)response.Value : "";

            if (this.Road != null)
                await Task.Run(() => this.Road.Add());
        }

        public async Task Update()
        {
            await this.session.SendAction(GetUpdateJsonObject());

            if (this.Road != null)
                await Task.Run(() => this.Road.Update());
        }

        private JObject GetAddJsonObject()
        {
            JArray nodes = new JArray();

            foreach (RouteNode routeNode in this.routeNodes)
            {
                JObject node = new JObject();
                node.Add("pos", routeNode.Position.GetJsonObject());
                node.Add("dir", routeNode.Direction.GetJsonObject());

                nodes.Add(node);
            }

            JObject data = new JObject();
            data.Add("nodes", nodes);

            JObject routeAdd = new JObject();
            routeAdd.Add("id", "route/add");
            routeAdd.Add("data", data);

            string test = routeAdd.ToString();

            return this.session.GetTunnelSendRequest(routeAdd);
        }

        private JObject GetUpdateJsonObject()
        {
            JArray nodes = new JArray();

            int index = 0;
            foreach (RouteNode routeNode in this.routeNodes)
            {
                JObject node = new JObject();
                node.Add("index", index);
                node.Add("pos", routeNode.Position.GetJsonObject());
                node.Add("dir", routeNode.Direction.GetJsonObject());

                nodes.Add(node);
                index++;
            }

            JObject data = new JObject();
            data.Add("id", this.Id);
            data.Add("nodes", nodes);

            JObject routeAdd = new JObject();
            routeAdd.Add("id", "route/update");
            routeAdd.Add("data", data);

            string test = routeAdd.ToString();

            return this.session.GetTunnelSendRequest(routeAdd);
        }

        public async Task<Response> GetResponse(string jsonResponse)
        {
            string routeId = "";
            string status = "";
            JObject jsonData = ActionRequest.GetActionData(jsonResponse);

            await Task.Run(() =>
            {
                status = jsonData.GetValue("status").ToString();
                if (jsonData.ContainsKey("data"))
                {
                    JObject data = jsonData.GetValue("data").ToObject<JObject>();

                    routeId = data.GetValue("uuid").ToString();
                }
            });

            //Documentatie geeft een verkeerde plaatst aan van het status veld of dit is mogelijk een fout op de VR server

            return new Response((!String.IsNullOrEmpty(routeId) && status.ToLower() == "ok") ? Response.ResponseStatus.SUCCES : Response.ResponseStatus.ERROR, routeId);
        }

        public List<RouteNode> GetRouteNodes()
        {
            return this.routeNodes;
        }
    }
}
