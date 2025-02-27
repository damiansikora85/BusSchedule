using BusSchedule.Components.ViewModels;
using BusSchedule.Core.Model;


namespace BusSchedule.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RouteView : ContentView
    {
        public Action<Routes> OnServiceClicked;
        private RouteViewModel _viewModel;
        public RouteView(Routes route)
        {
            InitializeComponent();
            _viewModel = new RouteViewModel(route);
            BindingContext = _viewModel;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            OnServiceClicked?.Invoke(_viewModel.Route);
        }
    }
}