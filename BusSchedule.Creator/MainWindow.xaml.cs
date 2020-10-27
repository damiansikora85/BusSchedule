using BusSchedule.Core.Model;
using BusSchedule.Creator.Dialogs;
using BusSchedule.Creator.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace BusSchedule.Creator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _viewModel.Setup(File.ReadAllText(openFileDialog.FileName));
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            if(saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, _viewModel.GetScheduleDataString());
            }
        }

        private void AddBusService_Click(object sender, RoutedEventArgs e)
        {
            var addBusServiceDialog = new AddBusServiceWindow();
            addBusServiceDialog.ShowDialog();
            //_viewModel.AddBusService();
        }

        private void RemoveBusService_Click(object sender, RoutedEventArgs e)
        {
            if(BusServicesList.SelectedItem != null && BusServicesList.SelectedItem is BusService busService)
            {
                _viewModel.RemoveBusService(busService);
            }
        }

        private void AddBusStation_Click(object sender, RoutedEventArgs e)
        {
            var addStationDialog = new AddBusStationDialog();
            addStationDialog.Setup(BusStationsList.Items.Count+1);
            addStationDialog.ShowDialog();
            var newStation = addStationDialog.GetResult();
            if(newStation != null)
            {
                if(!_viewModel.AddBusStation(newStation))
                {
                    MessageBox.Show("Przystanek o podanej nazwie już istnieje", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RemoveBusStation_Click(object sender, RoutedEventArgs e)
        {
            if(BusStationsList.SelectedItem != null && BusStationsList.SelectedItem is BusStation busStation)
            {
                _viewModel.RemoveBusStation(busStation);
            }
        }

        private void AddRoute_Click(object sender, RoutedEventArgs e)
        {
            if (BusServicesList.SelectedItem != null && BusServicesList.SelectedItem is BusService busService)
            {
                var dialog = new AddRouteDialog(busService.Name, _viewModel.BusStations.ToList());
                dialog.ShowDialog();
                var newRoute = dialog.GetResult();
                if(newRoute != null)
                {
                    newRoute.BusServiceId = busService.Id;
                    _viewModel.AddRoute(newRoute);
                }
            }
        }

        private void RemoveRoute_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BusServiceChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BusServicesList.SelectedItem != null && BusServicesList.SelectedItem is BusService busService)
            {
                _viewModel.OnBusServiceChanged(busService.Id);
            }
        }

        private void EditRoute_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditRouteDialog(((BusRoute)RoutesList.SelectedItem).Id, (BusService)BusServicesList.SelectedItem, _viewModel.BusStations, _viewModel.RouteDetailsForRoute, RouteVariants.SelectedIndex);
            dialog.ShowDialog();
            var result = dialog.GetResult();
            _viewModel.AddRouteDetails(result);
        }

        private void RouteChanged(object sender, SelectionChangedEventArgs e)
        {
            if(RoutesList.SelectedItem != null && RoutesList.SelectedItem is BusRoute route)
            {
                _viewModel.OnRouteChanged(route, GetSelectedScheduleDays(), 0);
                RouteVariants.SelectedIndex = 0;
            }
        }

        private void RouteVariants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoutesList.SelectedItem != null && RoutesList.SelectedItem is BusRoute route)
            {
                _viewModel.OnRouteChanged(route, GetSelectedScheduleDays(), RouteVariants.SelectedIndex);
            }
        }

        private void EditBeginTime_Click(object sender, RoutedEventArgs e)
        {
            if (RoutesList.SelectedItem != null && RoutesList.SelectedItem is BusRoute route)
            {
                var dialog = new EditBeginTimesDialog(_viewModel.BeginTimesForRoute.Select(item => item.Time));
                try
                {
                    dialog.ShowDialog();
                }
                catch(Exception exc)
                {
                    var msg = exc.Message;
                }
                var result = dialog.GetResult();
                var scheduleDays = GetSelectedScheduleDays();
                var id = 0;
                foreach (var time in result)
                {
                    time.Id = id++;
                    time.Days = scheduleDays;
                    time.RouteId = route.Id;
                    time.RouteVariant = RouteVariants.SelectedIndex;
                }
                if (result.Count() > 0)
                {
                    _viewModel.AddRouteBeginTimes(result);
                }
            }
        }

        private void AdjustTime_Click(object sender, RoutedEventArgs e)
        {
            if(RouteDetailsList.SelectedItem is RouteStationViewModel routeStationView)
            {
                var defaultTimeShift = _viewModel.GetTimeShiftForStation(routeStationView);
                var dialog = new TimeAdjustmentWindow(defaultTimeShift, routeStationView, _viewModel.BeginTimesForRoute, _viewModel.GetTimeAdjustmentsForRouteVariant((BusRoute)RoutesList.SelectedItem, RouteVariants.SelectedIndex, GetSelectedScheduleDays()));
                dialog.ShowDialog();
                var result = dialog.GetResult();
                if (result.Count() > 0)
                {
                    _viewModel.UpdateTimeAdjustments(result);
                }
            }
        }

        private RouteBeginTime.ScheduleDays GetSelectedScheduleDays()
        {
            switch(scheduleDaysSwitch.SelectedIndex)
            {
                case 0:
                    return RouteBeginTime.ScheduleDays.WorkingDays;
                case 1:
                    return RouteBeginTime.ScheduleDays.Saturday;
                case 2:
                    return RouteBeginTime.ScheduleDays.SundayAndHolidays;
                default:
                    return RouteBeginTime.ScheduleDays.WorkingDays;
            }
        }

        private void ScheduleDaysChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoutesList.SelectedItem != null && RoutesList.SelectedItem is BusRoute route)
            {
                RouteBeginTime.ScheduleDays sd = RouteBeginTime.ScheduleDays.WorkingDays;
                switch (scheduleDaysSwitch.SelectedIndex)
                {
                    case 0:
                        sd = RouteBeginTime.ScheduleDays.WorkingDays;
                        break;
                    case 1:
                        sd = RouteBeginTime.ScheduleDays.Saturday;
                        break;
                    case 2:
                        sd = RouteBeginTime.ScheduleDays.SundayAndHolidays;
                        break;
                }
                _viewModel.OnRouteChanged(route, sd, RouteVariants.SelectedIndex);
            }
        }

        private void RouteStationsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoutesList.SelectedItem != null && RoutesList.SelectedItem is BusRoute route && RouteDetailsList.SelectedItem is RouteStationViewModel stationViewModel)
            {
                _viewModel.OnRouteStationChanged(route, RouteVariants.SelectedIndex, GetSelectedScheduleDays(), stationViewModel);
            }
        }
    }
}
