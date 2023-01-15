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
}