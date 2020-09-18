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
    /// Interaction logic for EditBeginTimesDialog.xaml
    /// </summary>
    public partial class EditBeginTimesDialog : Window
    {
        public ObservableCollection<TimeSpan> Times { get; set; }
        public string Time { get; set; }

        public EditBeginTimesDialog()
        {
            Times = new ObservableCollection<TimeSpan>();
            InitializeComponent();
            DataContext = this;
        }

        public IList<RouteBeginTime> GetResult()
        {
            return Times.Select(time => new RouteBeginTime { Time = time,  }).ToList();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void time_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if(!string.IsNullOrEmpty(time.Text) && TimeSpan.TryParse(time.Text, out TimeSpan newTime))
                {
                    Times.Add(newTime);
                }
                time.Text = "";
            }
        }
    }
}
