using HealthcareServer.Vr.Actions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.World.Components
{
    public class Terrain : IResponseValidator
    {
        private Session session;

        private Bitmap bitmap;
        private int[] heights;

        public string NodeId { get; set; }
        public int Width { get; }
        public int Depth { get; }
        public int MaxHeight { get; set; }

        public bool SmoothNormals { get; }

        private string heightMapFilePath;

        public Terrain(int width, int depth, int maxHeight, string heightMapFilePath, bool smoothNormals, Session session)
        {
            this.Width = width;
            this.Depth = depth;
            this.MaxHeight = maxHeight;

            this.heights = new int[width * depth];
            this.SmoothNormals = smoothNormals;

            this.heightMapFilePath = heightMapFilePath;
            ConvertImageToMap(heightMapFilePath);

            this.session = session;
        }

        private bool ConvertImageToMap(string heightMapFilePath)
        {
            if (File.Exists(heightMapFilePath))
            {
                this.heightMapFilePath = heightMapFilePath;
                this.bitmap = new Bitmap(heightMapFilePath);

                for (int y = 0; y < this.Depth; y++)
                {
                    for (int x = 0; x < this.Width; x++)
                    {
                        int xPos = x * (this.bitmap.Width / this.Width);
                        int yPos = y * (this.bitmap.Height / this.Depth);
                        this.heights[(y * this.Width) + x] = (int)((this.MaxHeight * this.bitmap.GetPixel(xPos, yPos).B) / 255);
                    }
                }
                return true;
            }
            return false;
        }

        public Bitmap GetBitmap()
        {
            return this.bitmap;
        }

        public int[] GetHeights()
        {
            return this.heights;
        }

        public void SetMaxHeight(int maxHeight)
        {
            this.MaxHeight = maxHeight;
            if (ConvertImageToMap(this.heightMapFilePath))
            {
                Task.Run(() => Update());
            }
        }

        public void SetBitmap(string heightMapFilePath)
        {
            if(ConvertImageToMap(heightMapFilePath))
            {
                Task.Run(() => Update());
            }
        }

        public async Task Add()
        {
            await this.session.SendAction(GetAddJsonObject());
        }

        public async Task Update()
        {
            await this.session.SendAction(GetUpdateJsonObject());
        }

        public async Task Delete()
        {
            await this.session.SendAction(GetDeleteJsonObject());
        }

        public async Task AddTextureLayer(string diffuse, string normal, float minHeight, float maxHeight, float fadeDistance)
        {
            TerrainTextureLayer terrainTextureLayer = new TerrainTextureLayer(this.NodeId, diffuse, normal, minHeight, maxHeight, fadeDistance);

            await this.session.SendAction(this.session.GetTunnelSendRequest(terrainTextureLayer.GetAddLayerJsonObject()));
        }

        public async Task RemoveTextureLayer()
        {
            await this.session.SendAction(this.session.GetTunnelSendRequest(TerrainTextureLayer.GetDeleteLayerJsonObject(this.NodeId)));
        }

        public JObject GetAddJsonObject()
        {
            JArray size = new JArray();
            size.Add(this.Width);
            size.Add(this.Depth);

            JObject data = new JObject();
            data.Add("size", size);
            data.Add("heights", JArray.FromObject(this.heights));

            JObject terrainAdd = new JObject();
            terrainAdd.Add("id", "scene/terrain/add");
            terrainAdd.Add("data", data);

            return this.session.GetTunnelSendRequest(terrainAdd);
        }

        public JObject GetUpdateJsonObject()
        {
            JArray size = new JArray();
            size.Add(this.Width);
            size.Add(this.Depth);

            JObject data = new JObject();
            data.Add("size", size);
            data.Add("heights", JArray.FromObject(this.heights));

            JObject terrainUpdate = new JObject();
            terrainUpdate.Add("id", "scene/terrain/update");
            terrainUpdate.Add("data", data);

            return this.session.GetTunnelSendRequest(terrainUpdate);
        }

        public JObject GetDeleteJsonObject()
        {
            JObject data = new JObject();

            JObject terrainDelete = new JObject();
            terrainDelete.Add("id", "scene/terrain/delete");
            terrainDelete.Add("data", data);

            return this.session.GetTunnelSendRequest(terrainDelete);
        }

        public Task<Response> GetResponse(string jsonResponse)
        {
            return null;
        }
    }
}
