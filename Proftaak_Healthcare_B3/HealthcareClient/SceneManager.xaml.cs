﻿using HealthcareClient.Resources;
using HealthcareClient.SceneManagement;
using HealthcareClient.SceneManagement.Controls;
using HealthcareClient.SceneManagement.ModelLoading;
using HealthcareServer.Vr;
using HealthcareServer.Vr.World;
using Microsoft.Win32;
using Networking;
using Networking.Client;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;
using UIControls.Fields;

namespace HealthcareClient
{
    /// <summary>
    /// Interaction logic for SceneManager.xaml
    /// </summary>
    public partial class SceneManager : Window, ILogger
    {
        private Dictionary<string, object> treeItems;

        //private List<Node> nodes;
        //private List<Route> routes;

        private Client client;
        private Session session;

        public Node BikeNode;
        public HealthcareServer.Vr.World.Components.Panel TextPanel;

        public SceneManager(Session session, Client client)
        {
            InitializeComponent();

            this.treeItems = new Dictionary<string, object>();

            this.client = client;
            this.session = session;
            this.client.SetLogger(this);

            //this.nodes = new List<Node>();
            //this.routes = new List<Route>();

            con_Properties.SetValue(Grid.ColumnProperty, 1);
            con_Properties.SetValue(Grid.ColumnSpanProperty, 4);
            con_Properties.Visibility = Visibility.Collapsed;
            con_Builder.SetValue(Grid.ColumnSpanProperty, 4);
        }

        private async void LoadScene_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Scene files (*.Scene)|*.Scene";
            if (openFileDialog.ShowDialog() == true)
            {
                await this.session.GetScene().Reset();
                SceneLoader sceneLoader = new SceneLoader(ref this.session);
                sceneLoader.LoadSceneFile(openFileDialog.FileName);
                await sceneLoader.SubmitScene();
                await this.session.GetScene().GetSkyBox().SetTime(sceneLoader.SkyBox.GetTime());
                //this.nodes = sceneLoader.Nodes;
                //this.routes = sceneLoader.Routes;

                triNodes.Items.Clear();
                triRoutes.Items.Clear();

                foreach (Node node in sceneLoader.Nodes)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Foreground = Brushes.White;
                    item.Header = node.Name;
                    item.MouseDoubleClick += TreeItem_MouseDoubleClick;
                    triNodes.Items.Add(item);
                    this.treeItems.Add(node.Name, node);
                }
                int routeCounter = 0;
                foreach (Route route in sceneLoader.Routes)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Foreground = Brushes.White;
                    item.Header = $"Route {routeCounter}";
                    item.MouseDoubleClick += TreeItem_MouseDoubleClick;
                    triRoutes.Items.Add(item);
                    this.treeItems.Add(item.Header.ToString(), route);
                    routeCounter++;
                }

                Node groundPlane = await this.session.GetScene().FindNode("GroundPlane");
                await groundPlane.Delete();
                await SetupBikeWithPanel();
            }
        }

        private void SaveScene_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private async void ResetScene_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await this.session.GetScene().Reset();
            //this.nodes.Clear();
            //this.routes.Clear();
            this.triNodes.Items.Clear();
            this.triRoutes.Items.Clear();
            this.treeItems.Clear();
            Cancel();
        }

        private async void StartSession_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (String.IsNullOrEmpty(this.session.Id))
            {
                await this.session.Create("", "");
            }
        }

        private void NewNode_MouseDown(object sender, MouseButtonEventArgs e)
        {
            con_Properties.ScrollToTop();
            con_Builder.Visibility = Visibility.Collapsed;
            con_Properties.Visibility = Visibility.Visible;

            NodePropertiesView nodePropertiesView = new NodePropertiesView(AddNode, Cancel, this.session);
            con_Properties.Children.Clear();
            con_Properties.Children.Add(nodePropertiesView);
        }

        private void NewRoute_MouseDown(object sender, MouseButtonEventArgs e)
        {
            con_Properties.ScrollToTop();
            con_Builder.Visibility = Visibility.Collapsed;
            con_Properties.Visibility = Visibility.Visible;

            TextField nameField = new TextField();
            nameField.Header = "Name:";
            nameField.ApplyDarkTheme();

            RoutePropertiesView routePropertiesView = new RoutePropertiesView(AddRoute, Cancel, this.session);
            con_Properties.Children.Clear();
            con_Properties.Children.Add(nameField);
            con_Properties.Children.Add(routePropertiesView);
        }

        public void Log(string text)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                txb_Log.Text += text;
            }));
        }

        private void AddNode(Node node)
        {
            if (!this.treeItems.Keys.Contains(node.Name))
            {
                Task.Run(() => node.Add());
                this.treeItems.Add(node.Name, node);

                TreeViewItem item = new TreeViewItem();
                item.Foreground = Brushes.White;
                item.Header = node.Name;
                item.MouseDoubleClick += TreeItem_MouseDoubleClick;
                triNodes.Items.Add(item);

                con_Builder.Visibility = Visibility.Visible;
                con_Properties.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Name already exists!");
            }
        }

        private void AddRoute(Route route)
        {
            string name = (con_Properties.Children[0] as TextField).Value;

            if (name != null)
            {
                if (!this.treeItems.Keys.Contains(name))
                {
                    Task.Run(() => route.Add());
                    this.treeItems.Add(name, route);

                    TreeViewItem item = new TreeViewItem();
                    item.Foreground = Brushes.White;
                    item.Header = name;
                    item.MouseDoubleClick += TreeItem_MouseDoubleClick;
                    triRoutes.Items.Add(item);

                    con_Builder.Visibility = Visibility.Visible;
                    con_Properties.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Name already exists!");
                }
            }
            else
            {
                MessageBox.Show("Name cannot be empty!");
            }
        }

        private void UpdateNode(Node node)
        {
            Task.Run(() => node.Update());
        }

        private void UpdateRoute(Route route)
        {
            Task.Run(() => route.Update());
        }

        private void Cancel()
        {
            con_Builder.Visibility = Visibility.Visible;
            con_Properties.Visibility = Visibility.Collapsed;
            con_Properties.ScrollToTop();
        }

        private void TreeItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            object value = null;

            string test = item.Header.ToString();
            this.treeItems.TryGetValue(item.Header.ToString(), out value);

            con_Properties.ScrollToTop();

            if (value.GetType() == typeof(Node))
            {
                con_Builder.Visibility = Visibility.Collapsed;
                con_Properties.Visibility = Visibility.Visible;

                NodePropertiesView nodePropertiesView = new NodePropertiesView(UpdateNode, Cancel, this.session, (Node)value);
                con_Properties.Children.Clear();
                con_Properties.Children.Add(nodePropertiesView);
            }
            else if (value.GetType() == typeof(Route))
            {
                con_Builder.Visibility = Visibility.Collapsed;
                con_Properties.Visibility = Visibility.Visible;

                TextField nameField = new TextField();
                nameField.Header = "Name:";
                nameField.ApplyDarkTheme();
                nameField.Value = item.Header.ToString();
                nameField.IsEnabled = false;

                RoutePropertiesView routePropertiesView = new RoutePropertiesView(UpdateRoute, Cancel, this.session, (Route)value);
                con_Properties.Children.Clear();
                con_Properties.Children.Add(nameField);
                con_Properties.Children.Add(routePropertiesView);
            }
        }

        public async Task SetupBikeWithPanel()
        {
            this.BikeNode = new Node("BikeNode", this.session);
            this.BikeNode.SetTransform(new HealthcareServer.Vr.World.Components.Transform(new HealthcareServer.Vr.VectorMath.Vector3(0, 0, 0), 1.0f,
                                        new HealthcareServer.Vr.VectorMath.Vector3(0, 0, 0)));
            this.BikeNode.SetModel(new HealthcareServer.Vr.World.Components.Model(@"data\NetworkEngine\models\bike\Bike.fbx", false));
            await this.session.GetScene().AddNode(this.BikeNode);

            Node panelNode = new Node("PanelNode", this.session, this.BikeNode.Id);
            panelNode.SetTransform(new HealthcareServer.Vr.World.Components.Transform(new HealthcareServer.Vr.VectorMath.Vector3(-0.5f, 1.1f, 0), 1.0f,
                                        new HealthcareServer.Vr.VectorMath.Vector3(-60, 90, 0)));
            this.TextPanel = new HealthcareServer.Vr.World.Components.Panel(new HealthcareServer.Vr.VectorMath.Vector2(0.4f, 0.4f),
                                        new HealthcareServer.Vr.VectorMath.Vector2(512, 512),
                                        new HealthcareServer.Vr.VectorMath.Vector4(1, 1, 1, 0.5f), false, this.session);
            panelNode.SetPanel(this.TextPanel);
            panelNode.ParentId = this.BikeNode.Id;
            await this.session.GetScene().AddNode(panelNode);

            Node cameraNode = await this.session.GetScene().FindNode("Camera");
            cameraNode.SetTransform(new HealthcareServer.Vr.World.Components.Transform(new HealthcareServer.Vr.VectorMath.Vector3(0, 0, 0), 1.0f,
                                        new HealthcareServer.Vr.VectorMath.Vector3(0, 90, 0)));
            cameraNode.ParentId = this.BikeNode.Id;
            await cameraNode.Update();

            //Route route = new Route(this.session);
            //route.AddRouteNode(new Route.RouteNode(new HealthcareServer.Vr.VectorMath.Vector3(0, 0, 0), new HealthcareServer.Vr.VectorMath.Vector3(0, 0, 0)));
            //route.AddRouteNode(new Route.RouteNode(new HealthcareServer.Vr.VectorMath.Vector3(10, 0, 10), new HealthcareServer.Vr.VectorMath.Vector3(0, 0, 0)));
            //await this.session.GetScene().AddRoute(route);
            //Route route = this.session.GetScene().GetRoutes().Where(r => r.Name == "Route1").First();
            Route route = this.session.GetScene().GetRoutes().First();

            await this.TextPanel.SetClearColor(new HealthcareServer.Vr.VectorMath.Vector4(1, 1, 1, 0.5f));
            await this.TextPanel.Swap();
            await this.TextPanel.Clear();
            await this.TextPanel.Swap();

            await this.BikeNode.FollowRoute(route, 0.0f, new HealthcareServer.Vr.VectorMath.Vector3(0, 0, 0), new HealthcareServer.Vr.VectorMath.Vector3(0, 0, 0));
        }
    }
}
