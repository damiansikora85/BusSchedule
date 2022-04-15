using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RouteStopsList : ContentPage
    {
        public RouteStopsList()
        {
            InitializeComponent();
        }
        private async void OnStationSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }
    }
}