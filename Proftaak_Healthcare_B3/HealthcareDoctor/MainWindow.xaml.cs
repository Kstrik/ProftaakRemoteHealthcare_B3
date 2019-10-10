using Networking.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UIControls.Charts;
using static HealthcareDoctor.DataManager;

namespace HealthcareDoctor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Doctor doctor;
        DataManager dataManager;
        TestClient TestClient;

        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();
        string currentTime = string.Empty;
        Label clock = new Label();
        public MainWindow()
        {
            InitializeComponent();

            dispatcherTimer.Tick += new EventHandler(dt_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dataManager = new DataManager();

            foreach (TestClient client in dataManager.GetClients())
            {
                StackPanel stackpanel = new StackPanel();
                stackpanel.Background = (Brush)(new BrushConverter().ConvertFromString("#FF39437D"));

                stackpanel.Width = 300;
                stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
                stackpanel.VerticalAlignment = VerticalAlignment.Top;

                Label name = new Label();
                Label id = new Label();

                name.Foreground = Brushes.White;
                name.Margin = new Thickness(10, 10, 10, 1);

                id.Foreground = Brushes.White;
                id.Margin = new Thickness(10, 10, 10, 10);

                

                name.Content = "Naam: " + client.GetName();
                id.Content = "ID: " + client.GetId();

                stackpanel.MouseDown += Canvas_MouseDown;

                //grid.Name = client.GetId().ToString();
                //grid.HorizontalAlignment = HorizontalAlignment.Center;
                //grid.VerticalAlignment = VerticalAlignment.Stretch;

                //grid.Width = clientDataGrid.ActualWidth;
                //grid.Height = clientDataGrid.ActualWidth;
                //clientDataPanel.Width = clientDataGrid.ActualWidth;
                //clientDataPanel.Height = clientDataGrid.ActualHeight;


                stackpanel.Children.Add(name);
                stackpanel.Children.Add(id);

                
                clientConnectedStack.Children.Add(stackpanel);     
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            clientDataPanel.Children.Clear();

            LiveChartControl heartrate = new LiveChartControl("Hartslag", "", "", 40, 400, 200, 20, LiveChart.BlueGreenTheme, false, true, true,
                                                                            true, false, false, true);

            ToggleButton btnStartStop = new ToggleButton();
            btnStartStop.Content = "Start/Stop Training";
            btnStartStop.HorizontalAlignment = HorizontalAlignment.Center;
            btnStartStop.VerticalAlignment = VerticalAlignment.Top;
            btnStartStop.Margin = new Thickness(0, 20, 0, 10);
            btnStartStop.Click += BtnStartStop_Click;

            clock.Foreground = Brushes.White;
            clock.HorizontalContentAlignment = HorizontalAlignment.Right;



            Label label = new Label();
            label.Foreground = Brushes.White;
            label.Margin = new Thickness(0, 0, 0, 10);
            label.Content = ((sender as StackPanel).Children[0] as Label).Content;
            clientDataPanel.Children.Add(btnStartStop);
            clientDataPanel.Children.Add(clock);
            clientDataPanel.Children.Add(heartrate);
            clientDataPanel.Children.Add(label);
        }

        private void BtnStartStop_Click(object sender, RoutedEventArgs e)
        {
            
            if ((sender as ToggleButton).IsChecked == true)
            {
                stopWatch.Reset();
                clock.Content = "00:00:00";
                stopWatch.Start();
                dispatcherTimer.Start();
            }
            else
            {
                stopWatch.Stop();
            }
        }

        void dt_Tick(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                TimeSpan ts = stopWatch.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}",
                ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                clock.Content = currentTime;
            }
        }
    }
}
