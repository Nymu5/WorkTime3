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
        if (await GetEmployerAsync(employer.Id) != null)
        {
            await Database.UpdateWithChildrenAsync(employer);
            return;
        }

        await Database.InsertWithChildrenAsync(employer);
    }

    public async Task DeleteEmployerAsync(Employer employer)
    {
        await Init();
        await Database.DeleteAsync(employer, recursive: true);
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
        if (await GetTimeAsync(time.Id) != null)
        {
            await Database.UpdateAsync(time);
            return;
        }
        await Database.InsertAsync(time);
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