using DynamicData;
using SQLite;
using MyTime.Model;
using SQLiteNetExtensionsAsync.Extensions;
using Employer = MyTime.Model.Employer;
using Item = MyTime.Model.Time;


namespace MyTime.Core;

public class MyTimeDatabase
{
    private SQLiteAsyncConnection _database;
    private SQLiteAsyncConnection _importDatabase;

    async Task Init()
    {
        if (_database is not null) return;

        _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

        await _database.CreateTableAsync<Employer>();
        await _database.CreateTableAsync<Time>();
        await _database.CreateTableAsync<Settings>();
    }

    public async Task ImporterWorkTime(string path)
    {
        await Init();
        _importDatabase ??= new SQLiteAsyncConnection(path, Constants.Flags);

        try
        {
            List<MyTime.Core.WT3Core.Employer> importEmployers =
                await _importDatabase.GetAllWithChildrenAsync<MyTime.Core.WT3Core.Employer>();
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
                    Id = Employer.GetUuid(),
                    Name = oldEmployer.Name,
                    Salary = (float)oldEmployer.Salary,
                    Times = new List<Time>()
                };
                await this.SaveEmployerAsync(employer, true);
                List<MyTime.Core.WT3Core.Item> itemsOld =
                    await _importDatabase.GetAllWithChildrenAsync<MyTime.Core.WT3Core.Item>(i =>
                        i.EmployerId == oldEmployer.Id);
                foreach (MyTime.Core.WT3Core.Item itemOld in itemsOld)
                {
                    Time time = new Time
                    {
                        Id = Time.GetUuid(),
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
                    await this.SaveTimeAsync(time, true);
                    await this.SaveEmployerAsync(employer, true);
                }

                Console.WriteLine($"{oldEmployer.Name}: {itemsOld.Count}");
            }

            await _importDatabase.CloseAsync();
            File.Delete(path);
            await CleanFetch();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await _importDatabase.CloseAsync();
            await ImporterMyTime(path);
        }
    }
    
    public async Task ImporterMyTime (string path)
    {
        await Init();
        _importDatabase ??= new SQLiteAsyncConnection(path, Constants.Flags);

        try
        {
            List<Employer> importEmployers = await _importDatabase.GetAllWithChildrenAsync<Employer>();
            foreach (Employer oEmployer in importEmployers)
            {
                await this.SaveEmployerAsync(oEmployer, true);
                List<Time> importTimes =
                    await _importDatabase.GetAllWithChildrenAsync<Time>(i => i.EmployerId == oEmployer.Id);
                foreach (Time oTime in importTimes)
                {
                    oEmployer.Times.Add(oTime);
                    await this.SaveTimeAsync(oTime, true);
                    await this.SaveEmployerAsync(oEmployer, true);
                }
                Console.WriteLine($"{oEmployer.Name}: {importTimes.Count}");
            }

            await _importDatabase.CloseAsync();
            File.Delete(path);
            await CleanFetch();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await _importDatabase.CloseAsync();
            File.Delete(path);
        }
    }

    private async Task<Settings> GetSettingsAsync(string id)
    {
        await Init();
        return await _database.Table<Settings>().Where(s => s.Id == id).FirstOrDefaultAsync(); 
    }

    public async Task<List<Settings>> GetSettingsAsync()
    {
        await Init();
        return await _database.GetAllWithChildrenAsync<Settings>(); 
    }

    public async Task SaveProfileAsync(Settings settings)
    {
        await Init();
        if (await GetSettingsAsync(settings.Id) != null) await _database.UpdateWithChildrenAsync(settings);
        else await _database.InsertWithChildrenAsync(settings);
    }

    public async Task<List<Employer>> GetEmployersAsync()
    {
        await Init();
        return (await _database.GetAllWithChildrenAsync<Employer>()).OrderBy(e => e.Name).ToList();
    }

    private async Task<Employer> GetEmployerAsync(string id)
    {
        await Init();
        return (await _database.GetAllWithChildrenAsync<Employer>(e => e.Id == id)).FirstOrDefault();
    }

    public async Task SaveEmployerAsync(Employer employer, bool skip = false)
    {
        await Init();
        if (await GetEmployerAsync(employer.Id) != null) await _database.UpdateWithChildrenAsync(employer);
        else await _database.InsertWithChildrenAsync(employer);
        //Constants.Employers.AddOrUpdate(employer);
        if (!skip) await CleanFetch();
    }

    public async Task DeleteEmployerAsync(Employer employer)
    {
        await Init();
        await _database.DeleteAsync(employer, recursive: true);
        //Constants.Times.RemoveWhere(t => t.Employer.Id == employer.Id);
        //Constants.Employers.Remove(Constants.Employers.Lookup(employer.Id).Value);
        await CleanFetch();
    }

    public async Task<List<Time>> GetTimesAsync()
    {
        await Init();
        return (await _database.GetAllWithChildrenAsync<Time>()).OrderByDescending(t => t.Start).ToList();
    }

    private async Task<Time> GetTimeAsync(string id)
    {
        await Init();
        return (await _database.GetAllWithChildrenAsync<Time>(t => t.Id == id)).FirstOrDefault();
    }

    public async Task SaveTimeAsync(Time time, bool skip = false)
    {
        await Init();
        if (await GetTimeAsync(time.Id) != null) await _database.UpdateAsync(time);
        else await _database.InsertAsync(time);
        //Constants.Times.AddOrUpdate(time);
        if (!skip) await CleanFetch();
    }

    public async Task DeleteTimeAsync(Time time)
    {
        await Init();
        await _database.DeleteAsync(time);
        //Constants.Times.Remove(time);
        await CleanFetch();
    }

    private async Task CleanFetch()
    {
        Constants.Employers.Clear();
        Constants.Employers.AddOrUpdate(await GetEmployersAsync());
        Constants.Times.Clear();
        Constants.Times.AddOrUpdate(await GetTimesAsync());
        
    }
}