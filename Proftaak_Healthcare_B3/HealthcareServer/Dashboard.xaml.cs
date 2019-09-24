using HealthcareServer.Vr;
using HealthcareServer.Vr.VectorMath;
using HealthcareServer.Vr.World;
using HealthcareServer.Vr.World.Components;
using Networking;
using Networking.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using UIControls;
using UIControls.Menu;

namespace HealthcareServer
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window, IServerDataReceiver, ILogger
    {
        private Client client;
        private Session session;

        private SidemenuWindow sidemenuWindow;
        private Grid grid;

        private Container sceneContainer;
        private Container propertiesContainer;
        private Container addContainer;
        private Container logContainer;

        private TreeView treeView;
        private TreeViewItem skyBox;
        private TreeViewItem nodes;
        private TreeViewItem routes;
        private TreeViewItem scene;

        private TextBox logField;

        private Dictionary<TreeViewItem, Node> nodeTree;
        private Dictionary<TreeViewItem, Route> routeTree;

        private Node currentNode;

        private ComboBox sessionsSelect;

        public Dashboard()
        {
            InitializeComponent();

            this.nodeTree = new Dictionary<TreeViewItem, Node>();
            this.routeTree = new Dictionary<TreeViewItem, Route>();

            this.sidemenuWindow = new SidemenuWindow(new Sidemenu(250, 50, 400, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236")),
                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")),
                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007acc")),
                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"))));

            grd_Grid.Children.Add(sidemenuWindow);
            InitializeUIComponents();

            this.Closing += Dashboard_Closing;

            //LOG
            this.logField = new TextBox();
            this.logField.AcceptsReturn = true;
            this.logField.Margin = new Thickness(0, 0, 0, 0);
            this.logField.Foreground = Brushes.White;
            this.logField.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E1E1E"));
            this.logField.BorderBrush = Brushes.Transparent;
            this.logField.Height = 250;
            this.logContainer.GetContentPanel().Children.Add(this.logField);

            this.client = new Client("145.48.6.10", 6666, this, this);
            //this.session = new Session(ref client, "DESKTOP-KENLEY");
            this.client.Connect();

            SetupButtons();
            GetCurrentSessions();
        }

        private void Dashboard_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private async Task Initialize(string sessionHost)
        {
            this.session = new Session(ref client, sessionHost);
            await this.session.Create();
        }

        private void InitializeUIComponents()
        {
            SetupGrid();
            SetupContainers();
            SetupTreeView();
            SetupTabControl();
        }

        private void SetupGrid()
        {
            this.grid = new Grid();
            this.grid.Margin = new Thickness(2, 2, 2, 2);
            //grid.ShowGridLines = true;
            ColumnDefinition c1 = new ColumnDefinition();
            ColumnDefinition c2 = new ColumnDefinition();
            RowDefinition r1 = new RowDefinition();
            RowDefinition r2 = new RowDefinition();
            RowDefinition r3 = new RowDefinition();
            c1.Width = new GridLength(10, GridUnitType.Star);
            c2.Width = new GridLength(20, GridUnitType.Star);
            r1.Height = new GridLength(200, GridUnitType.Star);
            r2.Height = new GridLength(200, GridUnitType.Star);
            r3.Height = new GridLength(200, GridUnitType.Star);
            this.grid.ColumnDefinitions.Add(c1);
            this.grid.ColumnDefinitions.Add(c2);
            this.grid.RowDefinitions.Add(r1);
            this.grid.RowDefinitions.Add(r2);
            this.grid.RowDefinitions.Add(r3);

            this.sidemenuWindow.GetContentPanel().Children.Add(this.grid);
        }

        private void SetupContainers()
        {
            this.sceneContainer = new Container("Scene");
            this.propertiesContainer = new Container("Properties");
            this.addContainer = new Container("Add");
            this.logContainer = new Container("Log ouput");
            //sceneContainer.GetContentPanel().Height = 500;
            Grid.SetColumn(this.sceneContainer, 0);
            Grid.SetRowSpan(this.sceneContainer, 2);
            Grid.SetColumn(this.propertiesContainer, 1);
            Grid.SetColumn(this.addContainer, 1);
            Grid.SetRow(this.addContainer, 1);
            Grid.SetColumn(this.logContainer, 0);
            Grid.SetColumnSpan(this.logContainer, 2);
            Grid.SetRow(this.logContainer, 2);

            this.grid.Children.Add(sceneContainer);
            this.grid.Children.Add(propertiesContainer);
            this.grid.Children.Add(addContainer);
            this.grid.Children.Add(logContainer);
        }

        private void SetupTreeView()
        {
            this.skyBox = new TreeViewItem();
            this.skyBox.Header = "Skybox";
            this.skyBox.Foreground = Brushes.White;

            this.nodes = new TreeViewItem();
            this.nodes.Header = "Nodes";
            this.nodes.Foreground = Brushes.White;

            this.routes = new TreeViewItem();
            this.routes.Header = "Routes";
            this.routes.Foreground = Brushes.White;

            this.scene = new TreeViewItem();
            this.scene.Header = "Scene";
            this.scene.Items.Add(this.skyBox);
            this.scene.Items.Add(this.nodes);
            this.scene.Items.Add(this.routes);
            this.scene.Foreground = Brushes.White;

            this.treeView = new TreeView();
            this.treeView.Items.Add(this.scene);
            this.treeView.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E1E1E"));
            this.treeView.Foreground = Brushes.White;
            this.treeView.BorderBrush = Brushes.Transparent;
            this.treeView.Margin = new Thickness(0, 5, 0, 5);

            this.sceneContainer.GetContentPanel().Children.Add(this.treeView);
        }

        private void SetupTabControl()
        {
            TabControl tabControl = new TabControl();
            tabControl.Items.Add(GetAddNodeTab());
            tabControl.Items.Add(GetAddRouteTab());
            tabControl.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E1E1E"));

            this.addContainer.GetContentPanel().Children.Add(tabControl);
        }

        private TabItem GetAddNodeTab()
        {
            WrapPanel transform = new WrapPanel();
            transform.Children.Add(GetInputField("PosX:", "0", true));
            transform.Children.Add(GetInputField("PosY:", "0", true));
            transform.Children.Add(GetInputField("PosZ:", "0", true));
            transform.Children.Add(GetInputField("Scale:", "1,0", true));
            transform.Children.Add(GetInputField("DirX:", "0", true));
            transform.Children.Add(GetInputField("DirY:", "0", true));
            transform.Children.Add(GetInputField("DirZ:", "0", true));

            CheckBox smoothNormals = new CheckBox();
            smoothNormals.Foreground = Brushes.White;
            smoothNormals.Content = "Smooth normals";
            smoothNormals.Margin = new Thickness(0, 5, 0, 5);

            WrapPanel terrainProperties = new WrapPanel();
            terrainProperties.Children.Add(GetInputField("Width:", "1", true));
            terrainProperties.Children.Add(GetInputField("Depth:", "1", true));
            terrainProperties.Children.Add(GetInputField("MaxHeight:", "1", true));
            terrainProperties.Children.Add(smoothNormals);

            StackPanel terrain = new StackPanel();
            terrain.Children.Add(terrainProperties);
            terrain.Children.Add(GetInputField("Heightmap:", "", false));
            terrain.Visibility = Visibility.Collapsed;
            terrain.Margin = new Thickness(0, 0, 0, 5);

            CheckBox hasTerrain = new CheckBox();
            hasTerrain.Foreground = Brushes.White;
            hasTerrain.Content = "Has terrain";
            hasTerrain.Margin = new Thickness(0, 5, 0, 5);
            hasTerrain.Checked += new RoutedEventHandler((object sender, RoutedEventArgs e) => { terrain.Visibility = Visibility.Visible; });
            hasTerrain.Unchecked += new RoutedEventHandler((object sender, RoutedEventArgs e) => { terrain.Visibility = Visibility.Collapsed; });

            Button addNodeButton = new Button();
            addNodeButton.Content = "Add node";
            addNodeButton.Foreground = Brushes.White;
            addNodeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            addNodeButton.BorderBrush = Brushes.Transparent;

            Label lblModel = new Label();
            lblModel.Content = "Model:";
            lblModel.Foreground = Brushes.White;
            ComboBox modelComboBox = new ComboBox();
            modelComboBox.ItemsSource = Assets.Models;
            StackPanel modelStackPanel = new StackPanel();
            modelStackPanel.Children.Add(lblModel);
            modelStackPanel.Children.Add(modelComboBox);

            StackPanel addNodePanel = new StackPanel();
            addNodePanel.Children.Add(GetInputField("Name: ", "", false));
            addNodePanel.Children.Add(transform);
            addNodePanel.Children.Add(modelStackPanel);
            addNodePanel.Children.Add(hasTerrain);
            addNodePanel.Children.Add(terrain);
            addNodePanel.Children.Add(addNodeButton);

            addNodeButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                string name = ((addNodePanel.Children[0] as StackPanel).Children[1] as TextBox).Text;
                if (!String.IsNullOrEmpty(name))
                {
                    Node node = new Node(name, this.session);

                    //---------------------- Transform ----------------------
                    Vector3 position = new Vector3(float.Parse((((transform.Children[0] as StackPanel).Children[1]) as TextBox).Text),
                                                    float.Parse((((transform.Children[1] as StackPanel).Children[1]) as TextBox).Text),
                                                    float.Parse((((transform.Children[2] as StackPanel).Children[1]) as TextBox).Text));
                    float scale = float.Parse((((transform.Children[3] as StackPanel).Children[1]) as TextBox).Text);
                    Vector3 rotation = new Vector3(float.Parse((((transform.Children[4] as StackPanel).Children[1]) as TextBox).Text),
                                                    float.Parse((((transform.Children[5] as StackPanel).Children[1]) as TextBox).Text),
                                                    float.Parse((((transform.Children[6] as StackPanel).Children[1]) as TextBox).Text));
                    Vr.World.Components.Transform nodeTransform = new Vr.World.Components.Transform(position, scale, rotation);

                    node.SetTransform(nodeTransform);
                    //-------------------------------------------------------

                    //---------------------- Model ----------------------
                    //string modelFile = ((addNodePanel.Children[2] as StackPanel).Children[1] as TextBox).Text;
                    string modelFile = modelComboBox.SelectedItem?.ToString();
                    if (!String.IsNullOrEmpty(modelFile))
                    {
                        Model model = new Model(modelFile, false);
                        node.SetModel(model);
                    }
                    //---------------------------------------------------

                    //---------------------- Terrain ----------------------
                    string heightMapFile = ((terrain.Children[1] as StackPanel).Children[1] as TextBox).Text;
                    if (!String.IsNullOrEmpty(heightMapFile))
                    {
                        int width = int.Parse(((terrainProperties.Children[0] as StackPanel).Children[1] as TextBox).Text);
                        int depth = int.Parse(((terrainProperties.Children[1] as StackPanel).Children[1] as TextBox).Text);
                        int maxHeight = int.Parse(((terrainProperties.Children[2] as StackPanel).Children[1] as TextBox).Text);
                        Terrain nodeTerrain = new Terrain(width, depth, maxHeight, heightMapFile, (smoothNormals.IsChecked == true) ? true : false, this.session);
                        node.SetTerrain(nodeTerrain);
                    }
                    //-----------------------------------------------------

                    //---------------------- Panel ----------------------
                    Vr.World.Components.Panel panel = new Vr.World.Components.Panel(new Vector2(1, 1), new Vector2(500, 500), new Vector4(1, 1, 1, 1), true, this.session);
                    node.SetPanel(panel);
                    //---------------------------------------------------

                    TreeViewItem treeViewItem = new TreeViewItem();
                    treeViewItem.Header = name;
                    treeViewItem.Foreground = Brushes.White;
                    treeViewItem.MouseDoubleClick += new MouseButtonEventHandler((object s, MouseButtonEventArgs args) =>
                    {
                        Node sceneNode;
                        this.nodeTree.TryGetValue(treeViewItem, out sceneNode);

                        this.currentNode = sceneNode;
                    });

                    this.nodes.Items.Add(treeViewItem);
                    this.nodeTree.Add(treeViewItem, node);
                    Task.Run(() => this.session.GetScene().AddNode(node));
                }
            });

            TabItem addNode = new TabItem();
            addNode.Header = "Add Node";
            addNode.Content = addNodePanel;
            addNode.Foreground = Brushes.White;
            addNode.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            addNode.BorderBrush = Brushes.Transparent;
            return addNode;
        }

        private TabItem GetAddRouteTab()
        {
            Label lblPoints = new Label();
            lblPoints.Content = "Points:";
            lblPoints.Foreground = Brushes.White;

            StackPanel points = new StackPanel();
            points.Children.Add(lblPoints);
            points.Children.Add(GetPosDirField());
            points.Children.Add(GetPosDirField());

            StackPanel addRoutePanel = new StackPanel();

            Button addPointButton = new Button();
            addPointButton.Content = "Add point";
            addPointButton.Foreground = Brushes.White;
            addPointButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            addPointButton.BorderBrush = Brushes.Transparent;
            addPointButton.Margin = new Thickness(5, 5, 5, 5);
            addPointButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                points.Children.Add(GetPosDirField());
            });

            Button removePointButton = new Button();
            removePointButton.Content = "Remove point";
            removePointButton.Foreground = Brushes.White;
            removePointButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            removePointButton.BorderBrush = Brushes.Transparent;
            removePointButton.Margin = new Thickness(5, 5, 5, 5);
            removePointButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                if(points.Children.Count > 3)
                {
                    points.Children.RemoveAt(points.Children.Count - 1);
                }
            });

            addRoutePanel.Children.Add(points);

            StackPanel addRoad = new StackPanel();
            addRoad.Children.Add(GetComboBoxField("Diffuse:", Assets.DiffuseTextures));
            addRoad.Children.Add(GetComboBoxField("Normal:", Assets.NormalMaps));
            addRoad.Children.Add(GetComboBoxField("Specular:", Assets.SpecularMaps));
            addRoad.Children.Add(GetInputField("Heightoffset:", "0,01", true));
            addRoad.Visibility = Visibility.Collapsed;
            addRoad.Margin = new Thickness(0, 0, 0, 5);

            CheckBox hasRoad = new CheckBox();
            hasRoad.Foreground = Brushes.White;
            hasRoad.Content = "Has road";
            hasRoad.Margin = new Thickness(0, 5, 0, 5);
            hasRoad.Checked += new RoutedEventHandler((object sender, RoutedEventArgs e) => { addRoad.Visibility = Visibility.Visible; });
            hasRoad.Unchecked += new RoutedEventHandler((object sender, RoutedEventArgs e) => { addRoad.Visibility = Visibility.Collapsed; });

            addRoutePanel.Children.Add(addPointButton);
            addRoutePanel.Children.Add(removePointButton);
            addRoutePanel.Children.Add(hasRoad);
            addRoutePanel.Children.Add(addRoad);

            Button addRouteButton = new Button();
            addRouteButton.Content = "Add route";
            addRouteButton.Foreground = Brushes.White;
            addRouteButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            addRouteButton.BorderBrush = Brushes.Transparent;
            addRouteButton.Margin = new Thickness(5, 5, 5, 5);
            addRouteButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                //---------------------- Route ----------------------
                Route route = new Route(this.session);

                for(int i = 1; i < points.Children.Count; i++)
                {
                    Vector3 position = new Vector3(float.Parse(((((points.Children[i] as WrapPanel).Children[0] as StackPanel).Children[1]) as TextBox).Text),
                                                    float.Parse(((((points.Children[i] as WrapPanel).Children[1] as StackPanel).Children[1]) as TextBox).Text),
                                                    float.Parse(((((points.Children[i] as WrapPanel).Children[2] as StackPanel).Children[1]) as TextBox).Text));
                    Vector3 direction = new Vector3(float.Parse(((((points.Children[i] as WrapPanel).Children[3] as StackPanel).Children[1]) as TextBox).Text),
                                                    float.Parse(((((points.Children[i] as WrapPanel).Children[4] as StackPanel).Children[1]) as TextBox).Text),
                                                    float.Parse(((((points.Children[i] as WrapPanel).Children[5] as StackPanel).Children[1]) as TextBox).Text));
                    route.AddRouteNode(new Route.RouteNode(position, direction));
                }
                //---------------------------------------------------

                //---------------------- Road ----------------------
                if(hasRoad.IsChecked == true)
                {
                    string diffuse = ((addRoad.Children[0] as StackPanel).Children[1] as ComboBox).SelectedItem.ToString();
                    string normal = ((addRoad.Children[1] as StackPanel).Children[1] as ComboBox).SelectedItem.ToString();
                    string specular = ((addRoad.Children[2] as StackPanel).Children[1] as ComboBox).SelectedItem.ToString();
                    float heightOffset = float.Parse(((addRoad.Children[3] as StackPanel).Children[1] as TextBox).Text);

                    Road road = new Road(diffuse, normal, specular, heightOffset, route, this.session);
                    route.SetRoad(road);
                }
                //--------------------------------------------------

                TreeViewItem treeViewItem = new TreeViewItem();
                treeViewItem.Header = "Route";
                treeViewItem.Foreground = Brushes.White;
                treeViewItem.MouseDoubleClick += new MouseButtonEventHandler((object s, MouseButtonEventArgs args) =>
                {
                    Route sceneRoute;
                    this.routeTree.TryGetValue(treeViewItem, out sceneRoute);

                    if (this.currentNode != null)
                        Task.Run(() => this.currentNode.FollowRoute(sceneRoute, 2.0f, new Vector3(0,0,0), new Vector3(0,0,0)));
                });

                this.routes.Items.Add(treeViewItem);
                this.routeTree.Add(treeViewItem, route);
                Task.Run(() => this.session.GetScene().AddRoute(route));
            });

            addRoutePanel.Children.Add(addRouteButton);

            TabItem addRoute = new TabItem();
            addRoute.Header = "Add Route";
            addRoute.Content = addRoutePanel;
            addRoute.Foreground = Brushes.White;
            addRoute.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            addRoute.BorderBrush = Brushes.Transparent;
            return addRoute;
        }

        private void SetupButtons()
        {
            Button resetSceneButton = new Button();
            resetSceneButton.Content = "Reset scene";
            resetSceneButton.Foreground = Brushes.White;
            resetSceneButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            resetSceneButton.BorderBrush = Brushes.Transparent;
            resetSceneButton.Margin = new Thickness(5, 5, 5, 5);
            resetSceneButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                this.nodeTree.Clear();
                this.routeTree.Clear();
                this.currentNode = null;
                this.nodes.Items.Clear();
                this.routes.Items.Clear();
                Task.Run(() => this.session.GetScene().Reset());
            });

            WrapPanel wrapPanel = new WrapPanel();
            wrapPanel.Margin = new Thickness(5, 5, 5, 5);

            Button refreshSessionsButton = new Button();
            refreshSessionsButton.Content = "Refresh sessions";
            refreshSessionsButton.Foreground = Brushes.White;
            refreshSessionsButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            refreshSessionsButton.BorderBrush = Brushes.Transparent;
            refreshSessionsButton.Margin = new Thickness(5, 5, 5, 5);
            refreshSessionsButton.Width = 100;
            refreshSessionsButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                GetCurrentSessions();
            });

            Button startSessionButton = new Button();
            startSessionButton.Content = "Start session";
            startSessionButton.Foreground = Brushes.White;
            startSessionButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            startSessionButton.BorderBrush = Brushes.Transparent;
            startSessionButton.Margin = new Thickness(5, 5, 5, 5);
            startSessionButton.Width = 100;
            startSessionButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                //string host = ((wrapPanel.Children[0] as StackPanel).Children[1] as TextBox).Text;
                string host = ((wrapPanel.Children[0] as StackPanel).Children[1] as ComboBox).SelectedItem.ToString();
                Task.Run(() => Initialize(host));
                refreshSessionsButton.Visibility = Visibility.Collapsed;
            });

            Label lblSessions = new Label();
            lblSessions.Content = "Session host:";
            lblSessions.Foreground = Brushes.White;
            this.sessionsSelect = new ComboBox();
            StackPanel stkSesions = new StackPanel();
            stkSesions.Children.Add(lblSessions);
            stkSesions.Children.Add(this.sessionsSelect);

            //wrapPanel.Children.Add(GetInputField("Session host:", "DESKTOP-KENLEY", false));
            wrapPanel.Children.Add(stkSesions);
            wrapPanel.Children.Add(startSessionButton);
            wrapPanel.Children.Add(refreshSessionsButton);

            Button setTimeButton = new Button();
            setTimeButton.Content = "Set time";
            setTimeButton.Foreground = Brushes.White;
            setTimeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            setTimeButton.BorderBrush = Brushes.Transparent;
            setTimeButton.Margin = new Thickness(5, 5, 5, 5);
            setTimeButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                //List<PanelLine> lines = new List<PanelLine>();
                //lines.Add(new PanelLine(new Vector2(10, 10), new Vector2(10, 100), new Vector4(0, 0, 0, 1)));
                //lines.Add(new PanelLine(new Vector2(10, 100), new Vector2(100, 100), new Vector4(0, 0, 0, 1)));
                //Task.Run(async () => await this.currentNode.GetPanel().DrawLines(1, lines));

                //Task.Run(async () => await this.currentNode.GetPanel().DrawText("Test", new Vector2(10, 10), 20, new Vector4(0, 0, 0, 1), "segoeui"));


                //Task.Run(async () =>
                //{
                //    await this.currentNode.GetPanel().SetClearColor(new Vector4(0, 0, 1, 1));
                //    await this.currentNode.GetPanel().Swap();
                //    await this.currentNode.GetPanel().DrawText("Test2", new Vector2(10, 10), 20, new Vector4(0, 0, 0, 1), "segoeui");
                //    await Task.Delay(5000);
                //    await this.currentNode.GetPanel().Swap();
                //    await Task.Delay(5000);
                //    await this.currentNode.GetPanel().Swap();
                //});

                Task.Run(async () =>
                {
                    await this.currentNode.GetTerrain().AddTextureLayer(@"data\NetworkEngine\textures\terrain\savanna_green_d.jpg", @"data\NetworkEngine\textures\terrain\savanna_green_n.jpg", 0, 25, 1);
                    await this.currentNode.GetTerrain().AddTextureLayer(@"data\NetworkEngine\textures\terrain\snow_bumpy_d.jpg", @"data\NetworkEngine\textures\terrain\snow_bumpy_n.jpg", 25, 50, 1);
                });

                //int time = int.Parse(((this.propertiesContainer.GetContentPanel().Children[2] as StackPanel).Children[1] as TextBox).Text);
                //Task.Run(() => this.session.GetScene().GetSkyBox().SetTime(time));
            });

            Button deleteGroundPLaneButton = new Button();
            deleteGroundPLaneButton.Content = "Remove ground plane";
            deleteGroundPLaneButton.Foreground = Brushes.White;
            deleteGroundPLaneButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            deleteGroundPLaneButton.BorderBrush = Brushes.Transparent;
            deleteGroundPLaneButton.Margin = new Thickness(5, 5, 5, 5);
            deleteGroundPLaneButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                Task.Run(() => RemoveGroundPlane());
            });

            this.propertiesContainer.GetContentPanel().Children.Add(resetSceneButton);
            this.propertiesContainer.GetContentPanel().Children.Add(wrapPanel);
            this.propertiesContainer.GetContentPanel().Children.Add(GetInputField("Time:", "12", true));
            this.propertiesContainer.GetContentPanel().Children.Add(setTimeButton);
            this.propertiesContainer.GetContentPanel().Children.Add(deleteGroundPLaneButton);
        }

        private StackPanel GetInputField(string header, string text, bool isNumber)
        {
            Label label = new Label();
            label.Content = header;
            label.Foreground = Brushes.White;
            TextBox textBox = new TextBox();
            textBox.Text = text;
            textBox.MinWidth = 50;
            textBox.Foreground = Brushes.White;
            textBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            textBox.BorderBrush = Brushes.Transparent;

            textBox.LostFocus += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                if(isNumber && (sender as TextBox).Text == "")
                {
                    (sender as TextBox).Text = "0";
                }
            });

            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(textBox);
            return stackPanel;
        }

        private WrapPanel GetPosDirField()
        {
            WrapPanel posDirField = new WrapPanel();
            posDirField.Children.Add(GetInputField("PosX:", "0", true));
            posDirField.Children.Add(GetInputField("PosY:", "0", true));
            posDirField.Children.Add(GetInputField("PosZ:", "0", true));
            posDirField.Children.Add(GetInputField("DirX:", "0", true));
            posDirField.Children.Add(GetInputField("DirY:", "0", true));
            posDirField.Children.Add(GetInputField("DirZ:", "0", true));
            return posDirField;
        }

        private StackPanel GetComboBoxField(string header, List<string> items)
        {
            Label label = new Label();
            label.Content = header;
            label.Foreground = Brushes.White;
            ComboBox comboBox = new ComboBox();
            comboBox.ItemsSource = items;
            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(comboBox);

            return stackPanel;
        }

        public void OnDataReceived(byte[] data)
        {
            if(this.session != null)
            {
                this.session.OnDataReceived(data);
            }
            else
            {
                HandleRecieve(JObject.Parse(Encoding.UTF8.GetString(data)));
            }
        }

        public void Log(string text)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                this.logField.Text += text;
                this.logField.ScrollToEnd();
            }));
        }

        private void HandleRecieve(JObject jsonData)
        {
            if(jsonData.GetValue("id").ToString() == "session/list")
            {
                List<string> sessions = new List<string>();
                foreach (JObject session in jsonData.GetValue("data").ToObject<JToken>().Children())
                {
                    JObject clientInfo = session.GetValue("clientinfo").ToObject<JObject>();

                    sessions.Add(clientInfo.GetValue("host").ToString());
                }

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    this.sessionsSelect.ItemsSource = sessions;
                }));
            }
        }

        private void GetCurrentSessions()
        {
            this.client.Transmit(Encoding.UTF8.GetBytes(Session.GetSessionsListRequest().ToString()));
        }

        private async Task RemoveGroundPlane()
        {
            Node node = await this.session.GetScene().FindNode("GroundPlane");
            await node.Delete();
        }
    }
}
