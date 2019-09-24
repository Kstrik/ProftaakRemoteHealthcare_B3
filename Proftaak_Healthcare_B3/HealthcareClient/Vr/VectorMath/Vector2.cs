using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.VectorMath
{
    public class Vector2
    {
        public float X, Y;

        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public JToken GetJsonObject()
        {
            JArray jsonArray = new JArray();
            jsonArray.Add(this.X);
            jsonArray.Add(this.Y);

            return jsonArray;
        }
    }
}
