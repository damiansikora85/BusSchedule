using BusSchedule.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Utils
{
    public interface IDataProvider
    {
        IList<BusService> GetBusServices();
        IList<BusRoute> GetBusRoutes(int busServiceId);
    }
}
