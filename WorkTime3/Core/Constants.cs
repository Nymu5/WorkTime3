using DynamicData;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SQLite;
using MyTime.Model;
using SkiaSharp;

namespace MyTime.Core;

public static class Constants 
{
    private const string DatabaseFilename = "MyTime.db3";
    private const string ImportDatabaseFilename = "ImportDB.db3";

    public const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;

    public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    public static string ImportDbPath => Path.Combine(FileSystem.AppDataDirectory, ImportDatabaseFilename);

    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }

    public static SourceCache<Employer, string> Employers;
    public static SourceCache<Time, string> Times;
    public static Settings Settings;
    public static MyTimeDatabase Database;
    
    public static readonly SKColor[] Colors = {
        SKColor.Parse("F8B195"),
        SKColor.Parse("F67280"),
        SKColor.Parse("6C5B7B"),
        SKColor.Parse("C06C84"),
        SKColor.Parse("355C7D"),
        SKColor.Parse("FE4365"),
        SKColor.Parse("FC9D9A"),
        // ReSharper disable once StringLiteralTypo
        SKColor.Parse("F9CDAD"),
        SKColor.Parse("C8C8A9"),
        SKColor.Parse("83AF9B"),
    };
    
    public static Axis[] XAxes { get; set; } =
    {
        new Axis
        {
            Labels = new[]
                { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" },
            LabelsRotation = 90,
            SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
            SeparatorsAtCenter = false,
            TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
            TicksAtCenter = true,
            TextSize = 40,
        }
    };

    public static Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            TextSize = 40,
            //Labeler = Labelers.Currency,
            MinLimit = 0,
        }
    };
}