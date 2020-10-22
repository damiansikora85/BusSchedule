using BusSchedule.Core.Model;
using BusSchedule.Creator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace BusSchedule.Creator.Dialogs
{
    /// <summary>
    /// Interaction logic for TimeAdjustmentWindow.xaml
    /// </summary>
    public partial class TimeAdjustmentWindow : Window
    {
        public List<RouteBeginTime> Times { get; }
        public ObservableCollection<TimeAdjustmentViewModel> TimeAdjustments { get; }
        private RouteStationViewModel _station;

        public TimeAdjustmentWindow(TimeSpan defaultTimeShift, RouteStationViewModel station, List<RouteBeginTime> beginTimes)
        {
            TimeAdjustments = new ObservableCollection<TimeAdjustmentViewModel>();
            Times = new List<RouteBeginTime>();
            foreach(var beginTime in beginTimes)
            {
                var timeForStation = new RouteBeginTime(beginTime);
                timeForStation.Time += defaultTimeShift;
                Times.Add(timeForStation);
            }
//            Times.ForEach(item => item.Time += defaultTimeShift);

            _station = station;
            InitializeComponent();
            DataContext = this;
        }

        public IEnumerable<TimeAdjustmentViewModel> GetResult()
        {
            return TimeAdjustments;
        }

        private void AddAdjustmentClick(object sender, RoutedEventArgs e)
        {
            if(DefaultTimes.SelectedItem != null && int.TryParse(AdjustmentEntry.Text, out var adjustmentValue))
            {
                TimeAdjustments.Add(new TimeAdjustmentViewModel(DefaultTimes.SelectedItem as RouteBeginTime, TimeSpan.FromMinutes(adjustmentValue), _station.BusStation.Id));
            }
            AdjustmentEntry.Text = "";
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
