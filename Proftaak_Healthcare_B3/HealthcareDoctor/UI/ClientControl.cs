using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using UIControls.Charts;
using UIControls.Fields;

namespace HealthcareDoctor.UI
{
    public delegate void SendResistanceEventHandler(int resistance, string bsn);
    public delegate void SendMessageEventHandler(string text, string bsn);
    public delegate void StartSessionEventHandler(string bsn);
    public delegate void StopSessionEventHandler(string bsn);

    public class ClientControl : ContentControl
    {
        public static readonly DependencyProperty HeartrateProperty = DependencyProperty.Register("Heartrate", typeof(int), typeof(ClientControl));
        public static readonly DependencyProperty DistanceProperty = DependencyProperty.Register("Distance", typeof(int), typeof(ClientControl));
        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(int), typeof(ClientControl));
        public static readonly DependencyProperty CycleRhythmProperty = DependencyProperty.Register("CycleRhythm", typeof(int), typeof(ClientControl));

        public int Heartrate
        {
            get { return (int)this.GetValue(HeartrateProperty); }
            set { this.SetValue(HeartrateProperty, value); }
        }

        public int Distance
        {
            get { return (int)this.GetValue(DistanceProperty); }
            set { this.SetValue(DistanceProperty, value); }
        }

        public int Speed
        {
            get { return (int)this.GetValue(SpeedProperty); }
            set { this.SetValue(SpeedProperty, value); }
        }

        public int CycleRhythm
        {
            get { return (int)this.GetValue(CycleRhythmProperty); }
            set { this.SetValue(CycleRhythmProperty, value); }
        }

        private Grid grid;

        private StackPanel detailsPanel;
        private Label heartrateLabel;
        private Label heartrateDisplay;
        private Label distanceLabel;
        private Label distanceDisplay;
        private Label speedLabel;
        private Label speedDisplay;
        private Label cycleRhythmLabel;
        private Label cycleRhythmDisplay;

        private Canvas canvas;
        private LiveChart liveChart;

        private TextField resistanceField;
        private Button sendResistanceButton;

        private TextField textField;
        private Button sendMessageButton;

        private Button toggleSessionButton;

        public SendResistanceEventHandler OnSendResistance;
        public SendMessageEventHandler OnSendMessage;
        public StartSessionEventHandler OnStartSession;
        public StopSessionEventHandler OnStopSession;
        public string BSN;

        public ClientControl()
        {
            this.DataContext = this;

            this.grid = new Grid();
            this.grid.ColumnDefinitions.Add(new ColumnDefinition());
            this.grid.ColumnDefinitions.Add(new ColumnDefinition());
            this.grid.RowDefinitions.Add(new RowDefinition());
            this.grid.RowDefinitions.Add(new RowDefinition());

            this.resistanceField = new TextField();
            this.resistanceField.FontSize = 12;
            this.resistanceField.Header = "Weerstand aanpassen:";
            this.resistanceField.Width = 200;
            this.resistanceField.HorizontalAlignment = HorizontalAlignment.Left;
            this.sendResistanceButton = new Button();
            this.sendResistanceButton.Content = "Stuur weerstand";
            this.sendResistanceButton.Height = 25;
            this.sendResistanceButton.BorderBrush = Brushes.Transparent;
            this.sendResistanceButton.Margin = new Thickness(5, 5, 5, 5);
            this.sendResistanceButton.Width = 200;
            this.sendResistanceButton.HorizontalAlignment = HorizontalAlignment.Left;

            this.textField = new TextField();
            this.textField.FontSize = 12;
            this.textField.Header = "Chat bericht:";
            this.sendMessageButton = new Button();
            this.sendMessageButton.Content = "Stuur bericht";
            this.sendMessageButton.Height = 25;
            this.sendMessageButton.BorderBrush = Brushes.Transparent;
            this.sendMessageButton.Margin = new Thickness(5, 5, 5, 5);

            this.toggleSessionButton = new Button();
            this.toggleSessionButton.Content = "Start Session";
            this.toggleSessionButton.Height = 30;
            this.toggleSessionButton.BorderBrush = Brushes.Transparent;
            this.toggleSessionButton.Margin = new Thickness(5, 0, 5, 5);

            SetupLabels();

            this.detailsPanel = new StackPanel();
            this.detailsPanel.Children.Add(this.heartrateLabel);
            this.detailsPanel.Children.Add(this.heartrateDisplay);
            this.detailsPanel.Children.Add(this.distanceLabel);
            this.detailsPanel.Children.Add(this.distanceDisplay);
            this.detailsPanel.Children.Add(this.speedLabel);
            this.detailsPanel.Children.Add(this.speedDisplay);
            this.detailsPanel.Children.Add(this.cycleRhythmLabel);
            this.detailsPanel.Children.Add(this.cycleRhythmDisplay);
            this.detailsPanel.Children.Add(this.resistanceField);
            this.detailsPanel.Children.Add(this.sendResistanceButton);
            this.detailsPanel.Children.Add(this.textField);
            this.detailsPanel.Children.Add(this.sendMessageButton);

            this.canvas = new Canvas();
            this.liveChart = new LiveChart("Hartslag", "", "", 40, 400, 200, 20, LiveChart.BlueGreenDarkTheme, this.canvas, true, true, true, true, false, false, true);

            this.grid.Children.Add(this.detailsPanel);
            this.grid.Children.Add(this.canvas);
            this.grid.Children.Add(this.toggleSessionButton);

            BindingOperations.SetBinding(this.heartrateDisplay, Label.ContentProperty, new Binding("Heartrate"));
            BindingOperations.SetBinding(this.distanceDisplay, Label.ContentProperty, new Binding("Distance"));
            BindingOperations.SetBinding(this.speedDisplay, Label.ContentProperty, new Binding("Speed"));
            BindingOperations.SetBinding(this.cycleRhythmDisplay, Label.ContentProperty, new Binding("CycleRhythm"));

            BindingOperations.SetBinding(this.resistanceField, TextField.HeaderForegroundProperty, new Binding("Foreground"));
            BindingOperations.SetBinding(this.resistanceField, TextField.ValueForegroundProperty, new Binding("Foreground"));
            BindingOperations.SetBinding(this.resistanceField, TextField.ValueBackgroundProperty, new Binding("Background"));
            BindingOperations.SetBinding(this.resistanceField, TextField.ValueBorderBrushProperty, new Binding("BorderBrush"));
            BindingOperations.SetBinding(this.textField, TextField.HeaderForegroundProperty, new Binding("Foreground"));
            BindingOperations.SetBinding(this.textField, TextField.ValueForegroundProperty, new Binding("Foreground"));
            BindingOperations.SetBinding(this.textField, TextField.ValueBackgroundProperty, new Binding("Background"));
            BindingOperations.SetBinding(this.textField, TextField.ValueBorderBrushProperty, new Binding("BorderBrush"));

            BindingOperations.SetBinding(this.sendResistanceButton, Button.ForegroundProperty, new Binding("Foreground"));
            BindingOperations.SetBinding(this.sendResistanceButton, Button.BackgroundProperty, new Binding("Background"));
            BindingOperations.SetBinding(this.sendMessageButton, Button.ForegroundProperty, new Binding("Foreground"));
            BindingOperations.SetBinding(this.sendMessageButton, Button.BackgroundProperty, new Binding("Background"));
            BindingOperations.SetBinding(this.toggleSessionButton, Button.ForegroundProperty, new Binding("Foreground"));
            BindingOperations.SetBinding(this.toggleSessionButton, Button.BackgroundProperty, new Binding("Background"));

            Grid.SetColumn(this.detailsPanel, 0);
            Grid.SetColumn(this.canvas, 1);
            Grid.SetColumn(this.toggleSessionButton, 0);
            Grid.SetRow(this.detailsPanel, 0);
            Grid.SetRow(this.canvas, 0);
            Grid.SetRow(this.toggleSessionButton, 1);
            Grid.SetColumnSpan(this.toggleSessionButton, 2);

            this.sendResistanceButton.Click += SendResistanceButton_Click;
            this.sendMessageButton.Click += SendMessageButton_Click;
            this.toggleSessionButton.Click += ToggleSessionButton_Click;

            this.Content = this.grid;
        }

        public ClientControl(SendResistanceEventHandler sendResistanceEventHandler, SendMessageEventHandler sendMessageEventHandler, StartSessionEventHandler startSessionEventHandler, StopSessionEventHandler stopSessionEventHandler, string bsn)
            : this()
        {
            this.OnSendResistance += sendResistanceEventHandler;
            this.OnSendMessage += sendMessageEventHandler;
            this.OnStartSession = startSessionEventHandler;
            this.OnStopSession += stopSessionEventHandler;
            this.BSN = bsn;
        }

        private void SendResistanceButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.resistanceField.Value != null && this.BSN != null)
            {
                int resistance = int.Parse(this.resistanceField.Value);

                if(resistance >= 0 && resistance <= 100)
                    this.OnSendResistance(resistance, this.BSN);
                else
                    MessageBox.Show("Weerstand mag alleen tusseen 0 en 100 zitten!");
            }
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.textField.Value != null && this.BSN != null)
            {
                this.OnSendMessage(this.textField.Value, this.BSN);
                this.textField.Value = "";
            }
        }

        private void ToggleSessionButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.BSN != null)
            {
                if ((sender as Button).Content.ToString().StartsWith("Start"))
                {
                    this.toggleSessionButton.Content = "Stop Session";
                    this.OnStartSession(this.BSN);
                }
                else
                {
                    this.toggleSessionButton.Content = "Start Session";
                    this.OnStopSession(this.BSN);
                }
            }
        }

        private void SetupLabels()
        {
            this.heartrateLabel = new Label();
            this.heartrateLabel.FontSize = 12;
            this.heartrateLabel.Margin = new Thickness(5, 5, 0, 0);
            this.heartrateLabel.Content = "Hartslag:";
            BindingOperations.SetBinding(this.heartrateLabel, Label.ForegroundProperty, new Binding("Foreground"));

            this.heartrateDisplay = new Label();
            this.heartrateDisplay.FontSize = 12;
            this.heartrateDisplay.Margin = new Thickness(5, 5, 0, 0);
            BindingOperations.SetBinding(this.heartrateDisplay, Label.ForegroundProperty, new Binding("Foreground"));

            this.distanceLabel = new Label();
            this.distanceLabel.FontSize = 12;
            this.distanceLabel.Margin = new Thickness(5, 5, 0, 0);
            this.distanceLabel.Content = "Afstand:";
            BindingOperations.SetBinding(this.distanceLabel, Label.ForegroundProperty, new Binding("Foreground"));

            this.distanceDisplay = new Label();
            this.distanceDisplay.FontSize = 12;
            this.distanceDisplay.Margin = new Thickness(5, 5, 0, 0);
            BindingOperations.SetBinding(this.distanceDisplay, Label.ForegroundProperty, new Binding("Foreground"));

            this.speedLabel = new Label();
            this.speedLabel.FontSize = 12;
            this.speedLabel.Margin = new Thickness(5, 5, 0, 0);
            this.speedLabel.Content = "Snelheid:";
            BindingOperations.SetBinding(this.speedLabel, Label.ForegroundProperty, new Binding("Foreground"));

            this.speedDisplay = new Label();
            this.speedDisplay.FontSize = 12;
            this.speedDisplay.Margin = new Thickness(5, 5, 0, 0);
            BindingOperations.SetBinding(this.speedDisplay, Label.ForegroundProperty, new Binding("Foreground"));

            this.cycleRhythmLabel = new Label();
            this.cycleRhythmLabel.FontSize = 12;
            this.cycleRhythmLabel.Margin = new Thickness(5, 5, 0, 0);
            this.cycleRhythmLabel.Content = "Rotaties per minuut:";
            BindingOperations.SetBinding(this.cycleRhythmLabel, Label.ForegroundProperty, new Binding("Foreground"));

            this.cycleRhythmDisplay = new Label();
            this.cycleRhythmDisplay.FontSize = 12;
            this.cycleRhythmDisplay.Margin = new Thickness(5, 5, 0, 0);
            BindingOperations.SetBinding(this.cycleRhythmDisplay, Label.ForegroundProperty, new Binding("Foreground"));
        }

        public void UpdateChart(double value)
        {
            this.liveChart.Update(value);
        }

        public Button GetToggleButton()
        {
            return this.toggleSessionButton;
        }
    }
}
