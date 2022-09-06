using BusSchedule.Core.Model;
using BusSchedule.Dialogs.ViewModels;
using CommunityToolkit.Maui.Views;

namespace BusSchedule.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RouteSelectionDialog : Popup
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

        //protected override bool OnBackButtonPressed()
        //{
        //    _taskCompletionSource.TrySetCanceled();
        //    return base.OnBackButtonPressed();
        //}

        internal Task<int> WaitForResult()
        {
            return _taskCompletionSource.Task;
        }

        private void FirstRouteClicked(object sender, EventArgs e)
        {
            SetResult(0);
        }

        private void SecondRouteClicked(object sender, EventArgs e)
        {
            SetResult(1);
        }

        private void SetResult(int selectedDest)
        { 
            _taskCompletionSource.TrySetResult(selectedDest);
            Close();
        }
    }
}