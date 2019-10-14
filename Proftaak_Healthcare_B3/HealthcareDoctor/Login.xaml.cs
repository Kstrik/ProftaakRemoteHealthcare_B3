using HealthcareDoctor;
using Networking.Client;
using Networking.HealthCare;
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

namespace HealthcareClient
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window, IServerDataReceiver
    {
        Client client;
        public Login()
        {
            InitializeComponent();
            client = new Client("localhost", 1337, null, null);
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(txtBName.Text) || string.IsNullOrEmpty(passBox.Password))
            {
                MessageBox.Show("Ongeldige invoer of leeg!");
            }
            else
            {
                SendLogin(txtBName.Text, passBox.Password);

                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
        }

        public void SendLogin(string username, string password)
        {
            byte[] usernameBytes = Encoding.UTF8.GetBytes(username.PadRight(16));
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password.PadRight(16));
            byte[] messageContent = new byte[32];
            Buffer.BlockCopy(usernameBytes, 0, messageContent, 0, 16);
            Buffer.BlockCopy(passwordBytes, 0, messageContent, 0, 16);
            Message message = new Message(true, Message.MessageType.DOCTOR_LOGIN, messageContent);

            this.client.Transmit(message.GetBytes());
        }

        public void OnDataReceived(byte[] data)
        {
            Message message = Message.ParseMessage(data);
        }
    }
}
