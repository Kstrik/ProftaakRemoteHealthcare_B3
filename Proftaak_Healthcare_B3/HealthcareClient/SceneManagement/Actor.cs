using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace HealthcareClient.SceneManagement
{
    public class Actor
    {
        private Transform3DGroup transform;
        private TranslateTransform3D translation;
        private RotateTransform3D rotation;
        private AxisAngleRotation3D axisAngle;
        private ScaleTransform3D scale;

        public GeometryModel3D GeometryModel
        {
            get { return this.geometryModel; }
            set { SetGeometryModel(value); }
        }
        private GeometryModel3D geometryModel;

        public Material Material
        {
            get { return this.material; }
            set { SetMaterial(value); }
        }
        private Material material;

        public Vector3D Position;
        public Vector3D Velocity;
        public Vector3D RotateAxis;
        public double Angle;
        public Vector3D Scaling;

        public Actor()
        {
            this.Position = new Vector3D(0, 0, 0);
            this.Velocity = new Vector3D(0, 0, 0);
            this.RotateAxis = new Vector3D(0, 1, 0);
            this.Angle = 0;
            this.Scaling = new Vector3D(1, 1, 1);

            this.transform = new Transform3DGroup();
            UpdateTransformations();

            this.transform.Children.Add(this.translation);
            this.transform.Children.Add(this.rotation);
            this.transform.Children.Add(this.scale);
        }

        public virtual void Update(float deltatime)
        {
            this.Position += this.Velocity * deltatime;

            UpdateTransformations();

            if (this.geometryModel != null)
                this.geometryModel.Transform = this.transform;
        }

        private void UpdateTransformations()
        {
            this.transform.Children.Clear();
            this.translation = new TranslateTransform3D(this.Position);
            this.axisAngle = new AxisAngleRotation3D(this.RotateAxis, this.Angle);
            this.rotation = new RotateTransform3D(this.axisAngle);
            this.scale = new ScaleTransform3D(this.Scaling);

            this.transform.Children.Add(this.translation);
            this.transform.Children.Add(this.rotation);
            this.transform.Children.Add(this.scale);
        }

        private void SetGeometryModel(GeometryModel3D geometryModel)
        {
            if (geometryModel != null)
            {
                this.geometryModel = geometryModel;
                this.geometryModel.Transform = this.transform;

                if (this.material != null)
                {
                    this.geometryModel.Material = this.material;
                    this.geometryModel.BackMaterial = this.material;
                }
            }
        }

        private void SetMaterial(Material material)
        {
            if (material != null)
            {
                this.material = material;

                if (this.geometryModel != null)
                {
                    this.geometryModel.Material = this.material;
                    this.geometryModel.BackMaterial = this.material;
                }
            }
        }
    }
}
