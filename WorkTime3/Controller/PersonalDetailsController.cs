using System.Windows.Input;
using MyTime.Core;
using MyTime.Model;
using ReactiveUI;

namespace MyTime.Controller;

[QueryProperty(nameof(Settings), "settings")]
public class PersonalDetailsController : ReactiveObject
{
    public PersonalDetailsController()
    {
        SaveCommand = new Command(canExecute: () => true, execute: SaveTask);
    }

    private async void SaveTask()
    {
        Constants.Settings = _settings;
        await Constants.Database.SaveProfileAsync(Constants.Settings);
        await Shell.Current.GoToAsync("..");
    }

    private Settings _settings;

    public Settings Settings
    {
        get => _settings;
        set => this.RaiseAndSetIfChanged(ref _settings, value);
    }

    public ICommand SaveCommand { get; set; }
}