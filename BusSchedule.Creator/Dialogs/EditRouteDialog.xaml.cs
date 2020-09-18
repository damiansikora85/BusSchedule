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
    /// Interaction logic for EditRouteDialog.xaml
    /// </summary>
    public partial class EditRouteDialog : Window
    {
        public BusService BusService { get; }
        public ObservableCollection<BusStation> BusStations { get; }
        public ObservableCollection<RouteStationViewModel> Route { get; }

        private int _routeId;

        public EditRouteDialog(int routeId, BusService busService, ObservableCollection<BusStation> busStations)
        {
            _routeId = routeId;
            InitializeComponent();
            BusService = busService;
            BusStations = busStations;
            StationsList.ItemsSource = BusStations;
            Route = new ObservableCollection<RouteStationViewModel>();
            DataContext = this;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if(StationsList.SelectedItem != null && StationsList.SelectedItem is BusStation station)
            {
                Route.Add(new RouteStationViewModel
                {
                    RouteId = _routeId,
                    BusStation = station,
                    TimeDiff = int.Parse(TimeDiff.Text),
                    OrderNum = Route.Count
                });
            }
        }

        public List<RouteStationViewModel> GetResult()
        {
            return Route.ToList();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
