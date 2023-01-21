using System.Windows.Input;
using MyTime.Core;
using MyTime.Model;
using MyTime.View;
using ReactiveUI;

namespace MyTime.Controller;

[QueryProperty(nameof(Employer), "Employer")]
public class AddEmployerController : ReactiveObject
{

    public AddEmployerController()
    { 
        Employer = new Employer();
        Employer.Id = Employer.getUUID();

        SaveEmployerCommand = new Command<bool>(execute: async (canSave) =>
        {
            await Constants.Database.SaveEmployerAsync(Employer);
            await Shell.Current.GoToAsync("..");
        }, canExecute: (canSave) => canSave);

        EmployerNameChangedCommand = new Command(execute: () => this.RaisePropertyChanged(nameof(CanSave)));
    }

    // Commands
    public ICommand SaveEmployerCommand { get; set; }
    public ICommand EmployerNameChangedCommand { get; }

    // Properties
    private Employer _employer;

    public Employer Employer
    {
        get => _employer;
        set => this.RaiseAndSetIfChanged(ref _employer, value);
    }
    
    public bool CanSave => !String.IsNullOrWhiteSpace(Employer.Name);

    // Function
}