﻿namespace MyTime;

// ReSharper disable once RedundantExtendsListEntry
public partial class App : Application
{
    public App()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTAwNjAyMEAzMjMwMmUzNDJlMzBJbTIwWncrWmJmZlRvemZCNFdmb2VRUUNtU1FqV1N1d0cwNWNKenNwVGlJPQ==");
        
        InitializeComponent();

        MainPage = new AppShell();
    }
}