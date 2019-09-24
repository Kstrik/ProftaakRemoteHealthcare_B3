using HealthcareServer.Vr.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HealthcareServer.Vr.World.Components
{
    public class Transform
    {
        public Vector3 Position { get; }
        public float Scale { get; }
        public Vector3 Rotation { get; }

        public Transform(Vector3 position, float scale, Vector3 rotation)
        {
            this.Position = position;
            this.Scale = scale;
            this.Rotation = rotation;
        }

        public JObject GetJsonObject()
        {
            JObject transform = new JObject();
            transform.Add("position", this.Position.GetJsonObject());
            transform.Add("scale", this.Scale);
            transform.Add("rotation", this.Rotation.GetJsonObject());

            return transform;
        }
    }
}
