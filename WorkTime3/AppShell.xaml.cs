using WorkTime3.View;

namespace WorkTime3;

public partial class AppShell : Shell
{
    public AppShell()
    {
        Routing.RegisterRoute("EmployerPage/AddEmployerPage", typeof(AddEmployerPage));
        InitializeComponent();
    }
}