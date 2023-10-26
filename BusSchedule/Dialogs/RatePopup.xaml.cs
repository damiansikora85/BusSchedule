using BusSchedule.Core.Services;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
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
            //await _reviewService.ShowReviewPopup();
            await Launcher.OpenAsync(new Uri("market://details?id=com.darktower.bus"));
            _preferences.Set("rated", "1");
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("RateNow");
            //await Navigation.PopPopupAsync();
            Close();
        }

        private void OnRateLaterClicked(object sender, EventArgs e)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("RateLater");
            Close();
        }
    }
}