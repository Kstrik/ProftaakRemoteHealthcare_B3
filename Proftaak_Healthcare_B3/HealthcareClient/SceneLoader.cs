﻿using HealthcareServer.Vr;
using HealthcareServer.Vr.VectorMath;
using HealthcareServer.Vr.World;
using HealthcareServer.Vr.World.Components;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Transform = HealthcareServer.Vr.World.Components.Transform;

namespace HealthcareClient
{
    public class SceneLoader
    {
        private Session session;
        private List<Node> nodes;
        private List<Route> routes;
        private SkyBox skyBox;
        public SceneLoader(ref Session session)
        {
            this.session = session;
            this.nodes = new List<Node>();
            this.routes = new List<Route>();
            this.skyBox = new SkyBox(12, null);
        }

        public void LoadSceneFile(string fileName)
        {
            JObject scene = JObject.Parse(File.ReadAllText(fileName));

            try
            {
                string sceneName = scene.GetValue("name").ToString();
                JArray objects = scene.GetValue("objects").ToObject<JArray>();

                foreach (JObject jobject in objects.Children())
                {
                    string type = jobject.GetValue("type").ToString();
                    switch (type)
                    {
                        case "node":
                            LoadNode(jobject);                          
                            break; 

                        case "route":
                            LoadRoute(jobject);
                            break;

                        default:
                            break;
                    }
                }
                SubmitScene();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error");           
            }
        }

        private void LoadNode(JObject jObject)
        {
            string name = jObject.GetValue("name").ToString();
            string parent = "";
            Model model = null;
            Terrain terrain = null;
            Panel panel = null;

            if (jObject.ContainsKey("parent"))
            {
                parent = jObject.GetValue("parent").ToString();
            }
            JObject jTransform = jObject.GetValue("transform").ToObject<JObject>();
            JArray jPosition = jTransform.GetValue("position").ToObject<JArray>();
            JArray jRotation = jTransform.GetValue("rotation").ToObject<JArray>();
            Vector3 position = new Vector3(float.Parse(jPosition[0].ToString()), float.Parse(jPosition[1].ToString()), float.Parse(jPosition[2].ToString()));
            float scale = float.Parse(jTransform.GetValue("scale").ToString());
            Vector3 rotation = new Vector3(float.Parse(jRotation[0].ToString()), float.Parse(jRotation[1].ToString()), float.Parse(jRotation[2].ToString()));
            Transform transform = new Transform(position, scale, rotation);

            if (jObject.ContainsKey("model"))
            {
                JObject jModel = jObject.GetValue("model").ToObject<JObject>();
                string fileName = jModel.GetValue("filename").ToString();
                bool cullBackFaces = (jModel.GetValue("cullbackfaces").ToString().ToLower() == "true") ? true : false;
                string animationName = jModel.GetValue("animation").ToString();
                model = new Model(fileName, cullBackFaces, animationName);
            }
            if (jObject.ContainsKey("terrain"))
            {
                JObject jTerrain = jObject.GetValue("terrain").ToObject<JObject>();
                bool smoothNormals = (jTerrain.GetValue("cullbackfaces").ToString().ToLower() == "true") ? true : false;
                int width = int.Parse(jTerrain.GetValue("width").ToString());
                int depth = int.Parse(jTerrain.GetValue("depth").ToString());
                int maxHeight = int.Parse(jTerrain.GetValue("maxheight").ToString());
                string heightMap = jTerrain.GetValue("heightmap").ToString();
                terrain = new Terrain(width,depth, maxHeight, heightMap, smoothNormals, session);
            }
            if (jObject.ContainsKey("panel"))
            {
                JObject jPanel = jObject.GetValue("panel").ToObject<JObject>();
                JArray jSize = jPanel.GetValue("size").ToObject<JArray>();
                JArray jResolution = jPanel.GetValue("resolution").ToObject<JArray>();
                JArray jBackground = jPanel.GetValue("background").ToObject<JArray>();
                Vector2 size = new Vector2(float.Parse(jSize[0].ToString()), float.Parse(jSize[1].ToString()));
                Vector2 resolution = new Vector2(float.Parse(jResolution[0].ToString()), float.Parse(jResolution[1].ToString()));
                Vector4 background = new Vector4(float.Parse(jBackground[0].ToString()), float.Parse(jBackground[1].ToString()), float.Parse(jBackground[2].ToString()), float.Parse(jBackground[2].ToString()));
                bool castshadows = (jPanel.GetValue("cullbackfaces").ToString().ToLower() == "true") ? true : false;
                panel = new Panel(size, resolution, background, castshadows, session);
            }

            Node node = new Node(name, session, parent);
            node.SetTransform(transform);
            if (model != null)
                node.SetModel(model);
            if (terrain != null)
                node.SetTerrain(terrain);
            if (panel != null)
                node.SetPanel(panel);

            nodes.Add(node);
        }

        private void LoadRoute(JObject jObject)
        {
            string name = jObject.GetValue("name").ToString();
            Route route = null;
            Road road = null;

            if (jObject.ContainsKey("road"))
            {
                JObject jRoad = jObject.GetValue("road").ToObject<JObject>();
                string diffuse = jRoad.GetValue("diffuse").ToString();
                string normal = jRoad.GetValue("normal").ToString();
                string specular = jRoad.GetValue("specular").ToString();
                float heightOffset = float.Parse(jRoad.GetValue("heightoffset").ToString());               
                road = new Road(diffuse, normal, specular,heightOffset, route, session);
            }

            route = new Route(session);
            route.SetRoad(road);
            routes.Add(route);
        }

        public void SubmitScene()
        {
            Task.Run(async () =>
            {
                foreach (Node node in this.nodes)
                    await node.Add();
                foreach (Route route in this.routes)
                    await route.Add();
            });
        }
    }
}