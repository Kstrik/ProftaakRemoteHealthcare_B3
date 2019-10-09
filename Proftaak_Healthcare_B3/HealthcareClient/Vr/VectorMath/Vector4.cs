using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.VectorMath
{
    public class Vector4
    {
        public float X, Y, Z, W;

        public Vector4(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public Vector4 Clone()
        {
            return new Vector4(this.X, this.Y, this.Z, this.W);
        }

        public JToken GetJsonObject()
        {
            JArray jsonArray = new JArray();
            jsonArray.Add(this.X);
            jsonArray.Add(this.Y);
            jsonArray.Add(this.Z);
            jsonArray.Add(this.W);

            return jsonArray;
        }
    }
}
