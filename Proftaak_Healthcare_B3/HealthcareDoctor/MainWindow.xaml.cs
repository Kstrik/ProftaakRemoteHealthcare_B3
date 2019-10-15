using HealthcareDoctor.Net;
using Networking;
using Networking.Client;
using Networking.HealthCare;
using Networking.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UIControls.Charts;

namespace HealthcareDoctor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMessageReceiver
    {
        private HealthCareDoctor healthCareDoctor;
        private List<Cliënt> clients;

        //DataManager dataManager;
        //TestClient TestClient;

        //DispatcherTimer dispatcherTimer = new DispatcherTimer();
        //Stopwatch stopWatch = new Stopwatch();
        //string currentTime = string.Empty;
        //Label clock = new Label();

        private Cliënt selectedClient;
        private ClientHistoryWindow clientHistoryWindow;

        public MainWindow(HealthCareDoctor healthCareDoctor)
        {
            InitializeComponent();

            this.healthCareDoctor = healthCareDoctor;
            this.healthCareDoctor.SetReciever(this);

            this.clients = new List<Cliënt>();

            //dispatcherTimer.Tick += new EventHandler(dt_Tick);
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            //dataManager = new DataManager();

            //foreach (TestClient client in dataManager.GetClients())
            //{
            //    StackPanel stackpanel = new StackPanel();
            //    stackpanel.Background = (Brush)(new BrushConverter().ConvertFromString("#FF39437D"));
            //    stackpanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            //    stackpanel.VerticalAlignment = VerticalAlignment.Top;

            //    Label name = new Label();
            //    name.Foreground = Brushes.White;
            //    name.Margin = new Thickness(10, 10, 10, 1);
            //    name.Content = "Naam: " + client.GetName();

            //    Label id = new Label();
            //    id.Foreground = Brushes.White;
            //    id.Margin = new Thickness(10, 10, 10, 10);
            //    id.Content = "ID: " + client.GetId();

            //    stackpanel.MouseDown += Cliënt_MouseDown;

            //    stackpanel.Children.Add(name);
            //    stackpanel.Children.Add(id);
                
            //    clientConnectedStack.Children.Add(stackpanel);     
            //}
        }

        private void SendChatMessage(string chatMessage)
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(Encoding.UTF8.GetBytes(HashUtil.HashSha256(chatMessage)));

            Message message = new Message(true, Message.MessageType.DOCTOR_LOGIN, bytes.ToArray());
            string encryptedMessage = DataEncryptor.Encrypt(Encoding.UTF8.GetString(message.GetBytes()), "Test");
            this.healthCareDoctor.Transmit(message);
        }

        private void Cliënt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            stk_ClientData.Children.Clear();


        }

        //private void BtnStartStop_Click(object sender, RoutedEventArgs e)
        //{

        //    if ((sender as ToggleButton).IsChecked == true)
        //    {
        //        stopWatch.Reset();
        //        clock.Content = "00:00:00";
        //        stopWatch.Start();
        //        dispatcherTimer.Start();
        //    }
        //    else
        //    {
        //        stopWatch.Stop();
        //    }
        //}

        //void dt_Tick(object sender, EventArgs e)
        //{
        //    if (stopWatch.IsRunning)
        //    {
        //        TimeSpan ts = stopWatch.Elapsed;
        //        currentTime = String.Format("{0:00}:{1:00}:{2:00}",
        //        ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        //        clock.Content = currentTime;
        //    }
        //}

        private void HandleAddClient(string bsn)
        {
            Cliënt cliënt = new Cliënt(bsn, this.healthCareDoctor);
            this.clients.Add(cliënt);

            StackPanel stackpanel = new StackPanel();
            stackpanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
            stackpanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            stackpanel.VerticalAlignment = VerticalAlignment.Top;

            Label bsnLabel = new Label();
            bsnLabel.Foreground = Brushes.White;
            bsnLabel.Margin = new Thickness(10, 10, 10, 1);
            bsnLabel.Content = bsn;

            stackpanel.MouseDown += new MouseButtonEventHandler((object sender, MouseButtonEventArgs e) =>
            {
                if (bsnLabel.Content.ToString() == bsn)
                {
                    this.selectedClient = cliënt;

                    stackpanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC"));
                    bsnLabel.Content = $"[{bsn}]";

                    foreach (StackPanel panel in stk_ConnectedClients.Children)
                    {
                        panel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC"));
                        (panel.Children[0] as Label).Content = (panel.Children[0] as Label).Content.ToString().Trim(new char[2] { '[', ']' });
                    }

                    stk_ClientData.Children.Clear();
                    stk_ClientData.Children.Add(cliënt.ClientControl);
                }
                else if (bsnLabel.Content.ToString() == $"[{bsn}]")
                {
                    stackpanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC"));
                    bsnLabel.Content = bsn;
                }
            });

            stk_ConnectedClients.Children.Add(stackpanel);
        }

        private void HandleRemoveClient(string bsn)
        {
            if(this.clients.Where(c => c.BSN == bsn).Count() != 0)
            {
                Cliënt cliënt = this.clients.Where(c => c.BSN == bsn).First();
                this.clients.Remove(cliënt);

                if (this.selectedClient == cliënt)
                    stk_ClientData.Children.Clear();

                StackPanel removeStackPanel = null;
                foreach(StackPanel stackPanel in stk_ConnectedClients.Children)
                {
                    if ((stackPanel.Children[0] as Label).Content.ToString() == bsn)
                    {
                        removeStackPanel = stackPanel;
                        break;
                    }
                }

                if (removeStackPanel != null)
                    stk_ConnectedClients.Children.Remove(removeStackPanel);
            }
        }

        public void OnMessageReceived(Message message)
        {
            switch(message.messageType)
            {
                case Message.MessageType.SERVER_OK:
                    {
                        HandleServerOk(message);
                        break;
                    }
                case Message.MessageType.SERVER_ERROR:
                    {
                        HandleServerError(message);
                        break;
                    }
                case Message.MessageType.REMOVE_CLIENT:
                    {
                        HandleRemoveClient(Encoding.UTF8.GetString(message.Content));
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void HandleServerOk(Message message)
        {
            List<byte> bytes = new List<byte>(message.Content);

            switch ((Message.MessageType)message.Content[0])
            {
                case Message.MessageType.BIKEDATA:
                    {
                        string bsn = Encoding.UTF8.GetString(bytes.GetRange(1, bytes[0]).ToArray());

                        if (this.clients.Where(c => c.BSN == bsn).Count() == 0)
                            HandleAddClient(bsn);

                        Cliënt cliënt = this.clients.Where(c => c.BSN == bsn).First();
                        cliënt.HandleBikeData(bytes.GetRange(bytes[0] + 1, bytes.Count - (bytes[0] + 1)));
                        break;
                    }
                case Message.MessageType.CLIENT_HISTORY_START:
                    {
                        string bsn = Encoding.UTF8.GetString(message.Content);
                        this.clientHistoryWindow = new ClientHistoryWindow(bsn, 20);
                        break;
                    }
                case Message.MessageType.CLIENT_HISTORY_END:
                    {
                        if(this.clientHistoryWindow != null)
                        {
                            this.clientHistoryWindow.ProcessHistoryData();
                            this.clientHistoryWindow.Show();
                            this.clientHistoryWindow = null;
                        }
                        break;
                    }
                case Message.MessageType.CLIENT_HISTORY_DATA:
                    {
                        string bsn = Encoding.UTF8.GetString(bytes.GetRange(1, bytes[0]).ToArray());
                        HandleHistoryData(bsn, bytes.GetRange(bytes[0] + 1, message.Content.Count() - (bytes[0] + 1)));
                        break;
                    }
                case Message.MessageType.START_SESSION:
                    {
                        string bsn = Encoding.UTF8.GetString(bytes.GetRange(1, bytes[0]).ToArray());
                        Cliënt cliënt = this.clients.Where(c => c.BSN == bsn).First();
                        cliënt.StartSessionOK();
                        break;
                    }
                case Message.MessageType.STOP_SESSION:
                    {
                        string bsn = Encoding.UTF8.GetString(bytes.GetRange(1, bytes[0]).ToArray());
                        Cliënt cliënt = this.clients.Where(c => c.BSN == bsn).First();
                        cliënt.StopSessionOk();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void HandleServerError(Message message)
        {
            List<byte> bytes = new List<byte>(message.Content);

            switch ((Message.MessageType)message.Content[0])
            {
                case Message.MessageType.GET_CLIENT_HISTORY:
                    {
                        MessageBox.Show("Could not get history of cliënt!");
                        break;
                    }
                case Message.MessageType.START_SESSION:
                    {
                        string bsn = Encoding.UTF8.GetString(bytes.GetRange(1, bytes[0]).ToArray());
                        Cliënt cliënt = this.clients.Where(c => c.BSN == bsn).First();
                        cliënt.StartSessionError();
                        break;
                    }
                case Message.MessageType.STOP_SESSION:
                    {
                        string bsn = Encoding.UTF8.GetString(bytes.GetRange(1, bytes[0]).ToArray());
                        Cliënt cliënt = this.clients.Where(c => c.BSN == bsn).First();
                        cliënt.StopSessionError();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void HandleHistoryData(string bsn, List<byte> bytes)
        {
            if(this.clientHistoryWindow != null)
            {
                for (int i = 0; i < bytes.Count(); i += 21)
                {
                    int value = bytes[i + 1];
                    DateTime time = DateTime.Parse(Encoding.UTF8.GetString(bytes.GetRange(i + 2, 19).ToArray()));

                    switch ((Message.ValueId)bytes[i])
                    {
                        case Message.ValueId.HEARTRATE:
                            {
                                this.clientHistoryWindow.AddHeartRate((value, time));
                                break;
                            }
                        case Message.ValueId.DISTANCE:
                            {
                                this.clientHistoryWindow.AddDistance((value, time));
                                break;
                            }
                        case Message.ValueId.SPEED:
                            {
                                this.clientHistoryWindow.AddSpeed((value, time));
                                break;
                            }
                        case Message.ValueId.CYCLE_RHYTHM:
                            {
                                this.clientHistoryWindow.AddCycleRyhthm((value, time));
                                break;
                            }
                    }
                }
            }
        }
    }
}
