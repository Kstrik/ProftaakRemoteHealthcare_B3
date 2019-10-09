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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using HealthcareClient.Bike;
using HealthcareClient.BikeConnection;
using HealthcareClient.ServerConnection;
using HealthcareServer.Vr;
using HealthcareServer.Vr.World;
using Microsoft.Win32;
using Networking.Client;
using Networking.Server;
using Networking.VrServer;
using Newtonsoft.Json.Linq;
using UIControls;
using UIControls.Menu;

namespace HealthcareClient
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window, IServerDataReceiver
    {
        private Client client;
        private Session session;
        private DataManager dataManager;
        public ClientWindow()
        {
            InitializeComponent();
            this.client = new Client("145.48.6.10", 6666, this, null);
            this.client.Connect();
            dataManager = new DataManager(dataManager);
            GetCurrentSessions();
            ConnectToBike(dataManager);

            //SceneManager sceneManager = new SceneManager(new Session(ref this.client), this.client);
            //sceneManager.Show();
        }

        private void ConnectToBike(IBikeDataReceiver bikeDataReceiver)
        {
            RealBike bike = new RealBike("00438", dataManager);
        }

        private async Task Initialize(string sessionHost)
        {
            this.session = new Session(ref client);
            await this.session.Create(sessionHost, "testtest");
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sessionBox.SelectedItem != null)
            {
                string host = sessionBox.SelectedItem.ToString();
                await Initialize(host);
                lblConnected.Content = "Verbonden";

                SceneManager sceneManager = new SceneManager(this.session, this.client);
                sceneManager.Show();
            }
            else
            {
                MessageBox.Show("No seession selected!");
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentSessions();
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
                if (isNumber && (sender as TextBox).Text == "")
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
            if (this.session != null)
            {
                this.session.OnDataReceived(data);
            }
            else
            {
                HandleRecieve(JObject.Parse(Encoding.UTF8.GetString(data)));
            }
        }

        private void HandleRecieve(JObject jsonData)
        {
            if (jsonData.GetValue("id").ToString() == "session/list")
            {
                List<string> sessions = new List<string>();
                foreach (JObject session in jsonData.GetValue("data").ToObject<JToken>().Children())
                {
                    JObject clientInfo = session.GetValue("clientinfo").ToObject<JObject>();

                    sessions.Add(clientInfo.GetValue("host").ToString());
                }

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    this.sessionBox.ItemsSource = sessions;
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

        private void BtnScene_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.Scene)|*.Scene";
            if (openFileDialog.ShowDialog() == true)
            {
                SceneLoader sceneLoader = new SceneLoader(ref this.session);
                sceneLoader.LoadSceneFile(openFileDialog.FileName);
                sceneLoader.SubmitScene();
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            //Task.Run(() => session.Create());
        }
    }
}