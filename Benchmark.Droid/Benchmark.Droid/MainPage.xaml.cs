using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Benchmark.Droid
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            SetIsRunning(true);
            try
            {
                var logger = new AccumulationLogger();
                await Task.Run(() =>
                {
                    var config = default(IConfig);
#if DEBUG
                    config = new DebugInProcessConfig();
#endif
                    var summary = BenchmarkRunner.Run<BenchmarkTest>(config);
                    MarkdownExporter.Console.ExportToLog(summary, logger);
                    ConclusionHelper.Print(logger,
                            summary.BenchmarksCases
                                   .SelectMany(benchmark => benchmark.Config.GetCompositeAnalyser().Analyse(summary))
                                   .Distinct()
                                   .ToList());
                });
                SetSummary(logger.GetLog());
            }
            catch (Exception exc)
            {
                await DisplayAlert("Error", exc.Message, "Ok");
            }
            finally
            {
                SetIsRunning(false);
            }
        }

        void SetIsRunning(bool isRunning)
        {
            Indicator.IsRunning = isRunning;
            Run.IsVisible =
                Summary.IsVisible = !isRunning;
        }

        void SetSummary(string text)
        {
            Summary.Text = text;
            var size = Summary.Measure(double.MaxValue, double.MaxValue).Request;
            Summary.WidthRequest = size.Width;
            Summary.HeightRequest = size.Height;
        }
    }
}
