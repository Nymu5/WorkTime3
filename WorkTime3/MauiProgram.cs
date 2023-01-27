using Microsoft.Extensions.Logging;
using MyTime.Controller;
using MyTime.Core;
using CommunityToolkit.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace MyTime;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCompatibility()
            .ConfigureMauiHandlers((handlers) =>
            {   
#if IOS
                handlers.AddHandler<Entry, EntryHandler>();
#endif
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.UseMauiApp<App>().UseMauiCommunityToolkit();
        builder.UseSkiaSharp(true);
        builder.Services.AddSingleton<EmployerController>();
        builder.Services.AddTransient<EmployerController>();
        builder.Services.AddSingleton<AddEmployerController>();
        builder.Services.AddTransient<AddEmployerController>();
        builder.Services.AddSingleton<MyTimeDatabase>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}