using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public partial class Dashboard : Window, ILog
    {
        private HealthCareServer healthcareServer;

        public Dashboard()
        {
            InitializeComponent();
            this.healthcareServer = new HealthCareServer("localhost", 80, this); ;
            List<string> test = new List<string>() { "Test1", "Test2", "Test3", "Test4" };
            this.Loaded += Dashboard_Loaded;
        }

        private void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            sdm_Sidemenu.SetupMenu();
            sdm_Sidemenu.Menu.AddItem("Test", 250, 60, null);
        }

        public void Log(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                //txbLog.Text = message;
            }));
        }
    }
}
