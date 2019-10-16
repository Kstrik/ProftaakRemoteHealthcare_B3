using HealthcareDoctor;
using HealthcareDoctor.Net;
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
using System.Windows.Threading;

namespace HealthcareDoctor
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window, IMessageReceiver
    {
        private HealthCareDoctor healthCareDoctor;

        public Login()
        {
            InitializeComponent();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txb_Ip.Text) && !String.IsNullOrEmpty(txb_Port.Text))
            {
                this.healthCareDoctor = new HealthCareDoctor(txb_Ip.Text, int.Parse(txb_Port.Text), this);
                if (this.healthCareDoctor.Connect())
                {
                    stk_Connect.Visibility = Visibility.Collapsed;
                    stk_Login.Visibility = Visibility.Visible;
                }
                else
                {
                    this.healthCareDoctor = null;
                    lbl_ConnectError.Content = "Kon geen verbinden maken, geen connectie gevonden!";
                    lbl_ConnectError.Visibility = Visibility.Visible;
                }
            }
            else
            {
                lbl_ConnectError.Content = "Velden Ip en Poort mogen niet leeg zijn!";
                lbl_ConnectError.Visibility = Visibility.Visible;
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txb_Username.Text) && !String.IsNullOrEmpty(txb_Password.Password))
            {
                btn_Login.IsEnabled = false;
                SendLogin(txb_Username.Text, txb_Password.Password);
            }
            else
            {
                lbl_Error.Content = "Velden Gebruikersnaam en Wachtwoord mogen niet leeg zijn!";
                lbl_Error.Visibility = Visibility.Visible;
            }
        }

        public void SendLogin(string username, string password)
        {
            btn_Login.IsEnabled = false;
            List<byte> bytes = new List<byte>();

            bytes.AddRange(Encoding.UTF8.GetBytes(HashUtil.HashSha256(password)));
            bytes.AddRange(Encoding.UTF8.GetBytes(username));

            Message message = new Message(false, Message.MessageType.DOCTOR_LOGIN, bytes.ToArray());
            this.healthCareDoctor.Transmit(message);
        }

        public void OnMessageReceived(Message message)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                switch (message.messageType)
                {
                    case Message.MessageType.SERVER_OK:
                        {
                            Message.MessageType type = (Message.MessageType)message.Content[0];

                            if (type == Message.MessageType.DOCTOR_LOGIN)
                            {
                                MainWindow main = new MainWindow(this.healthCareDoctor);
                                main.Show();
                                this.Close();
                            }
                            break;
                        }
                    case Message.MessageType.SERVER_ERROR:
                        {
                            Message.MessageType type = (Message.MessageType)message.Content[0];

                            if (type == Message.MessageType.DOCTOR_LOGIN)
                            {
                                btn_Login.IsEnabled = true;
                                lbl_Error.Content = "Het is niet gelukt om in te loggen!";
                                lbl_Error.Visibility = Visibility.Visible;
                            }
                            break;
                        }
                    default:
                        break;
                }
            }));
        }
    }
}
