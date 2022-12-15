using WorkTime3.Core;

namespace WorkTime3.Model;

public class Currency : ControllerBase
{
    public Currency(string symbol, string text)
    {
        _symbol = symbol;
        _text = text;
    }

    private string _symbol;
    public string Symbol
    {
        get => _symbol;
        set => SetProperty(ref _symbol, value);
    }

    private string _text;
    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

    public string DisplayString => $"{_symbol} ({_text})";
}