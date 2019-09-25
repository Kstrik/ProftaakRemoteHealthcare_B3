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

namespace HealthcareClient
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBName.Text) || string.IsNullOrEmpty(passBox.Password)){
                MessageBox.Show("Ongeldige invoer of leeg!");
            }
            else
            {
                DataManager dataManager = new DataManager();
                dataManager.SendLogin(txtBName.Text, passBox.Password);

                if (true)
                {

                }

                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
        }
    }
}
