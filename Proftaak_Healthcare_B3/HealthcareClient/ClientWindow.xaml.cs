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
using HealthcareClient.Net;
using HealthcareClient.ServerConnection;
using HealthcareServer.Vr;
using HealthcareServer.Vr.World;
using Microsoft.Win32;
using Networking.Client;
using Networking.HealthCare;
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
    public partial class ClientWindow : Window, IServerDataReceiver, IMessageReceiver
    {
        private Client vrClient;
        private Session session;
        private DataManager dataManager;
        private RealBike bike;

        private HealthCareClient healthCareClient;

        public ClientWindow(HealthCareClient healthCareClient)
        {
            InitializeComponent();

            this.vrClient = new Client("145.48.6.10", 6666, this, null);
            this.vrClient.Connect();
            this.healthCareClient = healthCareClient;
            this.healthCareClient.SetReciever(this);

            this.dataManager = new DataManager(this.dataManager);
            GetCurrentSessions();
            ConnectToBike(this.dataManager);
            ConnectToHeartrateMonitor(this.dataManager);
        }

        private void ConnectToBike(IBikeDataReceiver bikeDataReceiver)
        {
            this.bike = new RealBike("00457", dataManager);
        }

        private void ConnectToHeartrateMonitor(IHeartrateDataReceiver heartrateDataReceiver)
        {
            HeartrateMonitor heartrateMonitor = new HeartrateMonitor(heartrateDataReceiver);
        }

        private async Task Initialize(string sessionHost, string key)
        {
            this.session = new Session(ref vrClient);
            await this.session.Create(sessionHost, key);
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (cmf_Sessions.SelectedValue != null)
            {
                string host = cmf_Sessions.SelectedValue.ToString();
                await Initialize(host, txf_Key.Value);

                if (!String.IsNullOrEmpty(this.session.GetTunnel().Id))
                {
                    lbl_Connected.Content = "Verbonden";

                    SceneManager sceneManager = new SceneManager(this.session, this.vrClient);
                    sceneManager.Show();
                }
                else
                    MessageBox.Show("Could not start session invalid session or key!");
            }
            else
                MessageBox.Show("No seession selected!");
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentSessions();
        }

        public void OnDataReceived(byte[] data)
        {
            if (this.session != null)
                this.session.OnDataReceived(data);
            else
                HandleRecieve(JObject.Parse(Encoding.UTF8.GetString(data)));
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

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    this.cmf_Sessions.Value = sessions;
                }));
            }
        }

        private void GetCurrentSessions()
        {
            this.vrClient.Transmit(Encoding.UTF8.GetBytes(Session.GetSessionsListRequest().ToString()));
        }

        public void OnMessageReceived(Message message)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                switch (message.messageType)
                {
                    case Message.MessageType.CHAT_MESSAGE:
                        {

                            break;
                        }
                    case Message.MessageType.CHANGE_RESISTANCE:
                        {
                            this.bike.SetResistence(message.Content[0]);
                            break;
                        }
                    case Message.MessageType.START_SESSION:
                        {

                            break;
                        }
                    case Message.MessageType.STOP_SESSION:
                        {

                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }));
        }
    }
}