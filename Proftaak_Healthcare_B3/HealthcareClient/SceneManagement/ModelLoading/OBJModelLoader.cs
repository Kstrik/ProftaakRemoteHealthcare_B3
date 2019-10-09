using HealthcareServer.Vr.VectorMath;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HealthcareClient.SceneManagement.ModelLoading
{
    public class OBJModelLoader
    {
        public static GeometryModel3D LoadModel(string filename)
        {
            if (File.Exists(filename))
            {
                string[] fileContent = File.ReadAllLines(filename);

                List<Vector3> vertices = new List<Vector3>();
                List<Vector2> uvCoordinates = new List<Vector2>();
                List<Vector3> normals = new List<Vector3>();
                List<TriangleFace> triangles = new List<TriangleFace>();
                Model model = new Model();

                foreach (string line in fileContent)
                {
                    string newLine = line.Replace(".", ",");
                    string[] lineSplit = newLine.Split(' ');

                    if (line.StartsWith("v "))
                    {
                        vertices.Add(new Vector3(float.Parse(lineSplit[1]), float.Parse(lineSplit[2]), float.Parse(lineSplit[3])));
                    }
                    if (line.StartsWith("vt "))
                    {
                        uvCoordinates.Add(new Vector2(float.Parse(lineSplit[1]), 1 - float.Parse(lineSplit[2])));
                    }
                    else if (line.StartsWith("vn "))
                    {
                        normals.Add(new Vector3(float.Parse(lineSplit[1]), float.Parse(lineSplit[2]), float.Parse(lineSplit[3])));
                    }
                    else if (line.StartsWith("f"))
                    {
                        Vertex[] vertexes = new Vertex[3];

                        for (int i = 1; i < 4; i++)
                        {
                            string[] vertexData = lineSplit[i].Split('/');

                            vertexes[i - 1] = new Vertex(vertices[int.Parse(vertexData[0]) - 1], uvCoordinates[int.Parse(vertexData[1]) - 1].Clone(), normals[int.Parse(vertexData[2]) - 1].Clone());
                        }
                        triangles.Add(new TriangleFace(vertexes[0], vertexes[1], vertexes[2]));
                    }
                }

                int indexCounter = 0;
                foreach (TriangleFace triangleFace in triangles)
                {
                    Vertex[] vertz = triangleFace.ToArray();

                    for (int i = 0; i < 3; i++)
                    {
                        model.Indices.Add(indexCounter);
                        model.Vertices.Add(vertz[i].Vertice);
                        model.UVCoordinates.Add(vertz[i].UVCoordinate);
                        model.Normals.Add(vertz[i].Normal);
                        indexCounter++;
                    }
                }

                return ConvertToGeometryModel3D(model);
            }
            return null;
        }

        private static GeometryModel3D ConvertToGeometryModel3D(Model model)
        {
            Point3DCollection vertices = new Point3DCollection();
            foreach (Vector3 vertice in model.Vertices)
                vertices.Add(new Point3D(vertice.X, vertice.Y, vertice.Z));

            Vector3DCollection normals = new Vector3DCollection();
            foreach (Vector3 normal in model.Normals)
                normals.Add(new System.Windows.Media.Media3D.Vector3D(normal.X, normal.Y, normal.Z));

            PointCollection uvCoordinates = new PointCollection();
            foreach (Vector2 uvCoordinate in model.UVCoordinates)
                uvCoordinates.Add(new Point(uvCoordinate.X, uvCoordinate.Y));

            Int32Collection indices = new Int32Collection();
            foreach (int indice in model.Indices)
                indices.Add(indice);

            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = vertices;
            mesh.Normals = normals;
            mesh.TextureCoordinates = uvCoordinates;
            mesh.TriangleIndices = indices;

            GeometryModel3D geometryModel = new GeometryModel3D();
            geometryModel.Geometry = mesh;

            return geometryModel;
        }
    }
}
