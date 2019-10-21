using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using UIControls.Charts;
using UIControls.Menu;

namespace HealthcareClient
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window, IServerDataReceiver, IMessageReceiver, IClientMessageReceiver
    {
        private Client vrClient;
        private Session session;
        private DataManager dataManager;
        private RealBike bike;

        private HealthCareClient healthCareClient;

        private LiveChartControl liveChartControl;

        private SceneManager sceneManager;

        private bool sessionInProgress;
        private bool bikeIsConnected;

        private int distance = 0;
        private string lastChatMessage;

        public ClientWindow(HealthCareClient healthCareClient)
        {
            InitializeComponent();

            this.vrClient = new Client("145.48.6.10", 6666, this, null);
            this.vrClient.Connect();
            this.healthCareClient = healthCareClient;
            this.healthCareClient.SetReciever(this);

            this.dataManager = new DataManager(this.healthCareClient, this);
            GetCurrentSessions();

            this.liveChartControl = new LiveChartControl("Hartslag", "", "", 40, 250, 180, 20, LiveChart.BlueGreenDarkTheme, true, true, true, true, false, false, true);
            Grid.SetColumn(this.liveChartControl, 1);
            grd_DataGrid.Children.Add(this.liveChartControl);

            this.Closed += ClientWindow_Closed;
            this.sessionInProgress = false;
            this.bikeIsConnected = false;

            this.lastChatMessage = "";
            this.KeyUp += ClientWindow_KeyUp;
        }

        private void ClientWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
                btn_SendTestData.Visibility = (btn_SendTestData.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void ClientWindow_Closed(object sender, EventArgs e)
        {
            if(this.sceneManager != null)
                await this.sceneManager.BikeNode.SetFollowSpeed(0);
            this.vrClient.Disconnect();
            this.healthCareClient.Disconnect();
            Environment.Exit(0);
        }

        private bool ConnectToBike()
        {
            if (!String.IsNullOrEmpty(txf_BikeId.Value))
            {
                this.bike = new RealBike(txf_BikeId.Value, this.dataManager);
                this.bikeIsConnected = true;
                return true;
            }

            MessageBox.Show("FietsId veld mag niet leeg zijn!");
            return false;
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
                    btn_Refresh.IsEnabled = false;
                    btn_ConnectToSession.IsEnabled = false;
                    btn_Refresh.Foreground = Brushes.Gray;
                    btn_ConnectToSession.Foreground = Brushes.Gray;

                    this.sceneManager = new SceneManager(this.session, this.vrClient);
                    this.sceneManager.Show();
                }
                else
                    MessageBox.Show("Could not start session invalid session or key!");
            }
            else
                MessageBox.Show("No session selected!");
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentSessions();
        }

        private async void ConnectToBike_Click(object sender, RoutedEventArgs e)
        {
            //if(ConnectToBike())
            //{
            //    txf_BikeId.IsEnabled = false;
            //    btn_ConnectToBike.IsEnabled = false;
            //    btn_ConnectToBike.Foreground = Brushes.Gray;
            //}

            if (!String.IsNullOrEmpty(txf_BikeId.Value))
            {
                this.bike = new RealBike(txf_BikeId.Value, this.dataManager);
                if (await this.bike.ConnectToBike())
                {
                    //txf_BikeId.IsEnabled = false;
                    //btn_ConnectToBike.IsEnabled = false;
                    //btn_ConnectToBike.Foreground = Brushes.Gray;

                    this.bikeIsConnected = true;
                }
                else
                {
                    //txf_BikeId.IsEnabled = true;
                    //btn_ConnectToBike.IsEnabled = true;
                    //btn_ConnectToBike.Foreground = Brushes.White;
                    MessageBox.Show("Kon niet verbinden met de fiets!");
                }
            }
            else
                MessageBox.Show("FietsId veld mag niet leeg zijn!");
        }

        private void SendTestData_Click(object sender, RoutedEventArgs e)
        {
            if(!this.bikeIsConnected)
            {
                SendTestBikeData();
                btn_SendTestData.IsEnabled = false;
                btn_SendTestData.Foreground = Brushes.Gray;
            }
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
                            string chatMessage = Encoding.UTF8.GetString(message.Content);
                            this.lastChatMessage = chatMessage;

                            txb_Chat.Text += "Dokter: " + chatMessage + Environment.NewLine;
                            txb_Chat.ScrollToEnd();

                            if (this.sceneManager != null)
                            {
                                Task.Run(async () =>
                                {
                                    await this.sceneManager.TextPanel.Swap();
                                    await this.sceneManager.TextPanel.Clear();
                                    await this.sceneManager.TextPanel.DrawText("Bericht dokter: ", new HealthcareServer.Vr.VectorMath.Vector2(10, 20), 20.0f, new HealthcareServer.Vr.VectorMath.Vector4(0, 0, 0, 1), "Arial");
                                    await this.sceneManager.TextPanel.DrawText(chatMessage, new HealthcareServer.Vr.VectorMath.Vector2(10, 50), 20.0f, new HealthcareServer.Vr.VectorMath.Vector4(0, 0, 0, 1), "Arial");
                                    await this.sceneManager.TextPanel.Swap();
                                });
                            }
                        break;
                        }
                    case Message.MessageType.CHANGE_RESISTANCE:
                        {
                            this.bike.SetResistence(message.Content[0]);
                            break;
                        }
                    case Message.MessageType.START_SESSION:
                        {
                            //lbl_Heartrate.Content = "";
                            //lbl_Distance.Content = "";
                            //lbl_Speed.Content = "";
                            //lbl_CycleRyhthm.Content = "";
                            //this.liveChartControl.GetLiveChart().Clear();

                            this.sessionInProgress = true;
                            break;
                        }
                    case Message.MessageType.STOP_SESSION:
                        {
                            //lbl_Heartrate.Content = "";
                            //lbl_Distance.Content = "";
                            //lbl_Speed.Content = "";
                            //lbl_CycleRyhthm.Content = "";
                            //this.liveChartControl.GetLiveChart().Clear();
                            if(this.sceneManager != null)
                                Task.Run(() => this.sceneManager.BikeNode.SetFollowSpeed(0.0f));

                            this.sessionInProgress = false;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }));
        }

        private void SendTestBikeData()
        {
            new Thread(() =>
            {
                Random random = new Random();

                while (true)
                {
                    Thread.Sleep(1000);
                    this.distance += 1;
                    ClientMessage clientMessage = new ClientMessage();
                    clientMessage.HasHeartbeat = true;
                    clientMessage.HasPage16 = true;
                    clientMessage.HasPage25 = true;
                    clientMessage.Heartbeat = (byte)random.Next(10, 100);
                    clientMessage.Distance = this.distance;
                    clientMessage.Speed = (byte)random.Next(1, 5);
                    clientMessage.Cadence = (byte)random.Next(10, 100);
                    HandleClientMessage(clientMessage);
                }
            }).Start();
        }

        public void HandleClientMessage(ClientMessage clientMessage)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if(this.sessionInProgress)
                {
                    this.healthCareClient.Transmit(new Message(false, Message.MessageType.BIKEDATA, clientMessage.GetData()));

                    if (clientMessage.HasHeartbeat)
                    {
                        lbl_Heartrate.Content = clientMessage.Heartbeat;
                        this.liveChartControl.GetLiveChart().Update(clientMessage.Heartbeat);
                    }
                    if (clientMessage.HasPage16)
                    {
                        lbl_Distance.Content = clientMessage.Distance;
                        lbl_Speed.Content = clientMessage.Speed;
                        if(this.sceneManager != null && this.sceneManager.BikeNode != null)
                            Task.Run(() => this.sceneManager.BikeNode.SetFollowSpeed(clientMessage.Speed));
                    }
                    if (clientMessage.HasPage25)
                    {
                        lbl_CycleRyhthm.Content = clientMessage.Cadence;
                    }
                }
            }));
        }
    }
}