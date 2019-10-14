using HealthcareDoctor;
using Networking;
using Networking.Client;
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

namespace HealthcareDoctor
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window, IMessageReceiver
    {
        private HealthCareDoctor client;

        public Login()
        {
            InitializeComponent();

            this.client = new HealthCareDoctor("127.0.0.1", 1551, this);
            this.client.Connect();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txb_LoginUsername.Text) || string.IsNullOrEmpty(txb_LoginPassword.Password))
                MessageBox.Show("Ongeldige invoer of leeg!");
            else
                SendLogin(txb_LoginUsername.Text, txb_LoginPassword.Password);
        }

        public void SendLogin(string username, string password)
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(Encoding.UTF8.GetBytes(HashUtil.HashSha256(password)));
            bytes.AddRange(Encoding.UTF8.GetBytes(username));

            Message message = new Message(true, Message.MessageType.BIKEDATA, bytes.ToArray());
            this.client.Transmit(message);
        }

        public void OnMessageReceived(Message message)
        {
            switch (message.messageType)
            {
                case Message.MessageType.SERVER_ERROR:
                    {
                        Message.MessageType type = (Message.MessageType)message.Content[0];

                        if (type == Message.MessageType.DOCTOR_LOGIN)
                        {
                            MessageBox.Show("Fout tijdens login!");
                        }
                        break;
                    }
                case Message.MessageType.SERVER_OK:
                    {
                        Message.MessageType type = (Message.MessageType)message.Content[0];

                        if (type == Message.MessageType.DOCTOR_LOGIN)
                        {
                            MainWindow main = new MainWindow();
                            main.Show();
                            this.Close();
                        }
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
