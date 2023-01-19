using DynamicData;
using SQLite;
using SQLiteNetExtensions.Extensions;
using MyTime.Model;
using MyTime.Core;
using SQLiteNetExtensionsAsync.Extensions;
using Employer = MyTime.Model.Employer;
using Item = MyTime.Model.Time;


namespace MyTime.Core;

public class MyTimeDatabase
{
    private SQLiteAsyncConnection Database;
    private SQLiteConnection DatabaseSync;
    private SQLiteAsyncConnection ImportDatabase;

    public MyTimeDatabase()
    {
    }

    async Task Init()
    {
        if (Database is not null) return;

        Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        DatabaseSync = new SQLiteConnection(Constants.DatabasePath, Constants.Flags);

        await Database.CreateTableAsync<Employer>();
        await Database.CreateTableAsync<Time>();
        await Database.CreateTableAsync<Settings>();
    }

    public async Task<Dictionary<string, object>> Importer(string path)
    {
        await Init();
        if (ImportDatabase is null) ImportDatabase = new SQLiteAsyncConnection(path, Constants.Flags);

        try
        {
            List<MyTime.Core.WT3Core.Employer> importEmployers =
                await ImportDatabase.GetAllWithChildrenAsync<MyTime.Core.WT3Core.Employer>();
            foreach (MyTime.Core.WT3Core.Employer oldEmployer in importEmployers)
            {
                Employer employer = new Employer
                {
                    AddressLine1 = oldEmployer.Address.Split("\n").Length < 2
                        ? oldEmployer.Address
                        : oldEmployer.Address.Split("\n")[0],
                    AddressLine2 = oldEmployer.Address.Split("\n").Length < 2
                        ? String.Empty
                        : oldEmployer.Address.Split("\n")[1],
                    Description = oldEmployer.Description,
                    EmployerNb = long.Parse(oldEmployer.EmployerNumber),
                    Id = Employer.getUUID(),
                    Name = oldEmployer.Name,
                    Salary = (float)oldEmployer.Salary,
                    Times = new List<Time>()
                };
                await this.SaveEmployerAsync(employer);
                List<MyTime.Core.WT3Core.Item> itemsOld =
                    await ImportDatabase.GetAllWithChildrenAsync<MyTime.Core.WT3Core.Item>(i =>
                        i.EmployerId == oldEmployer.Id);
                foreach (MyTime.Core.WT3Core.Item itemOld in itemsOld)
                {
                    Time time = new Time
                    {
                        Id = Time.getUUID(),
                        Description = itemOld.Description,
                        Employer = employer,
                        Start = new DateTime(itemOld.StartDate.Year, itemOld.StartDate.Month, itemOld.StartDate.Day,
                            itemOld.StartTime.Hours, itemOld.StartDate.Minute, 0),
                        End = new DateTime(itemOld.EndDate.Year, itemOld.EndDate.Month, itemOld.EndDate.Day,
                            itemOld.EndTime.Hours, itemOld.EndTime.Minutes, 0),
                        Salary = (float)itemOld.Salary,
                        Text = itemOld.Text
                    };
                    employer.Times.Add(time);
                    await this.SaveTimeAsync(time);
                    await this.SaveEmployerAsync(employer);
                }

                Console.WriteLine($"{oldEmployer.Name}: {itemsOld.Count}");
            }

            await ImportDatabase.CloseAsync();
            File.Delete(path);
            return new Dictionary<string, object>
            {
                { "code", 0 },
                { "message", "import successful" }
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await ImportDatabase.CloseAsync();
            File.Delete(path);
            return new Dictionary<string, object>
            {
                { "code", -1 },
                { "message", e }
            };
        }
    }

    public async Task<List<Employer>> GetEmployersAsync()
    {
        await Init();
        return (await Database.GetAllWithChildrenAsync<Employer>()).OrderBy(e => e.Name).ToList();
    }

    public async Task<Employer> GetEmployerAsync(string id)
    {
        await Init();
        return (await Database.GetAllWithChildrenAsync<Employer>(e => e.Id == id)).FirstOrDefault();
    }

    public async Task SaveEmployerAsync(Employer employer)
    {
        await Init();
        if (await GetEmployerAsync(employer.Id) != null) await Database.UpdateWithChildrenAsync(employer);
        else await Database.InsertWithChildrenAsync(employer);
        Constants.Employers.AddOrUpdate(employer);
    }

    public async Task DeleteEmployerAsync(Employer employer)
    {
        await Init();
        await Database.DeleteAsync(employer, recursive: true);
        Constants.Times.RemoveWhere(t => t.Employer.Id == employer.Id);
        Constants.Employers.Remove(employer);
    }

    public async Task<List<Time>> GetTimesAsync()
    {
        await Init();
        return (await Database.GetAllWithChildrenAsync<Time>()).OrderByDescending(t => t.Start).ToList();
    }

    public async Task<Time> GetTimeAsync(string id)
    {
        await Init();
        return (await Database.GetAllWithChildrenAsync<Time>(t => t.Id == id)).FirstOrDefault();
    }

    public async Task SaveTimeAsync(Time time)
    {
        await Init();
        if (await GetTimeAsync(time.Id) != null) await Database.UpdateAsync(time);
        else await Database.InsertAsync(time);
        Constants.Times.AddOrUpdate(time);
    }

    public async Task DeleteTimeAsync(Time time)
    {
        await Init();
        await Database.DeleteAsync(time);
        Constants.Times.Remove(time);
    }

    public async Task<Settings> LoadProfileByIdAsync(string id)
    {
        await Init();
        return await Database.Table<Settings>().Where(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Settings> LoadProfileAsync()
    {
        await Init();
        Settings settings = await Database.Table<Settings>().FirstOrDefaultAsync();
        if (settings == null)
        {
            settings = new Settings();
            settings.Id = Guid.NewGuid().ToString();
            await SaveProfileAsync(settings);
            settings = await Database.Table<Settings>().FirstOrDefaultAsync();
        }

        return settings;
    }

    public async Task<int> SaveProfileAsync(Settings settings)
    {
        await Init();
        if (await LoadProfileByIdAsync(settings.Id) != null)
        {
            return await Database.UpdateAsync(settings);
        }

        return await Database.InsertAsync(settings);
    }
}