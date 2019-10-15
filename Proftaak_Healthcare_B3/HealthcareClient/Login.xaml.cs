using HealthcareClient.Net;
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
using System.Windows.Threading;

namespace HealthcareClient
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window, IMessageReceiver
    {
        private HealthCareClient healthCareClient;

        public Login()
        {
            InitializeComponent();

            this.healthCareClient = new HealthCareClient("127.0.0.1", 1551, this);
            this.healthCareClient.Connect();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(txb_Name.Text) && !String.IsNullOrEmpty(txb_BSN.Text))
            {
                List<byte> bytes = new List<byte>();
                bytes.Add((byte)txb_BSN.Text.Length);
                bytes.AddRange(Encoding.UTF8.GetBytes(txb_BSN.Text));
                bytes.AddRange(Encoding.UTF8.GetBytes(txb_Name.Text));
                this.healthCareClient.Transmit(new Message(false, Message.MessageType.CLIENT_LOGIN, bytes.ToArray()));
            }
            else
            {
                lbl_Error.Content = "Velden BSN en Naam mogen niet leeg zijn!";
                lbl_Error.Visibility = Visibility.Visible;
            }
        }

        public void OnMessageReceived(Message message)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                Message.MessageType type = (Message.MessageType)message.Content[0];

                switch (message.messageType)
                {
                    case Message.MessageType.SERVER_OK:
                        {
                            if (type == Message.MessageType.CLIENT_LOGIN)
                            {
                                ClientWindow clientWindow = new ClientWindow(this.healthCareClient);
                                clientWindow.Show();
                                this.Close();
                            }
                            break;
                        }
                    case Message.MessageType.SERVER_ERROR:
                        {
                            if (type == Message.MessageType.CLIENT_LOGIN)
                            {
                                lbl_Error.Content = "Het is niet gelukt om in te loggen!";
                                lbl_Error.Visibility = Visibility.Visible;
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
    }
}
