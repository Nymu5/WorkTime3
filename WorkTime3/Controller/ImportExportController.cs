using System.Windows.Input;
using MyTime.Core;
using ReactiveUI;

namespace MyTime.Controller;

public class ImportExportController : ReactiveObject
{
    public ImportExportController()
    {
        Import = new Command(execute: ImportTask);

        Export = new Command(execute: ExportTask);
    }

    private async void ExportTask()
    {
        await Share.RequestAsync(new ShareFileRequest { Title = "MyTime .db3 Backup", File = new ShareFile(Constants.DatabasePath) });
    }

    private async void ImportTask()
    {
        await PickAndMove();
    }

    // Commands
    public ICommand Import { get; }
    public ICommand Export { get; }

    // Properties

    // Functions

    async Task PickAndMove()
    {
        try
        {
            var result = await FilePicker.PickAsync(PickOptions.Default);
            if (result == null) return;
            if (!result.FileName.EndsWith(".db3", StringComparison.OrdinalIgnoreCase))
            {
                await Application.Current!.MainPage!.DisplayAlert("Alert", "Please select .db3 file!", "Cancel");
                return;
            }

            var stream = await result.OpenReadAsync();

            await using (var fileStream = File.Create(Constants.ImportDbPath))
            {
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(fileStream);
                stream.Close();
                fileStream.Close();
            }

            await Constants.Database.ImporterWorkTime(Constants.ImportDbPath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}