using SQLite;
using MyTime.Core;

namespace MyTime.Model;

public class DSeparator : ControllerBase
{
    public DSeparator()
    {
        _id = String.Empty;
        _separator = String.Empty;
        _text = String.Empty;
    }
    public DSeparator(string id, string separator, string text)
    {
        _id = id;
        _separator = separator;
        _text = text;
    }

    private string _id;
    [PrimaryKey]
    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
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