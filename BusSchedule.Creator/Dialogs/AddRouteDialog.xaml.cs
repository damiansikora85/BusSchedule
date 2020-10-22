using BusSchedule.Core.Model;
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

namespace BusSchedule.Creator.Dialogs
{
    /// <summary>
    /// Interaction logic for AddRouteDialog.xaml
    /// </summary>
    public partial class AddRouteDialog : Window
    {
        public AddRouteDialog(string name, List<BusStation> stations)
        {
            InitializeComponent();
            startStationList.ItemsSource = stations;
            endStationList.ItemsSource = stations;
            serviceName.Content = name;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public BusRoute GetResult()
        {
            if (startStationList.SelectedItem != null && startStationList.SelectedItem is BusStation startStation
                && endStationList.SelectedItem != null && endStationList.SelectedItem is BusStation endStation)
            {
                return new BusRoute
                {
                    StartStationId = startStation.Id,
                    EndStationId = endStation.Id,
                    Name = $"{serviceName.Content}-{startStation.Name}-{endStation.Name}",
                    VariantsNum = int.Parse(VariantsNum.Text)
                };
            }
            return null;
        }
    }
}
