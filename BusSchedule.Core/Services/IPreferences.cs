using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Services
{
    public interface IPreferences
    {
        bool IsFirstLaunch { get; }
        bool IsFirstLaunchVersion { get; }

        void Set(string key, string value);
        void Set(string key, int value);
        void Set(string key, bool value);
        void Set(string key, DateTime value);
        string Get(string key, string defaultValue);
        int Get(string key, int defaultValue);
        bool Get(string key, bool defaultValue);
        DateTime Get(string key, DateTime defaultValue);
        void Delete(string key);
    }
}
