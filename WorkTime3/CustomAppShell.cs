using MyTime.View;

namespace MyTime;

public class CustomAppShell : AppShell
{
    protected override async void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);
        ShellNavigatingDeferral token = args.GetDeferral();
        var result = await DisplayActionSheet("Navigate?", "Cancel", "Yes", "No");
        if (result != "Yes")
        {
            await Shell.Current.Navigation.PushAsync(new EmployerPage());
            args.Cancel();
        }

        token.Complete();
    }
}