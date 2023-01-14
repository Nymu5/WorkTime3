using DynamicData;
using SQLite;
using SQLiteNetExtensions.Extensions;
using MyTime.Model;

namespace MyTime.Core;

public class MyTimeDatabase
{
    private SQLiteAsyncConnection Database;
    private SQLiteConnection DatabaseSync;

    public MyTimeDatabase()
    {
        _currencies = new List<Currency>
        {
            new Currency(Guid.NewGuid().ToString(), "€", "Euro"),
            new Currency(Guid.NewGuid().ToString(), "$", "Dollar")
        };
        _dSeparators = new List<DSeparator>
        {
            new DSeparator(Guid.NewGuid().ToString(), ",", "EU"),
            new DSeparator(Guid.NewGuid().ToString(), ".", "US")
        };
    }

    private List<Currency> _currencies;
    private List<DSeparator> _dSeparators;

    async Task Init()
    {
        if (Database is not null) return;

        Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        DatabaseSync = new SQLiteConnection(Constants.DatabasePath, Constants.Flags);

        await Database.CreateTableAsync<Employer>();
        await Database.CreateTableAsync<Time>();
        await Database.CreateTableAsync<Settings>();
        await Database.CreateTableAsync<DSeparator>();
        await Database.CreateTableAsync<Currency>();
        
    }

    public async Task<int> PreConfigConstantsAsync()
    {
        await Init();
        foreach (Currency currency in _currencies)
        {
            if (await GetCurrencyByTextAsync(currency.Text) == null) await Database.InsertAsync(currency);
        }

        foreach (DSeparator dSeparator in _dSeparators)
        {
            if (await GetDSeperatorByTextAsync(dSeparator.Text) == null) await Database.InsertAsync(dSeparator);
        }

        return 0;
    }

    public async Task<List<DSeparator>> GetDSeparatorsAsync()
    {
        await Init();
        return await Database.Table<DSeparator>().ToListAsync();
    }

    public async Task<DSeparator> GetDSeperatorByTextAsync(string text)
    {
        await Init();
        return await Database.Table<DSeparator>().Where(d => d.Text == text).FirstOrDefaultAsync();
    }

    public async Task<List<Currency>> GetCurrenciesAsync()
    {
        await Init();
        return await Database.Table<Currency>().ToListAsync();
    }

    public async Task<Currency> GetCurrencyByTextAsync(string text)
    {
        await Init();
        return await Database.Table<Currency>().Where(c => c.Text == text).FirstOrDefaultAsync(); 
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
        if (String.IsNullOrWhiteSpace(employer.Name)) return -1; 
        if (await GetEmployerAsync(employer.Id) != null)
        {
            return await Database.UpdateAsync(employer);
        }
        return await Database.InsertAsync(employer);
    }

    public async Task<int> DeleteEmployerAsync(Employer employer)
    {
        await Init();
        return await Database.DeleteAsync(employer); 
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
        DatabaseSync.GetChild(settings, nameof(Currency));
        DatabaseSync.GetChild(settings, nameof(DSeparator));

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