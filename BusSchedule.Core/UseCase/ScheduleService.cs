using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.UseCase
{
    public class ScheduleService
    {
        private IDataProvider _dataProvider;

        private ScheduleService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public static ScheduleService Create(IDataProvider dataProvider)
        {
            return new ScheduleService(dataProvider);
        }

        public void Setup(string jsonString)
        {

        }

        public Task<List<Routes>> GetBusServicesAsync()
        {
            return _dataProvider.GetRoutes();
        }
    }
}
