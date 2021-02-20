﻿using BusSchedule.Core.Utils;
using BusSchedule.Pages;
using BusSchedule.Providers;
using System;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using BusSchedule.Interfaces;
using BusSchedule.Interfaces.Implementation;

namespace BusSchedule
{
    public partial class App : Application
    {
        public static string DB_FILENAME = "sqlite20210218.db";
        public App()
        {
            InitializeComponent();
            RegisterIoC();
            UserAppTheme = OSAppTheme.Light;
            MainPage = new NavigationPage(new RoutesPage()) { BarBackgroundColor = Color.FromHex("#237194") };
        }

        private void RegisterIoC()
        {
            var container = TinyIoCContainer.Current;
            var databasePath = DependencyService.Get<IFileAccess>().GetLocalFilePath(DB_FILENAME);
            container.Register<IDataProvider, SQLDataProvider>(new SQLDataProvider(databasePath));
            container.Register<IPreferences, CustomPreferences>();
        }

        protected override void OnStart()
        {
            AppCenter.Start("android=fc2cc03c-f502-42d6-b5ed-8373e82d03c2;",
                  typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
