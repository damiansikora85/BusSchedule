using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.UI.Interfaces
{
    public interface ILogger
    {
        void LogError(Exception exception);
    }
}
