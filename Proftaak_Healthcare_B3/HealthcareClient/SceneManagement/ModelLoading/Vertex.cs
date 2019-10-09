using HealthcareServer.Vr.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareClient.SceneManagement.ModelLoading
{
    public class Vertex
    {
        public Vector3 Vertice;
        public Vector2 UVCoordinate;
        public Vector3 Normal;

        public Vertex(Vector3 vertice, Vector2 uvCoordinate, Vector3 normal)
        {
            this.Vertice = vertice;
            this.UVCoordinate = uvCoordinate;
            this.Normal = normal;
        }
    }
}
