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
        
        DatabaseSetupCommand = new Command(execute: async () =>
        {
            Constants.Settings = await Constants.Database.LoadProfileAsync();
            Constants.Employers.AddOrUpdate(await Constants.Database.GetEmployersAsync());
            Constants.Times.AddOrUpdate(await Constants.Database.GetTimesAsync());
            Console.WriteLine("Database loaded successfully");
        });
    }

    public ICommand DatabaseSetupCommand { get; set; }
}