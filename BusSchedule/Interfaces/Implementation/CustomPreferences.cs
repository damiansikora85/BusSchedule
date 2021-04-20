using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace BusSchedule.Interfaces.Implementation
{
    public class CustomPreferences : IPreferences
    {
        public bool IsFirstLaunch => VersionTracking.IsFirstLaunchEver;

        public void Delete(string key) => Preferences.Remove(key);

        public string Get(string key, string defaultValue) => Preferences.Get(key, defaultValue);

        public int Get(string key, int defaultValue) => Preferences.Get(key, defaultValue);

        public void Set(string key, string value) => Preferences.Set(key, value);

        public void Set(string key, int value) => Preferences.Set(key, value);
    }
}
