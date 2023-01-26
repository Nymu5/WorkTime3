using System.Windows.Input;
using MyTime.Core;
using MyTime.Model;
using ReactiveUI;

namespace MyTime.Controller;

[QueryProperty(nameof(Settings), "settings")]
public class InvoiceSettingsController : ReactiveObject
{
    public InvoiceSettingsController()
    {
        SaveCommand = new Command(execute: async () =>
        {
            Constants.Settings = _settings;
            await Constants.Database.SaveProfileAsync(Constants.Settings);
            await Shell.Current.GoToAsync("..");
        });
    }
    
    // Bindings
    private Settings _settings;
    public Settings Settings
    {
        get => _settings;
        set => this.RaiseAndSetIfChanged(ref _settings, value);
    }
    
    // Commands 
    public ICommand SaveCommand { get; }
    
    // Functions
}