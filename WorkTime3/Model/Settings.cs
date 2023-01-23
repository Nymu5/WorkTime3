using SQLite;
using SQLiteNetExtensions.Attributes;
using MyTime.Core;
using ReactiveUI;

namespace MyTime.Model;

public class Settings : ReactiveObject
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
        _lastInvoice = 0;
        _informationText = String.Empty;
    }

    private string _id;

    [PrimaryKey]
    public string Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    private string _name;

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    private string _taxId;

    public string TaxId
    {
        get => _taxId;
        set => this.RaiseAndSetIfChanged(ref _taxId, value);
    }
    
    private long _lastInvoice;
    public long LastInvoice
    {
            get => _lastInvoice;
            set => this.RaiseAndSetIfChanged(ref _lastInvoice, value);
    }

    private string _addressLine1;

    public string AddressLine1
    {
        get => _addressLine1;
        set => this.RaiseAndSetIfChanged(ref _addressLine1, value);
    }

    private string _addressLine2;

    public string AddressLine2
    {
        get => _addressLine2;
        set => this.RaiseAndSetIfChanged(ref _addressLine2, value);
    }

    private string _bankName;

    public string BankName
    {
        get => _bankName;
        set => this.RaiseAndSetIfChanged(ref _bankName, value);
    }

    private string _bankIban;

    public string BankIban
    {
        get => _bankIban;
        set => this.RaiseAndSetIfChanged(ref _bankIban, value);
    }

    private string _bankBic;

    public string BankBic
    {
        get => _bankBic;
        set => this.RaiseAndSetIfChanged(ref _bankBic, value);
    }
    
    private string _informationText;
    public string InformationText
    {
        get => _informationText;
        set => this.RaiseAndSetIfChanged(ref _informationText, value);
    }
    
    private int _defaultInvoiceDays;
    public int DefaultInvoiceDays
    {
        get => _defaultInvoiceDays;
        set => this.RaiseAndSetIfChanged(ref _defaultInvoiceDays, value);
    }

    [Ignore] public string PersonalDetailString => $"{Name}\n{AddressLine1}\n{AddressLine2}\n\nTax-ID\n{TaxId}";
    [Ignore] public string BankDetailString => $"{BankName}\nIBAN: {BankIban}\nBIC: {BankBic}";
}