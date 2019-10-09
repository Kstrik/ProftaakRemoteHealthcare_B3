using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareClient.SceneManagement.ModelLoading
{
    public class TriangleFace
    {
        public Vertex Vertex1;
        public Vertex Vertex2;
        public Vertex Vertex3;

        public TriangleFace(Vertex vertex1, Vertex vertex2, Vertex vertex3)
        {
            this.Vertex1 = vertex1;
            this.Vertex2 = vertex2;
            this.Vertex3 = vertex3;
        }

        public Vertex[] ToArray()
        {
            return new Vertex[3] { this.Vertex1, this.Vertex2, this.Vertex3 };
        }
    }
}
