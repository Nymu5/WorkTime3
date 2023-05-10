using System.Windows.Input;
using DynamicData;
using MyTime.Core;
using MyTime.Model;
using ReactiveUI;

namespace MyTime.Controller;

public class ShellController : ReactiveObject
{
    public ShellController()
    {
        Constants.Database = new MyTimeDatabase();
        Constants.Employers = new SourceCache<Employer, string>(e => e.Id);
        Constants.Times = new SourceCache<Time, string>(t => t.Id);
        
        DatabaseSetupCommand = new Command(execute: DatabaseSetupTask);
    }

    private async void DatabaseSetupTask()
    {
        var settings = await Constants.Database.GetSettingsAsync();
        if (settings.Count == 0)
        {
            Constants.Settings = new Settings
            {
                Id = Settings.GetUuid(),
                DefaultInvoiceDays = 10,
                InformationText = "%Nach § 19 Abs. 1. UStG wird keine Umsatzsteuer berechnet.%\n\nZahlung innerhalb von %due% Tagen ab Rechnungseingang ohne Abzüge an die unten angegebene Bankverbindung.\n\nBank: %bname%\nIBAN: %biban%\nBIC: %bbic%\nKontoinhaber: %name%\n\nMit freundlichen Grüßen\n%name%",
                AddressLine1 = String.Empty,
                AddressLine2 = String.Empty,
                BankBic = String.Empty,
                BankIban = String.Empty,
                BankName = String.Empty,
                LastInvoice = String.Empty,
                Name = String.Empty,
                TaxId = String.Empty,
                CompanyName = String.Empty,
                PhoneNumber = String.Empty,
                EmailAddress = String.Empty,
                ContactName = String.Empty,
                ManagingDirector = String.Empty,
                IntroductionText = String.Empty,
            };
            await Constants.Database.SaveProfileAsync(Constants.Settings);
        }
        else
            Constants.Settings = settings[0];

        Constants.Employers.AddOrUpdate(await Constants.Database.GetEmployersAsync());
        Constants.Times.AddOrUpdate(await Constants.Database.GetTimesAsync());
        Console.WriteLine("Database loaded successfully");
        Console.WriteLine(Constants.CurrencySymbol);
        Console.WriteLine(Constants.LanguageSymbol);
    }

    public ICommand DatabaseSetupCommand { get; set; }
}