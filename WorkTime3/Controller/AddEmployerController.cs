using WorkTime3.Core;

namespace WorkTime3.Controller;

public class AddEmployerController : ControllerBase
{
    private string _title = "Add Employer";

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
}