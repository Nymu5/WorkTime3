using SQLite;
using SQLiteNetExtensions.Attributes;
using MyTime.Core;

namespace MyTime.Model;

public class Settings : ControllerBase
{
    public Settings()
    {
        _id = String.Empty;
        _name = String.Empty;
        _taxId = String.Empty;
        _addressLine1 = String.Empty;
        _addressLine2 = String.Empty;
        _bankName = String.Empty;
        _bankIban = String.Empty;
        _bankBic = String.Empty;
    }

    private string _id;
    [PrimaryKey]
    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _taxId;
    public string TaxId
    {
        get => _taxId;
        set => SetProperty(ref _taxId, value);
    }

    private string _addressLine1;
    public string AddressLine1
    {
        get => _addressLine1;
        set => SetProperty(ref _addressLine1, value);
    }

    private string _addressLine2;
    public string AddressLine2
    {
        get => _addressLine2;
        set => SetProperty(ref _addressLine2, value);
    }

    private string _bankName;
    public string BankName
    {
        get => _bankName;
        set => SetProperty(ref _bankName, value);
    }

    private string _bankIban;
    public string BankIban
    {
        get => _bankIban;
        set => SetProperty(ref _bankIban, value);
    }

    private string _bankBic;
    public string BankBic
    {
        get => _bankBic;
        set => SetProperty(ref _bankBic, value);
    }
}