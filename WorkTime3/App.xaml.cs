using MyTime.Core;

namespace MyTime;

public partial class App : Application
{
    private MyTimeDatabase _db;
    public App()
    {
        
        InitializeComponent();

        MainPage = new AppShell();
    }
}