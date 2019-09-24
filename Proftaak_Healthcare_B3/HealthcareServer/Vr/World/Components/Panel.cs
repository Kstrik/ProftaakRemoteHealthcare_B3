using HealthcareServer.Vr.VectorMath;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.World.Components
{
    public class Panel
    {
        private Session session;
        public string NodeId { get; set; }
        public Vector2 Size { get; }
        public Vector2 Resolution { get; }
        public Vector4 Background { get; }
        public bool CastShadow { get; }

        public Panel(Vector2 size, Vector2 resolution, Vector4 background, bool castShadow, Session session)
        {
            this.Size = size;
            this.Resolution = resolution;
            this.Background = background;
            this.CastShadow = castShadow;
            this.session = session;
        }

        public async Task DrawText(string text, Vector2 position, float size, Vector4 color, string font)
        {
            await this.session.SendAction(GetDrawTextJsonObject(text, position, size, color, font));
        }

        public async Task DrawLines(int strokeWidth, List<PanelLine> lines)
        {
            await this.session.SendAction(GetDrawLinesJsonObject(strokeWidth, lines));
        }

        public async Task SetClearColor(Vector4 color)
        {
            await this.session.SendAction(GetSetClearColorJsonObject(color));
        }

        public async Task Swap()
        {
            await this.session.SendAction(GetSwapJsonObject());
        }

        public async Task Clear()
        {
            await this.session.SendAction(GetClearJsonObject());
        }

        private JObject GetDrawLinesJsonObject(int strokeWidth, List<PanelLine> lines)
        {
            JObject data = new JObject();
            data.Add("id", this.NodeId);
            data.Add("width", strokeWidth);
            data.Add("lines", GetLinesArrayFromList(lines));

            JObject drawLines = new JObject();
            drawLines.Add("id", "scene/panel/drawlines");
            drawLines.Add("data", data);

            return this.session.GetTunnelSendRequest(drawLines);
        }

        private JObject GetDrawTextJsonObject(string text, Vector2 position, float size, Vector4 color, string font)
        {
            JObject data = new JObject();
            data.Add("id", this.NodeId);
            data.Add("text", text);
            data.Add("position", position.GetJsonObject());
            data.Add("size", size);
            data.Add("color", color.GetJsonObject());
            data.Add("font", font);

            JObject drawText = new JObject();
            drawText.Add("id", "scene/panel/drawtext");
            drawText.Add("data", data);

            return this.session.GetTunnelSendRequest(drawText);
        }

        private JObject GetSetClearColorJsonObject(Vector4 color)
        {
            JObject data = new JObject();
            data.Add("id", this.NodeId);
            data.Add("color", color.GetJsonObject());

            JObject setClearColor = new JObject();
            setClearColor.Add("id", "scene/panel/setclearcolor");
            setClearColor.Add("data", data);

            return this.session.GetTunnelSendRequest(setClearColor);
        }

        private JObject GetSwapJsonObject()
        {
            JObject data = new JObject();
            data.Add("id", this.NodeId);

            JObject swap = new JObject();
            swap.Add("id", "scene/panel/swap");
            swap.Add("data", data);

            return this.session.GetTunnelSendRequest(swap);
        }

        private JObject GetClearJsonObject()
        {
            JObject data = new JObject();
            data.Add("id", this.NodeId);

            JObject clear = new JObject();
            clear.Add("id", "scene/panel/clear");
            clear.Add("data", data);

            return this.session.GetTunnelSendRequest(clear);
        }

        private JArray GetLinesArrayFromList(List<PanelLine> lines)
        {
            JArray linesJson = new JArray();

            foreach(PanelLine line in lines)
            {
                linesJson.Add(line.GetJsonObject());
            }

            return linesJson;
        }
    }
}
