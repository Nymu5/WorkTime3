using System.Windows.Input;
using MyTime.Core;
using MyTime.Model;

namespace MyTime.Controller;

[QueryProperty(nameof(Settings), "settings")]
public class RegionalSettingsController : ControllerBase
{
    private MyTimeDatabase _db;

    public RegionalSettingsController()
    {
        _db = new MyTimeDatabase();

        SaveCommand = new Command(canExecute: () => true, execute: async () =>
        {
            if (_db != null) await _db.SaveProfileAsync(Settings);
            Console.WriteLine(Settings);
            await Shell.Current.GoToAsync("..");
        });
        LoadRegionalCommand = new Command(canExecute: () => true, execute: () =>
        {
            if (_db != null) Settings.Currency = Constants.Currencies.Find(c => c.Id == Settings.CurrencyId);
            if (_db != null) Settings.DSeparator = Constants.DSeparators.Find(d => d.Id == Settings.DSeparatorId);
        });
    }
    private Settings _settings;
    public Settings Settings
    {
        get => _settings;
        set => SetProperty(ref _settings, value);
    }

    public List<Currency> Currencies => Constants.Currencies;
    public List<DSeparator> DSeparators => Constants.DSeparators;

    public ICommand SaveCommand { get; set; }
    public ICommand LoadRegionalCommand { get; set; }
}