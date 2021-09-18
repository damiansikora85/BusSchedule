using BusSchedule.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.CloudService
{
    public interface ICloudService
    {
        Task<string> GetLatestScheduleFilename();
        Task<List<News>> GetNews();
    }
}
