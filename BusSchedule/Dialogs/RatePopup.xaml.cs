using BusSchedule.Core.Services;
using BusSchedule.Interfaces;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using TinyIoC;
using IPreferences = BusSchedule.Core.Services.IPreferences;

namespace BusSchedule.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RatePopup : Popup
    {
        private IPreferences _preferences;
        //private IReviewService _reviewService;

        public RatePopup(IPreferences preferences)//, IReviewService reviewService)
        {
            _preferences = preferences;
            //_reviewService = reviewService;
            InitializeComponent();
            Opened += OnOpened;
        }

        void OnOpened(object? sender, PopupOpenedEventArgs e)
        {
            Opened += OnOpened;
            _preferences.Set("rate_popup_last_shown", DateTime.Today.ToString());
        }

        private async void OnRateClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync(new Uri("market://details?id=com.darktower.bus"));
            _preferences.Set("rated", "1");
            TinyIoCContainer.Current.Resolve<IAnalyticsService>().LogEvent("RateNow");
            Close();
        }

        private void OnRateLaterClicked(object sender, EventArgs e)
        {
            TinyIoCContainer.Current.Resolve<IAnalyticsService>().LogEvent("RateLater");
            Close();
        }
    }
}