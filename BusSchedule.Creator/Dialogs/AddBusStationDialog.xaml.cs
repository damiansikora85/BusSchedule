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
    /// Interaction logic for AddBusStationDialog.xaml
    /// </summary>
    public partial class AddBusStationDialog : Window
    {
        public AddBusStationDialog()
        {
            InitializeComponent();
        }

        public void Setup(int id)
        {
            Id.Text = id.ToString();
        }

        public BusStation GetResult()
        {
            if(string.IsNullOrEmpty(StationName.Text) || string.IsNullOrEmpty(Id.Text))
            {
                return null;
            }
            else
            {
                return new BusStation { Name = StationName.Text, Id = int.Parse(Id.Text) };
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
