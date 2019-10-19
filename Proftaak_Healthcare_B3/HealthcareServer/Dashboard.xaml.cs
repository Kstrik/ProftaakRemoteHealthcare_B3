using HealthcareServer.Files;
using HealthcareServer.Net;
using Networking;
using Networking.HealthCare;
using Networking.Server;
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

namespace HealthcareServer
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window, ILogger, IConnectionsDisplay
    {
        private HealthCareServer healthcareServer;

        public Dictionary<string, StackPanel> clients;
        public Dictionary<string, StackPanel> doctors;

        public Dashboard()
        {
            InitializeComponent();

            this.Loaded += Dashboard_Loaded;

            Authorizer.AddNewDoctorAuthorization("Test", HashUtil.HashSha256("test"), "Test");
            Authorizer.AddNewDoctorAuthorization("Testtest", HashUtil.HashSha256("testtest"), "Test");
            FileHandler.GetAllClientBSNS();

            this.Closed += Dashboard_Closed;

            this.clients = new Dictionary<string, StackPanel>();
            this.doctors = new Dictionary<string, StackPanel>();
        }

        private void Dashboard_Closed(object sender, EventArgs e)
        {
            this.healthcareServer.Stop();
            Environment.Exit(0);
        }

        private void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void Log(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                txb_Log.Text += message;
                txb_Log.ScrollToEnd();
            }));
        }

        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            if((sender as Button).Content.ToString() == "Start")
            {
                if(!String.IsNullOrEmpty(txf_Ip.Value) && !String.IsNullOrEmpty(txf_Port.Value))
                {
                    this.healthcareServer = new HealthCareServer(txf_Ip.Value, int.Parse(txf_Port.Value), this, this);
                    if (this.healthcareServer.Start())
                    {
                        txf_Ip.IsEnabled = false;
                        txf_Port.IsEnabled = false;
                        btn_StartStop.Content = "Stop";
                    }
                    else
                        Log("Kon de server niet starten!\n");
                }
                else
                    Log("De velden Ip en Poort mogen niet leeg zijn!\n");
            }
            else if ((sender as Button).Content.ToString() == "Stop")
            {
                txf_Ip.IsEnabled = true;
                txf_Port.IsEnabled = true;
                btn_StartStop.Content = "Start";
                this.healthcareServer.Stop();
            }
        }

        public void OnClientConnected(string name)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                StackPanel stackPanel = GetStackPanel("Cliënt: " + name);
                this.clients.Add(name, stackPanel);
                con_Connections.Children.Add(stackPanel);
            }));
        }

        public void OnDoctorConnected(string name)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                StackPanel stackPanel = GetStackPanel("Doctor: " + name);
                this.doctors.Add(name, stackPanel);
                con_Connections.Children.Add(stackPanel);
            }));
        }

        public void OnClientDisconnected(string name)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (this.clients.Keys.Contains(name))
                    con_Connections.Children.Remove(this.clients[name]);
            }));
        }

        public void OnDoctorDisconnected(string name)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (this.doctors.Keys.Contains(name))
                    con_Connections.Children.Remove(this.doctors[name]);
            }));
        }

        private StackPanel GetStackPanel(string text)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
            stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            stackPanel.VerticalAlignment = VerticalAlignment.Top;
            stackPanel.Margin = new Thickness(5, 5, 5, 0);

            Label nameLabel = new Label();
            nameLabel.Foreground = Brushes.White;
            nameLabel.Margin = new Thickness(5, 5, 5, 5);
            nameLabel.Content = text;

            stackPanel.Children.Add(nameLabel);

            return stackPanel;
        }
    }
}
