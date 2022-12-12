using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using WorkTime3.Controller;
using WorkTime3.Core;
using WorkTime3.Resources.Templates;
using WorkTime3.View;

namespace WorkTime3;

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
        builder.Services.AddSingleton<EmployerController>();
        builder.Services.AddTransient<EmployerController>();
        builder.Services.AddSingleton<AddEmployerController>();
        builder.Services.AddTransient<AddEmployerController>();
        builder.Services.AddSingleton<WorkTime3Database>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}