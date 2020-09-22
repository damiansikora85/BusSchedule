using BusSchedule.Core.Model;
using BusSchedule.Dialogs.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RouteSelectionDialog : PopupPage
    {
        private RouteSelectionViewModel _viewModel;
        private TaskCompletionSource<BusRoute> _taskCompletionSource;

        public RouteSelectionDialog(List<Core.Model.BusRoute> routes)
        {
            _taskCompletionSource = new TaskCompletionSource<BusRoute>();
            _viewModel = new RouteSelectionViewModel(routes);
            InitializeComponent();
            BindingContext = _viewModel;
        }

        internal Task<BusRoute> WaitForResult()
        {
            return _taskCompletionSource.Task;
        }

        private async void FirstRouteClicked(object sender, EventArgs e)
        {
            await SetResult(_viewModel.FirstRoute);
        }

        private async void SecondRouteClicked(object sender, EventArgs e)
        {
            await SetResult(_viewModel.SecondRoute);
        }

        private async Task SetResult(BusRoute route)
        {
            await PopupNavigation.Instance.RemovePageAsync(this);
            _taskCompletionSource.SetResult(route);
        }
    }
}