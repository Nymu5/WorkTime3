using SQLite;

namespace WorkTime3.Core;

public static class Constants
{
    private const string DatabaseFilename = "WorkTime3.db3";

    public const SQLite.SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;

    public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
}