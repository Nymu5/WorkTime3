using System.Windows.Input;
using DynamicData;
using MyTime.Core;
using MyTime.Model;
using ReactiveUI;

namespace MyTime.Controller;

public class ShellController : ReactiveObject
{
    public ShellController()
    {
        Constants.Database = new MyTimeDatabase();
        Constants.Employers = new SourceCache<Employer, string>(e => e.Id);
        Constants.Times = new SourceCache<Time, string>(t => t.Id);
        Constants.Charts = new SourceCache<ChartData, int>(c => c.Year);
        
        DatabaseSetupCommand = new Command(execute: async () =>
        {
            var settings = await Constants.Database.GetSettingsAsync();
            if (settings.Count == 0)
            {
                Constants.Settings = new Settings
                {
                    Id = Settings.getUUID(),
                };
                await Constants.Database.SaveProfileAsync(Constants.Settings);
            }
            else Constants.Settings = settings[0];

            Constants.Employers.AddOrUpdate(await Constants.Database.GetEmployersAsync());
            Constants.Times.AddOrUpdate(await Constants.Database.GetTimesAsync());
            Console.WriteLine("Database loaded successfully");
        });
    }

    public ICommand DatabaseSetupCommand { get; set; }
}