using HealthcareServer.Vr.VectorMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareClient.SceneManagement.ModelLoading
{
    public class Model
    {
        public string Name;
        public List<Vector3> Vertices;
        public List<Vector2> UVCoordinates;
        public List<Vector3> Normals;
        public List<int> Indices;

        public Model()
        {
            this.Vertices = new List<Vector3>();
            this.UVCoordinates = new List<Vector2>();
            this.Normals = new List<Vector3>();
            this.Indices = new List<int>();
        }
    }
}
