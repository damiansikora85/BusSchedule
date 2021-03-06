﻿using BusSchedule.Interfaces;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RatePopup : PopupPage
    {
        private IPreferences _preferences;
        //private IReviewService _reviewService;

        public RatePopup(IPreferences preferences)//, IReviewService reviewService)
        {
            _preferences = preferences;
            //_reviewService = reviewService;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            _preferences.Set("rate_popup_last_shown", DateTime.Today.ToString());
            base.OnAppearing();
        }

        private async void OnRateClicked(object sender, EventArgs e)
        {
            //await _reviewService.ShowReviewPopup();
            await Launcher.OpenAsync(new Uri("market://details?id=com.darktower.bus"));
            _preferences.Set("rated", "1");
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("RateNow");
            await Navigation.PopPopupAsync();
        }

        private async void OnRateLaterClicked(object sender, EventArgs e)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("RateLater");
            await Navigation.PopPopupAsync();
        }
    }
}