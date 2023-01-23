using MyTime.Model;

namespace MyTime.Core;

public static class Utility
{
    public static async Task SaveAndShare(string documentName, MemoryStream stream)
    {
        string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), documentName);
             
        using (var fileStream = File.Create(path))
        {
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(fileStream);
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Share Data",
                File = new ShareFile(path)
            });
        }
    }

    public static string StringReplacerProfile(string value, Settings settings)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Replace("%timespan%", Constants.Settings.DefaultInvoiceDays.ToString())
            .Replace("%name%", Constants.Settings.Name.Trim())
            .Replace("%address%", $"{Constants.Settings.AddressLine1.Trim()}\n{Constants.Settings.AddressLine2.Trim()}")
            .Replace("%taxid%", Constants.Settings.TaxId.Trim())
            .Replace("%bname%", Constants.Settings.BankName.Trim())
            .Replace("%biban%", Constants.Settings.BankIban.Trim())
            .Replace("%bbic%", Constants.Settings.BankBic.Trim());
    }
    
    public static string StringReplacerItem(String value, Time time)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Replace("%date%", time.Start.ToString("dd.MM.yyyy"))
            .Replace("%text%", string.IsNullOrWhiteSpace(time.Text) ? string.Empty : time.Text.Trim());
    }
}