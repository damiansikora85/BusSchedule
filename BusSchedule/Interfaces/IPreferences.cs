using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Interfaces
{
    public interface IPreferences
    {
        bool IsFirstLaunch { get; }

        void Set(string key, string value);
        void Set(string key, int value);
        string Get(string key, string defaultValue);
        int Get(string key, int defaultValue);
    }
}
