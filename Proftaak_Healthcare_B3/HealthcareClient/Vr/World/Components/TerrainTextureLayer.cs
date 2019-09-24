using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.World.Components
{
    public class TerrainTextureLayer
    {
        public string NodeId { get; set; }
        private string diffuse;
        private string normal;
        private float minHeight;
        private float maxHeight;
        private float fadeDistance;

        public TerrainTextureLayer(string nodeId, string diffuse, string normal, float minHeight, float maxHeight, float fadeDistance)
        {
            this.NodeId = nodeId;
            this.diffuse = diffuse;
            this.normal = normal;
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
            this.fadeDistance = fadeDistance;
        }

        public JObject GetAddLayerJsonObject()
        {
            JObject data = new JObject();
            data.Add("id", this.NodeId);
            data.Add("diffuse", this.diffuse);
            data.Add("normal", this.normal);
            data.Add("minHeight", this.minHeight);
            data.Add("maxHeight", this.maxHeight);
            data.Add("fadeDist", this.fadeDistance);

            JObject addLayer = new JObject();
            addLayer.Add("id", "scene/node/addlayer");
            addLayer.Add("data", data);

            return addLayer;
        }

        public static JObject GetDeleteLayerJsonObject(string nodeId)
        {
            JObject data = new JObject();
            data.Add("id", nodeId);

            JObject deleteLayer = new JObject();
            deleteLayer.Add("id", "scene/node/dellayer");
            deleteLayer.Add("data", data);

            return deleteLayer;
        }
    }
}
