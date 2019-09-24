using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HealthcareServer.Vr.VectorMath
{
    public class Vector3
    {
        public float X, Y, Z;

        public Vector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public JToken GetJsonObject()
        {
            JArray jsonArray = new JArray();
            jsonArray.Add(this.X);
            jsonArray.Add(this.Y);
            jsonArray.Add(this.Z);

            return jsonArray;
        }
    }
}
