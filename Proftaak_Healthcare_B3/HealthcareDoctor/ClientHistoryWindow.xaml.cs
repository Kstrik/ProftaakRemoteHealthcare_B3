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
using UIControls.Charts;

namespace HealthcareDoctor
{
    /// <summary>
    /// Interaction logic for ClientHistoryWindow.xaml
    /// </summary>
    public partial class ClientHistoryWindow : Window
    {
        private LiveChartControl heartrateChart;
        private LiveChartControl distanceChart;
        private LiveChartControl speedChart;
        private LiveChartControl cycleRhythmChart;

        private HistoryData historyData;

        private int maxIntervals;

        public ClientHistoryWindow(string bsn, int maxIntervals)
        {
            InitializeComponent();

            this.historyData = new HistoryData();
            this.maxIntervals = maxIntervals;
            con_ClientData.Header = $"Cliënt data [{bsn}]";

            this.heartrateChart = new LiveChartControl("Hartslag", "", "", 40, 400, 200, this.maxIntervals, LiveChart.BlueGreenDarkTheme, true, true, true, true, false, false, true);
            this.distanceChart = new LiveChartControl("Afstand", "", "", 40, 400, 200, this.maxIntervals, LiveChart.BlueGreenDarkTheme, true, true, true, true, false, false, true);
            this.speedChart = new LiveChartControl("Snelheid", "", "", 40, 400, 200, this.maxIntervals, LiveChart.BlueGreenDarkTheme, true, true, true, true, false, false, true);
            this.cycleRhythmChart = new LiveChartControl("Rotaties per minuut", "", "", 40, 400, 200, this.maxIntervals, LiveChart.BlueGreenDarkTheme, true, true, true, true, false, false, true);

            grd_Grid.Children.Add(this.heartrateChart);
            grd_Grid.Children.Add(this.distanceChart);
            grd_Grid.Children.Add(this.speedChart);
            grd_Grid.Children.Add(this.cycleRhythmChart);

            Grid.SetColumn(this.heartrateChart, 0);
            Grid.SetColumn(this.distanceChart, 1);
            Grid.SetColumn(this.speedChart, 0);
            Grid.SetColumn(this.cycleRhythmChart, 1);
            Grid.SetRow(this.heartrateChart, 0);
            Grid.SetRow(this.distanceChart, 0);
            Grid.SetRow(this.speedChart, 1);
            Grid.SetRow(this.cycleRhythmChart, 1);
        }

        public void ProcessHistoryData()
        {
            List<(LiveChartControl, List<(int value, DateTime time)>)> dataHisory = new List<(LiveChartControl, List<(int value, DateTime time)>)>()
            {
                (this.heartrateChart, this.historyData.HeartrateValues),
                (this.distanceChart, this.historyData.DistanceValues),
                (this.speedChart, this.historyData.SpeedValues),
                (this.cycleRhythmChart, this.historyData.CycleRhythmValues)
            };

            foreach((LiveChartControl livechart, List<(int value, DateTime time)> history) data in dataHisory)
            {
                int stepSize = (data.history.Count() >= this.maxIntervals) ? data.history.Count() / this.maxIntervals : 1;
                int stepAmount = data.history.Count() / stepSize;

                for (int i = 0; i < data.history.Count; i += stepSize)
                {
                    if(data.history.Count > (i + stepAmount))
                        data.livechart.GetLiveChart().Update(GetAverage(data.history.GetRange(i, stepAmount)));
                    else
                        data.livechart.GetLiveChart().Update(GetAverage(data.history.GetRange(i, data.history.Count - i)));
                }
            }
        }

        //private void ProcessHeartrateHistory()
        //{
        //    int stepSize = this.historyData.HeartrateValues.Count() / this.maxIntervals;
        //    int stepAmount = this.historyData.HeartrateValues.Count() / stepSize;

        //    for (int i = 0; i < this.historyData.HeartrateValues.Count; i += stepSize)
        //        this.heartrateChart.GetLiveChart().Update(GetAverage(this.historyData.HeartrateValues.GetRange(i, stepAmount)));
        //}

        //private void ProcessDistanceHistory()
        //{
        //    int stepSize = this.historyData.DistanceValues.Count() / this.maxIntervals;
        //    int stepAmount = this.historyData.DistanceValues.Count() / stepSize;

        //    for (int i = 0; i < this.historyData.DistanceValues.Count; i += stepSize)
        //        this.distanceChart.GetLiveChart().Update(GetAverage(this.historyData.DistanceValues.GetRange(i, stepAmount)));
        //}

        //private void ProcessSpeedHistory()
        //{
        //    int stepSize = this.historyData.SpeedValues.Count() / this.maxIntervals;
        //    int stepAmount = this.historyData.SpeedValues.Count() / stepSize;

        //    for (int i = 0; i < this.historyData.SpeedValues.Count; i += stepSize)
        //        this.speedChart.GetLiveChart().Update(GetAverage(this.historyData.SpeedValues.GetRange(i, stepAmount)));
        //}

        //private void ProcessCycleRyhthmHistory()
        //{
        //    int stepSize = this.historyData.CycleRhythmValues.Count() / this.maxIntervals;
        //    int stepAmount = this.historyData.CycleRhythmValues.Count() / stepSize;

        //    for (int i = 0; i < this.historyData.CycleRhythmValues.Count; i += stepSize)
        //        this.cycleRhythmChart.GetLiveChart().Update(GetAverage(this.historyData.CycleRhythmValues.GetRange(i, stepAmount)));
        //}

        private double GetAverage(List<(int value, DateTime time)> values)
        {
            double total = 0;
            foreach((int value, DateTime time) item in values)
                total += item.value;

            return total / values.Count;
        }

        public void AddHeartRate((int value, DateTime time) heartrate)
        {
            historyData.HeartrateValues.Add(heartrate);
        }

        public void AddDistance((int value, DateTime time) distance)
        {
            historyData.DistanceValues.Add(distance);
        }

        public void AddSpeed((int value, DateTime time) speed)
        {
            historyData.SpeedValues.Add(speed);
        }

        public void AddCycleRyhthm((int value, DateTime time) cycleRhythm)
        {
            historyData.CycleRhythmValues.Add(cycleRhythm);
        }
    }
}
