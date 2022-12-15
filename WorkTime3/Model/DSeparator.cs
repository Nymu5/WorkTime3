using WorkTime3.Core;

namespace WorkTime3.Model;

public class DSeparator : ControllerBase
{
    public DSeparator(string separator, string text)
    {
        _separator = separator;
        _text = text;
    }

    private string _separator;
    public string Separator
    {
        get => _separator;
        set => SetProperty(ref _separator, value);
    }

    private string _text;
    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

    public string SeparatorString => $"{_separator} ({_text})";
}