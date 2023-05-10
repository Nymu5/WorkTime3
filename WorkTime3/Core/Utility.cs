using MyTime.Model;

namespace MyTime.Core;

public static class Utility
{
    public static async Task SaveAndShare(string documentName, MemoryStream stream)
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), documentName);

        await using var fileStream = File.Create(path);
        stream.Seek(0, SeekOrigin.Begin);
        await stream.CopyToAsync(fileStream);
        await Microsoft.Maui.ApplicationModel.DataTransfer.Share.RequestAsync(new ShareFileRequest
        {
            Title = "Share Data",
            File = new ShareFile(path)
        });
    }

    public static async Task<string> Save(string documentName, MemoryStream stream)
    {
        var directory = documentName.Split("/");
        var workingPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        for (int i = 0; i < directory.Length - 1; i++)
        {
            workingPath += $"/{directory[i]}";
            if (!Directory.Exists(workingPath)) Directory.CreateDirectory(workingPath);
        }
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), documentName);
        await using var fileStrem = File.Create(path);
        stream.Seek(0, SeekOrigin.Begin);
        await stream.CopyToAsync(fileStrem);
        return path;
    }

    public static async Task Share(string path)
    {
        await Microsoft.Maui.ApplicationModel.DataTransfer.Share.RequestAsync(new ShareFileRequest
        {
            Title = "Share Data",
            File = new ShareFile(path),
        });
    }

    public static string StringReplacerProfile(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Replace("%due%", Constants.Settings.DefaultInvoiceDays.ToString())
            .Replace("%name%", Constants.Settings.Name.Trim())
            .Replace("%address%", $"{Constants.Settings.AddressLine1.Trim()}\n{Constants.Settings.AddressLine2.Trim()}")
            .Replace("%taxid%", Constants.Settings.TaxId.Trim())
            .Replace("%bname%", Constants.Settings.BankName.Trim())
            .Replace("%biban%", Constants.Settings.BankIban.Trim())
            .Replace("%bbic%", Constants.Settings.BankBic.Trim());
    }
    
    public static string StringReplacerItem(String value, Time time)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Replace("%date%", time.Start.ToString("d"))
            .Replace("%text%", string.IsNullOrWhiteSpace(time.Text) ? string.Empty : time.Text.Trim());
    }
}