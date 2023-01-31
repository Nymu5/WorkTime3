using ReactiveUI;

namespace MyTime.Model;

public class ChartData : ReactiveObject
{
    private int _year;
    public int Year
    {
        get => _year;
        set => this.RaiseAndSetIfChanged(ref _year, value);
    }
    
    private string _htmlElement;
    public string HtmlElement
    {
        get => _htmlElement;
        set => this.RaiseAndSetIfChanged(ref _htmlElement, value);
    }

}