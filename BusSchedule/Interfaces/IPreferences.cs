﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Interfaces
{
    public interface IPreferences
    {
        bool IsFirstLaunch { get; }

        void Set(string key, string value);
        string Get(string key, string defaultValue);
    }
}
