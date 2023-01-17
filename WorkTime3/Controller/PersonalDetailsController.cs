using System.Windows.Input;
using MyTime.Core;
using MyTime.Model;

namespace MyTime.Controller;

[QueryProperty(nameof(Settings), "settings")]
public class PersonalDetailsController : ControllerBase
{
    private MyTimeDatabase db;

    public PersonalDetailsController()
    {
        db = new MyTimeDatabase();
        SaveCommand = new Command(canExecute: () => true, execute: async () =>
        {
            if (db != null) await db.SaveProfileAsync(Settings);
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