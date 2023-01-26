using System.Windows.Input;
using MyTime.Core;
using MyTime.Model;

namespace MyTime.Controller;

[QueryProperty(nameof(Settings), "settings")]
public class BankInformationController : ControllerBase
{
    public BankInformationController()
    {
        SaveCommand = new Command(canExecute: () => true, execute: async () =>
        {
            Constants.Settings = _settings;
            await Constants.Database.SaveProfileAsync(Constants.Settings);
            await Shell.Current.GoToAsync("..");
        });
    }

    private Settings _settings;

    public Settings Settings
    {
        get => _settings;
        set => SetProperty(ref _settings, value);
    }

    public ICommand SaveCommand { get; set; }
}