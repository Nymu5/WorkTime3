using Microsoft.Extensions.Logging;
using MyTime.Controller;
using MyTime.Core;
using CommunityToolkit.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using MyTime.Controls;
using Syncfusion.Maui.Core.Hosting;

namespace MyTime;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCompatibility()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureSyncfusionCore()
            .UseMauiApp<App>().UseMauiCommunityToolkit()
            .ConfigureMauiHandlers(handlers =>
            {
#if IOS
                //handlers.AddHandler(typeof(Entry), typeof(KeyboardOverlapRenderer));
#endif
            })
            .Services
                .AddSingleton<EmployerController>()
                .AddTransient<EmployerController>()
                .AddSingleton<AddEmployerController>()
                .AddTransient<AddEmployerController>()
                .AddSingleton<MyTimeDatabase>();
       
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}