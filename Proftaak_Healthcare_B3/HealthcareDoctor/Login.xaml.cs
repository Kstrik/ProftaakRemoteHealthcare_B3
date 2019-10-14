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
            client = new Client("83.82.9.9", 25575, this, null);

            client.Connect();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(txb_LoginUsername.Text) || string.IsNullOrEmpty(txb_LoginPassword.Password))
            {
                MessageBox.Show("Ongeldige invoer of leeg!");
            }
            else
            {
                SendLogin(txb_LoginUsername.Text, txb_LoginPassword.Password);
            }
        }

        public void SendLogin(string username, string password)
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(Encoding.UTF8.GetBytes(HashUtil.HashSha256(password)));
            bytes.AddRange(Encoding.UTF8.GetBytes(username));

            Message message = new Message(true, Message.MessageType.DOCTOR_LOGIN, bytes.ToArray());
            string encryptedMessage = DataEncryptor.Encrypt(Encoding.UTF8.GetString(message.GetBytes()), "Test");
            this.client.Transmit(Encoding.UTF8.GetBytes(encryptedMessage));
        }

        public void OnDataReceived(byte[] data)
        {
            byte[] decryptedData = Encoding.UTF8.GetBytes(DataEncryptor.Decrypt(Encoding.UTF8.GetString(data), "Test"));
            Message message = Message.ParseMessage(decryptedData);

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
