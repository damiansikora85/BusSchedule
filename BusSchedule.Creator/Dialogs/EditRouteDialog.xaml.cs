using BusSchedule.Core.Model;
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

        public EditRouteDialog(BusService busService, ObservableCollection<BusStation> busStations)
        {
            InitializeComponent();
            BusService = busService;
            BusStations = busStations;
            StationsList.ItemsSource = BusStations;
        }
    }
}
