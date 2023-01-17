using System.Windows.Input;
using MyTime.Core;
using MyTime.Model;

namespace MyTime.Controller;

[QueryProperty(nameof(Settings), "settings")]
public class BankInformationController : ControllerBase
{
    private MyTimeDatabase _db;

    public BankInformationController()
    {
        _db = new MyTimeDatabase();
        SaveCommand = new Command(canExecute: () => true, execute: async () =>
        {
            if (_db != null) await _db.SaveProfileAsync(Settings);
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