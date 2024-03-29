﻿using System.Reactive.Disposables;
using ReactiveUI;

namespace MyTime.View;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, x => x.StatsEmployers, x => x.StatsEmployers.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.StatsHours, x => x.StatsHours.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.StatsTotal, x => x.StatsTotal.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.StatsTimes, x => x.StatsTimes.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.ChartData, x => x.TabView.ItemsSource)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.ChartSelectedIndex, x => x.TabView.SelectedIndex)
                .DisposeWith(disposable);
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Title = String.Empty;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Title = String.Empty;
    }
}