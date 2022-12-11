using WorkTime3.View;
using AddTimeController = WorkTime3.Controller.AddTimeController;

namespace WorkTime3;

public partial class AppShell : Shell
{
    public AppShell()
    {
        
        InitializeComponent();
        
        Routing.RegisterRoute(nameof(AddEmployerPage), typeof(AddEmployerPage));
        Routing.RegisterRoute(nameof(AddTimePage), typeof(AddTimePage));
    }
}