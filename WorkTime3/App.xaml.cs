﻿using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using MyTime.Core;

namespace MyTime;

public partial class App : Application
{
    public App()
    {
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