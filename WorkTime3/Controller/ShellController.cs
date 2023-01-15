using System.Windows.Input;
using MyTime.Core;

namespace MyTime.Controller;

public class ShellController : ControllerBase
{
    private MyTimeDatabase _db;

    public ShellController()
    {
        _db = new MyTimeDatabase();
        DatabaseSetupCommand = new Command(canExecute: () => true, execute: async () =>
        {
            Constants.Settings = await _db.LoadProfileAsync();
        });
    }
    
    public ICommand DatabaseSetupCommand { get; set; }
    
}