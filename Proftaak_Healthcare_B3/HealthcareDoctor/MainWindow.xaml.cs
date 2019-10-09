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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static HealthcareDoctor.DataManager;
using Networking.Client;

namespace HealthcareDoctor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataManager dataManager;
        public MainWindow(Client TCPClient)
        {
            InitializeComponent();

            dataManager = new DataManager(TCPClient);

            foreach (TestClient client in dataManager.GetClients())
            {
                StackPanel canvas = new StackPanel();
                canvas.Background = (Brush)(new BrushConverter().ConvertFromString("#FF39437D"));   
                
                canvas.Width = 200;
                canvas.HorizontalAlignment = HorizontalAlignment.Left;
                canvas.VerticalAlignment = VerticalAlignment.Top;

                Label name = new Label();
                Label id = new Label();

                name.Foreground = Brushes.White;
                name.Margin = new Thickness(10, 10, 10, 1);

                id.Foreground = Brushes.White;
                id.Margin = new Thickness(10, 10, 10, 10);

                //name.Width = canvas.ActualWidth;
                //id.Width = canvas.ActualWidth;

                name.Content = "Naam: " + client.GetName();
                id.Content = "ID: " + client.GetId();

                canvas.Children.Add(name);
                canvas.Children.Add(id);

                clientConnectedStack.Children.Add(canvas);
            }
        }
    }
}
