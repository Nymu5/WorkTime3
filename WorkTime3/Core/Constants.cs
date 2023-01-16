using SQLite;
using MyTime.Model;

namespace MyTime.Core;

public static class Constants
{
    private const string DatabaseFilename = "MyTime.db3";

    public const SQLite.SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;

    public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

    public static Settings Settings;
    
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }

    public static string TsFormatter(TimeSpan ts)
    {
        return $"{(int)ts.TotalHours}:{((int)ts.Minutes).ToString("00")} h";
    }
}