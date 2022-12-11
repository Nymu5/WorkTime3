using DynamicData;
using SQLite;
using WorkTime3.Model;

namespace WorkTime3.Core;

public class WorkTime3Database
{
    private SQLiteAsyncConnection Database;

    public WorkTime3Database()
    {
        
    }

    async Task Init()
    {
        if (Database is not null) return;

        Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        var result = await Database.CreateTableAsync<Employer>(); 
    }

    public async Task<SourceCache<Employer, String>> GetEmployersAsync()
    {
        await Init();
        SourceCache<Employer, String> employers = new SourceCache<Employer, string>(e => e.Id);
        employers.AddOrUpdate(await Database.Table<Employer>().ToListAsync());
        return employers;
    }

    public async Task<Employer> GetEmployerAsync(string id)
    {
        await Init();
        return await Database.Table<Employer>().Where(e => e.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveEmployerAsync(Employer employer)
    {
        await Init();
        if (String.IsNullOrWhiteSpace(employer.Id) && String.IsNullOrWhiteSpace(employer.Name))
        {
            return await Database.UpdateAsync(employer);
        }
        return await Database.InsertAsync(employer);
    }

    public async Task<int> DeleteItemAsync(Employer employer)
    {
        await Init();
        return await Database.DeleteAsync(employer); 
    }
}