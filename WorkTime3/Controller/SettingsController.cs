using System.Windows.Input;
using MyTime.Core;
using MyTime.Model;

namespace MyTime.Controller;

public class SettingsController : ControllerBase
{
    private MyTimeDatabase _db;
    private Settings _settings;

    public SettingsController()
    {
        _db = new MyTimeDatabase();


        SettingSelectedCommand = new Command<string>(canExecute: (string setting) => true,
            execute: async (string setting) =>
            {
                _settings = await _db.LoadProfileAsync();
                switch (setting)
                {
                    case "personal":
                        await Shell.Current.GoToAsync("PersonalDetailsPage", new Dictionary<string, object>
                        {
                            { "settings", _settings },
                            { "db", _db }
                        });
                        break;
                    case "bank":
                        await Shell.Current.GoToAsync("BankInformationPage", new Dictionary<string, object>
                        {
                            { "settings", _settings }
                        });
                        break;
                    case "iesettings":
                        await Shell.Current.GoToAsync("ImportExportPage", new Dictionary<string, object>
                        {
                            { "settings", _settings }
                        });
                        break;
                }
            });
    }

    public ICommand SettingSelectedCommand { get; set; }
}