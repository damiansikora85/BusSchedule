using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ThreadsTestPage : ContentPage, INotifyPropertyChanged
    {
        public string ThreadsNum { get; set; }
        public string ThreadsMin { get; set; }
        public string ThreadsMax { get; set; }

        public ThreadsTestPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(async () => await UpdateInfo());
        }

        private async Task UpdateInfo()
        {
            while(true)
            {
                ThreadPool.GetAvailableThreads(out int workerThreads, out int completionPortThreads);
                ThreadPool.GetMinThreads(out int workerThreadsMin, out int completionPortThreadsMin);
                ThreadPool.GetMaxThreads(out int workerThreadsMax, out int completionPortThreadsMax);

                ThreadsNum = $"Threads num: {workerThreads}";
                ThreadsMin = $"Threads min: {workerThreadsMin}";
                ThreadsMax = $"Threads max: {workerThreadsMax}";

                OnPropertyChanged(nameof(ThreadsNum));
                OnPropertyChanged(nameof(ThreadsMin));
                OnPropertyChanged(nameof(ThreadsMax));

                await Task.Delay(1000);
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    }
}