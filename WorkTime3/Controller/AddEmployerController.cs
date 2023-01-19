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

        this.WhenAnyValue(x => x.Employer)
            .Subscribe(_ => CheckCanSave()); 

        var canSave = this.WhenAnyValue(x => x.CanSave);
        SaveEmployerCommand = ReactiveCommand.CreateFromTask(SaveEmployerTask, canSave);
    }

    // Commands
    public ICommand SaveEmployerCommand { get; set; }

    // Properties
    private Employer _employer;

    public Employer Employer
    {
        get => _employer;
        set => this.RaiseAndSetIfChanged(ref _employer, value);
    }
    
    private bool _canSave;
    public bool CanSave
    {
        get => _canSave;
        set => this.RaiseAndSetIfChanged(ref _canSave, value);
    }
    
    // Function
    private void CheckCanSave()
    {
        CanSave = !String.IsNullOrWhiteSpace(Employer.Name); 
    }

    private async Task SaveEmployerTask()
    {
        await Constants.Database.SaveEmployerAsync(Employer);
        await Shell.Current.GoToAsync("..");
    }
}