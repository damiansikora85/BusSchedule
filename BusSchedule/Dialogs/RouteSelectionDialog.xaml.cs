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
        private TaskCompletionSource<int> _taskCompletionSource;

        public RouteSelectionDialog(Destination destination)
        {
            _taskCompletionSource = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            _viewModel = new RouteSelectionViewModel(destination);
            InitializeComponent();
            BindingContext = _viewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            _taskCompletionSource.TrySetCanceled();
            return base.OnBackButtonPressed();
        }

        internal Task<int> WaitForResult()
        {
            return _taskCompletionSource.Task;
        }

        private async void FirstRouteClicked(object sender, EventArgs e)
        {
            await SetResult(0);
        }

        private async void SecondRouteClicked(object sender, EventArgs e)
        {
            await SetResult(1);
        }

        private async Task SetResult(int selectedDest)
        { 
            _taskCompletionSource.TrySetResult(selectedDest);        
            await PopupNavigation.Instance.RemovePageAsync(this);
        }
    }
}