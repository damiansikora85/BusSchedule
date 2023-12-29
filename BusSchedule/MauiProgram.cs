using BusSchedule.Core.Services;
using CommunityToolkit.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;

namespace BusSchedule;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCompatibility()
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
