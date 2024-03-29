﻿using BusSchedule.Core.Services;
using System;
using IPreferences = BusSchedule.Core.Services.IPreferences;

namespace BusSchedule.Interfaces.Implementation
{
    public class CustomPreferences : IPreferences
    {
        public bool IsFirstLaunch => VersionTracking.IsFirstLaunchEver;

        public bool IsFirstLaunchVersion => VersionTracking.IsFirstLaunchForCurrentVersion;

        public void Delete(string key) => Preferences.Remove(key);

        public string Get(string key, string defaultValue) => Preferences.Get(key, defaultValue);

        public int Get(string key, int defaultValue) => Preferences.Get(key, defaultValue);
        public DateTime Get(string key, DateTime defaultValue) => Preferences.Get(key, defaultValue);

        public bool Get(string key, bool defaultValue) => Preferences.Get(key, defaultValue);

        public void Set(string key, string value) => Preferences.Set(key, value);

        public void Set(string key, int value) => Preferences.Set(key, value);

        public void Set(string key, bool value) => Preferences.Set(key, value);

        public void Set(string key, DateTime value) => Preferences.Set(key, value);
    }
}
