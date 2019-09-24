using HealthcareServer.Vr.Actions;
using HealthcareServer.Vr.World.Components;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.World
{
    public class Scene : IResponseValidator
    {
        private Session session;

        private List<Node> nodes;
        private List<Route> routes;
        private SkyBox skyBox;

        public Scene(Session session)
        {
            this.nodes = new List<Node>();
            this.routes = new List<Route>();
            this.skyBox = new SkyBox(10, session);

            this.session = session;
        }

        public async Task AddNode(Node node)
        {
            if(node != null)
            {
                this.nodes.Add(node);
                await node.Add();
            }
        }

        public async Task AddRoute(Route route)
        {
            if(route != null)
            {
                this.routes.Add(route);
                await route.Add();
            }
        }

        public async Task Reset()
        {
            await this.session.SendAction(GetResetJsonObject());
            this.nodes.Clear();
            this.routes.Clear();
        }

        public async Task<Node> FindNode(string name)
        {
            Response response = await this.session.SendAction(Node.GetFindNodeJsonObject(name, this.session), new ActionRequest("tunnel/send", "scene/node/find", this));
            return (Node)response.Value;
        }

        private JObject GetResetJsonObject()
        {
            JObject resetScene = new JObject();
            resetScene.Add("id", "scene/reset");

            return this.session.GetTunnelSendRequest(resetScene);
        }

        public List<Node> GetNodes()
        {
            return this.nodes;
        }

        public List<Route> GetRoutes()
        {
            return this.routes;
        }

        public SkyBox GetSkyBox()
        {
            return this.skyBox;
        }

        public async Task<Response> GetResponse(string jsonResponse)
        {
            string nodeId = "";
            string nodeName = "";
            string status = "";
            JObject jsonData = ActionRequest.GetActionData(jsonResponse);

            await Task.Run(() =>
            {
                status = jsonData.GetValue("status").ToString();
                if (jsonData.ContainsKey("data"))
                {
                    JArray data = jsonData.GetValue("data").ToObject<JArray>();

                    nodeId = data[0].ToObject<JObject>().GetValue("uuid").ToString();
                    nodeName = data[0].ToObject<JObject>().GetValue("name").ToString();
                }
            });

            Node node = new Node(nodeName, this.session);
            node.Id = nodeId;

            return new Response((!String.IsNullOrEmpty(nodeId) && status.ToLower() == "ok") ? Response.ResponseStatus.SUCCES : Response.ResponseStatus.ERROR, node);
        }
    }
}
