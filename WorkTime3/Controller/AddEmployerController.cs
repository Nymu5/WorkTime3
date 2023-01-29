using System.Windows.Input;
using MyTime.Core;
using MyTime.Model;
using ReactiveUI;

namespace MyTime.Controller;

[QueryProperty(nameof(Employer), "Employer")]
public class AddEmployerController : ReactiveObject
{

    public AddEmployerController()
    { 
        Employer = new Employer
        {
            Id = Employer.GetUuid()
        };

        async void SaveEmployerTask(bool canSave)
        {
            await Constants.Database.SaveEmployerAsync(Employer);
            await Shell.Current.GoToAsync("..");
        }

        SaveEmployerCommand = new Command<bool>(execute: SaveEmployerTask, canExecute: (canSave) => canSave);

        EmployerNameChangedCommand = new Command(execute: () => this.RaisePropertyChanged(nameof(CanSave)));
    }

    // Commands
    public ICommand SaveEmployerCommand { get; set; }
    public ICommand EmployerNameChangedCommand { get; }

    // Properties
    private readonly Employer _employer;
    public Employer Employer
    {
        get => _employer;
        init => this.RaiseAndSetIfChanged(ref _employer, value);
    }
    
    public bool CanSave => !String.IsNullOrWhiteSpace(Employer.Name);

    // Function
}