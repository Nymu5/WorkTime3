using System.Windows.Input;
using MyTime.Core;
using MyTime.Model;
using MyTime.View;
using ReactiveUI;

namespace MyTime.Controller;

[QueryProperty(nameof(Employer), "Employer")]
public class DetailEmployerController : ReactiveObject
{
    public DetailEmployerController()
    {
        EditEmployerCommand = new Command(execute: EditEmployerTask);
        DeleteEmployerCommand = new Command(execute: DeleteEmployerTask);
        CreateInvoiceCommand = new Command(execute: CreateInvoiceTask);
    }

    private async void CreateInvoiceTask()
    {
        await Shell.Current.GoToAsync(nameof(InvoiceCreatorPage), new Dictionary<string, object> { { "Employer", Employer }, { "Settings", Constants.Settings } });
    }

    private async void DeleteEmployerTask()
    {
        var result = await Shell.Current.DisplayActionSheet($"Are you sure you want to delete {Employer.Name} and ALL OF THE TRACKED HOURS? This operation cannot be undone!", "Cancel", "Yes");
        if (result != "Yes") return;
        await Constants.Database.DeleteEmployerAsync(Employer);
        await Shell.Current.GoToAsync("..");
    }

    private async void EditEmployerTask()
    {
        if (Employer == null) return;
        await Shell.Current.GoToAsync("AddEmployerPage", new Dictionary<string, object> { { "Employer", Employer }, });
    }

    private Employer _employer;
    public Employer Employer
    {
        get => _employer;
        set => this.RaiseAndSetIfChanged(ref _employer, value);
    }

    public ICommand EditEmployerCommand { get; set; }
    public ICommand DeleteEmployerCommand { get; set; }

    public ICommand CreateInvoiceCommand { get; }

    // Functions
}