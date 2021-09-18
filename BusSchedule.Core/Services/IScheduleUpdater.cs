using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.Services
{
    public interface IScheduleUpdater
    {
        Task<bool> TryUpdateSchedule(IFileAccess fileAccess, string defaultDbFilename);
    }
}
