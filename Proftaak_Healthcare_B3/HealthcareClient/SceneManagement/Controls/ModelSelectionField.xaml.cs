using HealthcareClient.SceneManagement.ModelLoading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HealthcareClient.SceneManagement.Controls
{
    /// <summary>
    /// Interaction logic for ModelSelectionField.xaml
    /// </summary>
    public partial class ModelSelectionField : UserControl
    {
        public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(ModelSelectionField));
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(ModelSelectionField));
        public static readonly DependencyProperty ValueForegroundProperty = DependencyProperty.Register("ValueForeground", typeof(Brush), typeof(ModelSelectionField));
        public static readonly DependencyProperty ValueBackgroundProperty = DependencyProperty.Register("ValueBackground", typeof(Brush), typeof(ModelSelectionField));
        public static readonly DependencyProperty ValueBorderBrushProperty = DependencyProperty.Register("ValueBorderBrush", typeof(Brush), typeof(ModelSelectionField));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(IEnumerable<object>), typeof(ModelSelectionField), null);
        public static readonly DependencyProperty MaxViewportSizeProperty = DependencyProperty.Register("MaxViewportSize", typeof(double), typeof(ModelSelectionField), null);

        public Brush HeaderForeground
        {
            get { return (Brush)this.GetValue(HeaderForegroundProperty); }
            set { this.SetValue(HeaderForegroundProperty, value); }
        }

        public string Header
        {
            get { return (string)this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        public Brush ValueForeground
        {
            get { return (Brush)this.GetValue(ValueForegroundProperty); }
            set { this.SetValue(ValueForegroundProperty, value); }
        }

        public Brush ValueBackground
        {
            get { return (Brush)this.GetValue(ValueBackgroundProperty); }
            set { this.SetValue(ValueBackgroundProperty, value); }
        }

        public Brush ValueBorderBrush
        {
            get { return (Brush)this.GetValue(ValueBorderBrushProperty); }
            set { this.SetValue(ValueBorderBrushProperty, value); }
        }

        public IEnumerable<object> Value
        {
            get { return (IEnumerable<object>)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        public double MaxViewportSize
        {
            get { return (double)this.GetValue(MaxViewportSizeProperty); }
            set { this.SetValue(MaxViewportSizeProperty, value); }
        }

        public object SelectedValue
        {
            get { return cmbValue.SelectedItem; }
            set { cmbValue.SelectedItem = value; }
        }

        public Dictionary<string, string> Dictionary;

        private World world;
        private Actor actor;

        public ModelSelectionField()
        {
            InitializeComponent();

            this.world = new World(vwpViewport);
            this.world.AddLight(new DirectionalLight(Colors.White, new Vector3D(0, -1, -1)));
            this.world.Camera = new PerspectiveCamera(new Point3D(0, 0, 0), new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 90);

            this.actor = new Actor();
        }

        private void CmbValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string filepath = "";
            if (this.Dictionary != null)
                this.Dictionary.TryGetValue(cmbValue.SelectedItem.ToString(), out filepath);
            else
                filepath = cmbValue.SelectedItem.ToString();

            if (!String.IsNullOrEmpty(filepath) && File.Exists(filepath))
            {
                if (this.actor != null)
                    this.world.RemoveActor(this.actor);

                this.actor.GeometryModel = OBJModelLoader.LoadModel(filepath);
                this.actor.Material = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(filepath.Replace(".obj", ".png"), UriKind.Absolute))));
                this.world.AddActor(this.actor);

                Tuple<double, double> maxWidthHeight = GetMaxWidthHeight(this.actor.GeometryModel);
                this.world.Camera.Transform = new TranslateTransform3D(0, maxWidthHeight.Item1, maxWidthHeight.Item2 * 0.80);
            }
        }

        private Tuple<double, double> GetMaxWidthHeight(GeometryModel3D geomertyModel)
        {
            MeshGeometry3D mesh = (MeshGeometry3D)geomertyModel.Geometry;

            double maxWidth = Math.Abs(mesh.Positions[0].X);
            double maxHeight = mesh.Positions[0].Y;

            foreach (Point3D position in mesh.Positions)
            {
                if (position.Y > maxHeight)
                    maxHeight = position.Y;

                if (Math.Abs(position.X) > maxWidth)
                    maxWidth = Math.Abs(position.X);
            }

            return new Tuple<double, double>(maxWidth, maxHeight);
        }

        public void ApplyDarkTheme()
        {
            this.HeaderForeground = Brushes.White;
            this.ValueForeground = Brushes.White;
            this.ValueBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
            this.ValueBorderBrush = Brushes.Transparent;
            this.FontSize = 12;
        }
    }
}
