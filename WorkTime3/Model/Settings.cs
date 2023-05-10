using SQLite;
using ReactiveUI;

namespace MyTime.Model;

public class Settings : ReactiveObject
{
    private readonly string _id;

    [PrimaryKey]
    public string Id
    {
        get => _id;
        init => this.RaiseAndSetIfChanged(ref _id, value);
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
    
    private string _lastInvoice;
    public string LastInvoice
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
    
    private string _companyName;
    public string CompanyName
    {
        get => _companyName;
        set => this.RaiseAndSetIfChanged(ref _companyName, value);
    }
    
    private string _phoneNumber;
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => this.RaiseAndSetIfChanged(ref _phoneNumber, value);
    }
    
    private string _emailAddress;
    public string EmailAddress
    {
        get => _emailAddress;
        set => this.RaiseAndSetIfChanged(ref _emailAddress, value);
    }
    
    private string _contactName;
    public string ContactName
    {
        get => _contactName;
        set => this.RaiseAndSetIfChanged(ref _contactName, value);
    }
    
    private string _managingDirector;
    public string ManagingDirector
    {
        get => _managingDirector;
        set => this.RaiseAndSetIfChanged(ref _managingDirector, value);
    }
    
    private string _introductionText;
    public string IntroductionText
    {
        get => _introductionText;
        set => this.RaiseAndSetIfChanged(ref _introductionText, value);
    }
    
    public static string GetUuid()
    {
        return Guid.NewGuid().ToString();
    }

    [Ignore] public string PersonalDetailString => $"{Name}\n{AddressLine1}\n{AddressLine2}\n\nTax-ID\n{TaxId}";
    [Ignore] public string BankDetailString => $"{BankName}\nIBAN: {BankIban}\nBIC: {BankBic}";
}