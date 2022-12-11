using System.Windows.Input;
using WorkTime3.Core;

namespace WorkTime3.Controller;

public class MainController : ControllerBase
{
    private string _txt = "Mole";

    public MainController()
    {
        TestCommand = new Command(
            () =>
            {
                Txt = "Bimbo";
                (TestCommand as Command)?.ChangeCanExecute();
                Console.WriteLine();
            },
            () => _txt != "Bimbo");
    }

    // Commands
    public ICommand TestCommand { get; set; }

    public string Txt
    {
        get => _txt;
        set
        {
            SetProperty(ref _txt, value);
            (TestCommand as Command)?.ChangeCanExecute();
        }
    }
}