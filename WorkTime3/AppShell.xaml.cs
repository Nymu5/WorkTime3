using MyTime.View;

namespace MyTime;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(AddTimePage), typeof(AddTimePage));
        Routing.RegisterRoute(nameof(AddEmployerPage), typeof(AddEmployerPage));
        Routing.RegisterRoute(nameof(DetailEmployerPage), typeof(DetailEmployerPage));
        Routing.RegisterRoute(nameof(PersonalDetailsPage), typeof(PersonalDetailsPage));
        Routing.RegisterRoute(nameof(BankInformationPage), typeof(BankInformationPage));
        Routing.RegisterRoute(nameof(ImportExportPage), typeof(ImportExportPage));
        Routing.RegisterRoute(nameof(InvoiceSettingsPage), typeof(InvoiceSettingsPage));
        Routing.RegisterRoute(nameof(InvoiceCreatorPage), typeof(InvoiceCreatorPage));
    }
}