using DynamicData;
using SQLite;
using MyTime.Model;
using ReactiveUI;
using SkiaSharp;

namespace MyTime.Core;

public static class Constants 
{
    private const string DatabaseFilename = "MyTime.db3";
    private const string ImportDatabaseFilename = "ImportDB.db3";

    public const SQLite.SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;

    public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    public static string ImportDbPath => Path.Combine(FileSystem.AppDataDirectory, ImportDatabaseFilename);

    public static Settings Settings;

    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }

    public static string TsFormatter(TimeSpan ts)
    {
        return $"{(int)ts.TotalHours}:{((int)ts.Minutes).ToString("00")} h";
    }

    public static SourceCache<Employer, string> Employers;
    public static SourceCache<Time, string> Times;
    public static MyTimeDatabase Database;
    
    public static SKColor[] Colors = new SKColor[]
    {
        SKColor.Parse("F8B195"),
        SKColor.Parse("F67280"),
        SKColor.Parse("6C5B7B"),
        SKColor.Parse("C06C84"),
        SKColor.Parse("355C7D"),
        SKColor.Parse("FE4365"),
        SKColor.Parse("FC9D9A"),
        SKColor.Parse("F9CDAD"),
        SKColor.Parse("C8C8A9"),
        SKColor.Parse("83AF9B"),
    };
}