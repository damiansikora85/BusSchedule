using BusSchedule.Core.Model;
using BusSchedule.Providers;
using BusSchedule.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BusSchedule.Components.ViewModels
{
    public class BusServiceViewModel
    {
        public string Name => _busService.Name;
        public Action<BusService> OnClick;
        private BusService _busService;

        public ICommand TapCommand
        {
            get
            {
                return new Command(() =>
                {
                    OnClick?.Invoke(_busService);
                });
            }
        }

        public BusServiceViewModel(BusService busService)
        {
            _busService = busService;
        }
    }
}
