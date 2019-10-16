using HealthcareDoctor.Net;
using HealthcareDoctor.UI;
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
        private List<string> clientBSNList;

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
            this.clientBSNList = new List<string>();

            //dispatcherTimer.Tick += new EventHandler(dt_Tick);
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            //dataManager = new DataManager();

            this.healthCareDoctor.Transmit(new Message(false, Message.MessageType.GET_CLIENTS, null));
        }

        private void SendChatMessage(string chatMessage)
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(Encoding.UTF8.GetBytes(HashUtil.HashSha256(chatMessage)));

            Message message = new Message(true, Message.MessageType.DOCTOR_LOGIN, bytes.ToArray());
            string encryptedMessage = DataEncryptor.Encrypt(Encoding.UTF8.GetString(message.GetBytes()), "Test");
            this.healthCareDoctor.Transmit(message);
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

        private void HandleAddClient(string bsn, string name)
        {
            Cliënt cliënt = new Cliënt(bsn, name, this.healthCareDoctor);
            this.clients.Add(cliënt);

            StackPanel stackpanel = new StackPanel();
            stackpanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
            stackpanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            stackpanel.VerticalAlignment = VerticalAlignment.Top;
            stackpanel.Margin = new Thickness(5, 5, 5, 0);

            Label nameLabel = new Label();
            nameLabel.Foreground = Brushes.White;
            nameLabel.Margin = new Thickness(5, 5, 5, 5);
            nameLabel.Content = name;

            Label bsnLabel = new Label();
            bsnLabel.Foreground = Brushes.White;
            bsnLabel.Margin = new Thickness(5, 0, 5, 5);
            bsnLabel.Content = bsn;

            stackpanel.Children.Add(nameLabel);
            stackpanel.Children.Add(bsnLabel);

            stackpanel.MouseDown += new MouseButtonEventHandler((object sender, MouseButtonEventArgs e) =>
            {
                this.selectedClient = cliënt;
                con_ClientData.Header = this.selectedClient.Name;
                con_ClientData.Children.Clear();
                con_ClientData.Children.Add(cliënt.ClientControl);
            });

            stackpanel.MouseEnter += new MouseEventHandler((object sender, MouseEventArgs e) =>
            {
                stackpanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC"));
            });

            stackpanel.MouseLeave += new MouseEventHandler((object sender, MouseEventArgs e) =>
            {
                stackpanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
            });

            con_ConnectedClients.Children.Add(stackpanel);
        }

        private void HandleRemoveClient(string bsn)
        {
            if(this.clients.Where(c => c.BSN == bsn).Count() != 0)
            {
                Cliënt cliënt = this.clients.Where(c => c.BSN == bsn).First();
                this.clients.Remove(cliënt);

                if (this.selectedClient == cliënt)
                    con_ClientData.Children.Clear();

                StackPanel removeStackPanel = null;
                foreach(StackPanel stackPanel in con_ConnectedClients.Children)
                {
                    if ((stackPanel.Children[0] as Label).Content.ToString() == bsn)
                    {
                        removeStackPanel = stackPanel;
                        break;
                    }
                }

                if (removeStackPanel != null)
                    con_ConnectedClients.Children.Remove(removeStackPanel);
            }
        }

        public void OnMessageReceived(Message message)
        {
            List<byte> bytes = new List<byte>(message.Content);

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                switch (message.messageType)
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
                    case Message.MessageType.CLIENT_LOGIN:
                        {
                            string bsn = Encoding.UTF8.GetString(bytes.GetRange(1, bytes[0]).ToArray());
                            string name = Encoding.UTF8.GetString(bytes.GetRange(bytes[0] + 1, bytes.Count() - (bytes[0] + 1)).ToArray());

                            if (this.clients.Where(c => c.BSN == bsn).Count() == 0)
                                HandleAddClient(bsn, name);

                            HandleRemoveClient(Encoding.UTF8.GetString(message.Content));
                            break;
                        }
                    case Message.MessageType.REMOVE_CLIENT:
                        {
                            HandleRemoveClient(Encoding.UTF8.GetString(message.Content));
                            break;
                        }
                    case Message.MessageType.BIKEDATA:
                        {
                            string bsn = Encoding.UTF8.GetString(bytes.GetRange(1, bytes[0]).ToArray());
                            string name = Encoding.UTF8.GetString(bytes.GetRange(bytes[0] + 2, bytes[bytes[0] + 1]).ToArray());

                            if (this.clients.Where(c => c.BSN == bsn).Count() == 0)
                                HandleAddClient(bsn, name);

                            Cliënt cliënt = this.clients.Where(c => c.BSN == bsn).First();
                            //cliënt.HandleBikeData(bytes.GetRange(bytes[0] + 1, bytes.Count - (bytes[0] + 1)));
                            cliënt.HandleBikeData(bytes.GetRange(bsn.Length + name.Length + 2, bytes.Count - (bsn.Length + name.Length + 2)));
                            break;
                        }
                    case Message.MessageType.CLIENT_DATA:
                        {
                            string bsn = Encoding.UTF8.GetString(bytes.GetRange(1, bytes[0]).ToArray());

                            if (!this.clientBSNList.Contains(bsn))
                            {
                                this.clientBSNList.Add(bsn);
                                cmf_BSN.Value = this.clientBSNList.ToArray();
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }));
        }

        private void HandleServerOk(Message message)
        {
            List<byte> bytes = new List<byte>(message.Content);

            switch ((Message.MessageType)message.Content[0])
            {
                case Message.MessageType.CLIENT_HISTORY_START:
                    {
                        string bsn = Encoding.UTF8.GetString(bytes.GetRange(2, bytes[1]).ToArray());
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
                            btn_GetHistory.IsEnabled = true;
                        }
                        break;
                    }
                case Message.MessageType.CLIENT_HISTORY_DATA:
                    {
                        string bsn = Encoding.UTF8.GetString(bytes.GetRange(2, bytes[1]).ToArray());
                        HandleHistoryData(bsn, bytes.GetRange(bytes[1] + 2, message.Content.Count() - (bytes[1] + 2)));
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
                        MessageBox.Show("Kon geschiedenis van cliënt niet ophalen!");
                        btn_GetHistory.IsEnabled = true;
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

        private void GetHistory_Click(object sender, RoutedEventArgs e)
        {
            if (cmf_BSN.SelectedValue != null)
            {
                btn_GetHistory.IsEnabled = false;
                this.healthCareDoctor.Transmit(new Message(false, Message.MessageType.GET_CLIENT_HISTORY, Encoding.UTF8.GetBytes((string)cmf_BSN.SelectedValue)));
            }
            else
                MessageBox.Show("Er is geen BSN geselecteerd!");
        }
    }
}
