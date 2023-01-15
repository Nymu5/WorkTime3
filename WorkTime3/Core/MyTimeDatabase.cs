using DynamicData;
using SQLite;
using SQLiteNetExtensions.Extensions;
using MyTime.Model;
using SQLiteNetExtensionsAsync.Extensions;

namespace MyTime.Core;

public class MyTimeDatabase
{
    private SQLiteAsyncConnection Database;
    private SQLiteConnection DatabaseSync;

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

    public async Task<SourceCache<Employer, String>> GetEmployersAsync()
    {
        await Init();
        SourceCache<Employer, String> employers = new SourceCache<Employer, string>(e => e.Id);
        employers.AddOrUpdate(await Database.GetAllWithChildrenAsync<Employer>());
        return employers;
    }

    public async Task<Employer> GetEmployerAsync(string id)
    {
        await Init();
        return (await Database.GetAllWithChildrenAsync<Employer>(e => e.Id == id)).FirstOrDefault();
        //return await Database.GetWithChildrenAsync<Employer>(id);
        //return await Database.Table<Employer>().Where(e => e.Id == id).FirstOrDefaultAsync();
    }

    public async Task SaveEmployerAsync(Employer employer)
    {
        await Init();
        if (await GetEmployerAsync(employer.Id) != null)
        {
            await Database.UpdateWithChildrenAsync(employer);
            return;
        }

        await Database.InsertWithChildrenAsync(employer);
        //await Database.InsertAsync(employer);
    }

    public async Task<int> DeleteEmployerAsync(Employer employer)
    {
        await Init();
        return await Database.DeleteAsync(employer); 
    }

    public async Task<SourceCache<Time, string>> GetTimesAsync()
    {
        await Init();
        SourceCache<Time, String> times = new SourceCache<Time, string>(e => e.Id);
        times.AddOrUpdate(await Database.GetAllWithChildrenAsync<Time>());
        //times.AddOrUpdate(await Database.Table<Time>().ToListAsync());
        return times;
    }

    public async Task<Time> GetTimeAsync(string id)
    {
        await Init();
        return (await Database.GetAllWithChildrenAsync<Time>(t => t.Id == id)).FirstOrDefault();
        //return await Database.Table<Time>().Where(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveTimeAsync(Time time)
    {
        await Init();
        if (await GetTimeAsync(time.Id) != null)
        {
            return await Database.UpdateAsync(time);
        }

        return await Database.InsertAsync(time);
    }

    public async Task<int> DeleteTimeAsync(Time time)
    {
        await Init();
        return await Database.DeleteAsync(time);
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
            DatabaseSync.UpdateWithChildren(settings); 
            return await Database.UpdateAsync(settings);
        }
        DatabaseSync.InsertWithChildren(settings);
        return await Database.InsertAsync(settings);
    }
}