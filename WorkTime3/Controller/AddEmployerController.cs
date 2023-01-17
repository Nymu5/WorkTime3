using System.Windows.Input;
using MyTime.Core;
using MyTime.Model;
using MyTime.View;

namespace MyTime.Controller;

[QueryProperty(nameof(Employer), "Employer")]
public class AddEmployerController : ControllerBase
{
    private MyTimeDatabase _db;

    public AddEmployerController()
    {
        _db = new MyTimeDatabase();
        Employer = new Employer();
        Employer.Id = Employer.getUUID();
        SaveEmployerCommand = new Command<bool>(execute: async (canSave) =>
        {
            await _db.SaveEmployerAsync(Employer);
            await Shell.Current.GoToAsync("..");
        }, canExecute: (canSave) => canSave);
    }

    // Commands
    public ICommand SaveEmployerCommand { get; set; }

    // Properties
    private Employer _employer;

    public Employer Employer
    {
        get => _employer;
        set => SetProperty(ref _employer, value);
    }

    private bool _canExecute;

    public bool CanExecute
    {
        get => _canExecute;
        set => SetProperty(ref _canExecute, value);
    }
}