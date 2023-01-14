using System.ComponentModel.DataAnnotations.Schema;
using SQLite;
using MyTime.Core;

namespace MyTime.Model;

public class Currency : ControllerBase
{
    public Currency()
    {
        _id = String.Empty;
        _symbol = String.Empty;
        _text = String.Empty;
    }
    public Currency(string id, string symbol, string text)
    {
        _id = id;
        _symbol = symbol;
        _text = text;
    }

    private string _id;
    [PrimaryKey]
    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
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