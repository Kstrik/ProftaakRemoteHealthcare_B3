using HealthcareServer.Vr.VectorMath;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.World.Components
{
    public class PanelLine
    {
        public Vector2 Point1 { get; set; }
        public Vector2 Point2 { get; set; }
        public Vector4 Color { get; set; }

        public PanelLine(Vector2 point1, Vector2 point2, Vector4 color)
        {
            this.Point1 = point1;
            this.Point2 = point2;
            this.Color = color;
        }

        public JToken GetJsonObject()
        {
            JArray line = new JArray();
            line.Add(this.Point1.X);
            line.Add(this.Point1.Y);
            line.Add(this.Point2.X);
            line.Add(this.Point2.Y);
            line.Add(this.Color.X);
            line.Add(this.Color.Y);
            line.Add(this.Color.Z);
            line.Add(this.Color.W);

            return line.ToObject<JToken>();
        }
    }
}
