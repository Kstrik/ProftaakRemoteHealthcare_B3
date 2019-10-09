using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace HealthcareClient.SceneManagement
{
    public class World
    {
        private Model3DGroup modelGroup;
        private ModelVisual3D modelVisual;

        private List<Actor> actors;
        private List<Light> lights;

        public Camera Camera
        {
            get { return this.camera; }
            set { SetCamera(value); }
        }
        private Camera camera;

        private Viewport3D viewport;

        public World(Viewport3D viewport)
        {
            this.modelGroup = new Model3DGroup();
            this.modelVisual = new ModelVisual3D();
            this.modelVisual.Content = this.modelGroup;
            viewport.Children.Add(this.modelVisual);

            this.actors = new List<Actor>();
            this.lights = new List<Light>();

            this.viewport = viewport;
        }

        public void AddActor(Actor actor)
        {
            if (actor != null)
            {
                this.actors.Add(actor);
                this.modelGroup.Children.Add(actor.GeometryModel);
            }
        }

        public void RemoveActor(Actor actor)
        {
            if (actor != null)
            {
                this.actors.Remove(actor);
                this.modelGroup.Children.Remove(actor.GeometryModel);
            }
        }

        public void AddLight(Light light)
        {
            if (light != null)
            {
                this.lights.Add(light);
                this.modelGroup.Children.Add(light);
            }
        }

        public void RemoveLight(Light light)
        {
            if (light != null)
            {
                this.lights.Remove(light);
                this.modelGroup.Children.Remove(light);
            }
        }

        private void SetCamera(Camera camera)
        {
            if (camera != null)
            {
                this.camera = camera;
                this.viewport.Camera = this.camera;
            }
        }
    }
}
