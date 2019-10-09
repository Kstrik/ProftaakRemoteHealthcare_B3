using HealthcareDoctor;
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
using Networking.Client;
using System.Runtime.Remoting.Messaging;

namespace HealthcareClient
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        Client TCPClient;
        public Login()
        {
            InitializeComponent();
            TCPClient = new Client("localhost", 80, null, null); //nulls waar in?
            //connectToServer(TCPClient, new AsyncCallback(connectedToServer));
            TCPClient.Connect();
        }

        
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBName.Text) || string.IsNullOrEmpty(passBox.Password)){
                MessageBox.Show("Ongeldige invoer of leeg!");
            }
            else if(false)
            {
                MessageBox.Show("Geen verbinding met server!");

            }
            else
            {
                DataManager dataManager = new DataManager(TCPClient);
                dataManager.SendLogin(txtBName.Text, passBox.Password);

                if (true)
                {
                    // Todo: implement login check
                }

                MainWindow main = new MainWindow(TCPClient);
                main.Show();
                this.Close();
            }
        }

       
    }
}
