using System.Windows.Input;
using MyTime.Core;

namespace MyTime.Controller;

public class ImportExportController : ControllerBase
{
    private MyTimeDatabase _db;
    
    public ImportExportController()
    {
        _db = new MyTimeDatabase();

        Import = new Command(execute: async () =>
        {
            await PickAndMove(PickOptions.Default);
        });

        Export = new Command(execute: async () =>
        {
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "MyTime .db3 Backup",
                File = new ShareFile(Constants.DatabasePath)
            });
        });
    }
    
    // Commands
    public ICommand Import { get; }
    public ICommand Export { get; }
    
    // Properties

    private FilePickerFileType _db3FileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.iOS, new[] { "application/vnd.sqlite3" } }
    });

    // Functions

    async Task<FileResult> PickAndMove(PickOptions options)
    {
        try
        {
            var result = await FilePicker.PickAsync(PickOptions.Default);
            if (result == null) return null;
            if (!result.FileName.EndsWith(".db3", StringComparison.OrdinalIgnoreCase))
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Please select .db3 file!", "Cancel");
                return null;
            }

            var stream = await result.OpenReadAsync();
            
            using (var fileStream = File.Create(Constants.ImportDbPath))
            {
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(fileStream);
                stream.Close();
                fileStream.Close();
            }
            await _db.Importer(Constants.ImportDbPath);
            
            return result;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}