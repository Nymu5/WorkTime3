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
            await _db.PreConfigConstantsAsync();
            Constants.Currencies = await _db.GetCurrenciesAsync();
            Constants.DSeparators = await _db.GetDSeparatorsAsync();
        });
    }
    
    public ICommand DatabaseSetupCommand { get; set; }
    
}