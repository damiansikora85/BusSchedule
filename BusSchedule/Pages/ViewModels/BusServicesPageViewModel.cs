﻿using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using BusSchedule.Providers;
using BusSchedule.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TinyIoC;

namespace BusSchedule.Pages.ViewModels
{
    public class BusServicesPageViewModel
    {
        private IDataProvider _dataProvider;
        public List<BusService> BusServices { get; private set; }

        public BusServicesPageViewModel()
        {
            _dataProvider = TinyIoCContainer.Current.Resolve<IDataProvider>();
            UpdateData();
        }

        private void UpdateData()
        {
            DataUpdater.SetupData(_dataProvider);
        }

        internal async Task RefreshBusServicesAsync()
        {
            BusServices = await _dataProvider.GetBusServices();
        }
    }
}
