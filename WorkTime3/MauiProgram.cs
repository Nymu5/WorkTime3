using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using MyTime.Controller;
using MyTime.Core;
using MyTime.Resources.Templates;
using MyTime.View;
using CommunityToolkit.Maui;

namespace MyTime;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.UseMauiApp<App>().UseMauiCommunityToolkit();
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