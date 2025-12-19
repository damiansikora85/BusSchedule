using BusSchedule.Core.Model;
using BusSchedule.Dialogs.ViewModels;
using CommunityToolkit.Maui.Views;

namespace BusSchedule.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RouteSelectionDialog : Popup<int>
    {
        public RouteSelectionDialog(Destination destination)
        {
            var viewModel = new RouteSelectionViewModel(destination);
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void FirstRouteClicked(object sender, EventArgs e)
        {
            await CloseAsync(0);
        }

        private async void SecondRouteClicked(object sender, EventArgs e)
        {
            await CloseAsync(1);
        }
    }
}