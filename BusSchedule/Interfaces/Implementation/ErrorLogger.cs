using BusSchedule.Core.UI.Interfaces;
using Microsoft.AppCenter.Crashes;
using System;

namespace BusSchedule.Interfaces.Implementation
{
    public class ErrorLogger : ILogger
    {
        public void LogError(Exception exception)
        {
            Crashes.TrackError(exception);
        }
    }
}
