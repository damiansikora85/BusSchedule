using CommunityToolkit.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusSchedule.Core.Services;
using Syncfusion.Maui.Core.Hosting;

namespace BusSchedule;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCompatibility()
            .ConfigureSyncfusionCore()
            .UseMauiMaps()
        .ConfigureMauiHandlers(handlers =>
         {

            // Register just one handler for the control you need
            //handlers.AddCompatibilityRenderer(typeof(Xamarin.CommunityToolkit.UI.Views.MediaElement), typeof(Xamarin.CommunityToolkit.UI.Views.MediaElementRenderer));
         });
        //.ConfigureFonts(fonts =>
        //{
        //    fonts.AddFont("AmaticSC-Regular.ttf", "Amatic");
        //    fonts.AddFont("materialdesignicons-webfont.ttf", "MaterialIcons");
        //    fonts.AddFont("Michella-Garden.otf", "Michella");
        //});

        builder.Services.AddTransient<FileAccessService>();

        return builder.Build();
    }
}
