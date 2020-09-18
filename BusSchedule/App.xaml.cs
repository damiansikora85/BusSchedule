using BusSchedule.Core.Utils;
using BusSchedule.Pages;
using BusSchedule.Providers;
using System;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            RegisterIoC();
            MainPage = new BusServicesPage();
        }

        private void RegisterIoC()
        {
            var container = TinyIoCContainer.Current;
            container.Register<IDataProvider, SQLDataProvider>();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
