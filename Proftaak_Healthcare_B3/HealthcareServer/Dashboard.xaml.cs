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
    public partial class Dashboard : Window, ILogger
    {
        private HealthCareServer healthcareServer;

        public Dashboard()
        {
            InitializeComponent();

            this.healthcareServer = new HealthCareServer("127.0.0.1", 1551, this);
            this.healthcareServer.Start();

            this.Loaded += Dashboard_Loaded;

            Authorizer.AddNewDoctorAuthorization("Test", HashUtil.HashSha256("test"), "Test");
        }

        private void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void Log(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                txb_Log.Text += message;
            }));
        }

        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            if((sender as Button).Content.ToString() == "Start")
            {
                if(!String.IsNullOrEmpty(txf_Ip.Value) && !String.IsNullOrEmpty(txf_Port.Value))
                {
                    this.healthcareServer = new HealthCareServer(txf_Ip.Value, int.Parse(txf_Port.Value), this);
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
    }
}
