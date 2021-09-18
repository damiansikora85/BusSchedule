using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.CloudService
{
    public interface ICloudService
    {
        Task<string> TestGet();
        Task<string> GetLatestScheduleFilename();
    }
}
