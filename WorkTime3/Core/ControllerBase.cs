using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyTime.Core;

public class ControllerBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(storage, value)) return false;
        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public void OnPropertyChanged([CallerMemberName] string properyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(properyName));
    }
}