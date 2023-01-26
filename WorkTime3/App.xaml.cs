using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using MyTime.Core;

namespace MyTime;

public partial class App : Application
{
    public App()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTAwNjAyMEAzMjMwMmUzNDJlMzBJbTIwWncrWmJmZlRvemZCNFdmb2VRUUNtU1FqV1N1d0cwNWNKenNwVGlJPQ==");
        
        InitializeComponent();

        MainPage = new AppShell();

        LiveCharts.Configure(config =>
        {
            config
                .AddSkiaSharp()
                .AddDefaultMappers()
                .AddLightTheme();
        });
    }
}