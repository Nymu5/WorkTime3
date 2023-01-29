using System.Windows.Input;
using MyTime.Core;
using MyTime.View;
using ReactiveUI;

namespace MyTime.Controller;

public class SettingsController : ReactiveObject
{
    public SettingsController()
    {
        SettingSelectedCommand = new Command<string>(canExecute: _ => true,
            execute: SettingSelectedTask);
    }

    private async void SettingSelectedTask(string setting)
    {
        var settings = Constants.Settings;
        switch (setting)
        {
            case "personal":
                await Shell.Current.GoToAsync("PersonalDetailsPage", new Dictionary<string, object> { { "settings", settings }, { "db", Constants.Database } });
                break;
            case "bank":
                await Shell.Current.GoToAsync("BankInformationPage", new Dictionary<string, object> { { "settings", settings } });
                break;
            case "iesettings":
                await Shell.Current.GoToAsync("ImportExportPage", new Dictionary<string, object> { { "settings", settings } });
                break;
            case "invoice":
                await Shell.Current.GoToAsync(nameof(InvoiceSettingsPage), new Dictionary<string, object> { { "settings", settings } });
                break;
        }
    }

    public ICommand SettingSelectedCommand { get; set; }
}