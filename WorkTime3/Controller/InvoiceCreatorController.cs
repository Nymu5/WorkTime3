using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using MyTime.Core;
using MyTime.Model;
using ReactiveUI;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Border = Syncfusion.DocIO.DLS.Border;
using Extensions = MyTime.Core.Extensions;
using HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment;
using Settings = MyTime.Model.Settings;
using Tab = Syncfusion.DocIO.DLS.Tab;

// ReSharper disable All

namespace MyTime.Controller;

[QueryProperty(nameof(Employer), "Employer")]
[QueryProperty(nameof(Settings), "Settings")]
public class InvoiceCreatorController : ReactiveObject
{
    public InvoiceCreatorController()
    {
        DateTime reference = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0, 0, 0);
        DateTime last = new DateTime((reference - TimeSpan.FromDays(1)).Year, (reference - TimeSpan.FromDays(1)).Month,
            1, 0, 0, 0, 0, 0);
        Start = last;
        End = reference - TimeSpan.FromDays(1);

        CreateInvoiceCommand = new Command(execute: CreateInvoice);
        SaveInvoiceCommand = new Command(execute: SaveInvoice);
        ToggleCheckedCommand = new Command(execute: ToggleChecked);
        ToggleVatCommand = new Command(execute: ToggleVat);
        ToggleContactNameCommand = new Command(execute: ToggleContactName);

        Func<Time, bool> StartFilter(DateTime date) => time => time.Start >= Start;
        var startPredicate = this.WhenAnyValue(x => x.Start)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.TaskpoolScheduler)
            .DistinctUntilChanged()
            .Select(StartFilter);

        Func<Time, bool> EndFilter(DateTime date) => time => time.Start <= End;
        var endPredicate = this.WhenAnyValue(x => x.End)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.TaskpoolScheduler)
            .DistinctUntilChanged()
            .Select(EndFilter);

        Func<Time, bool> EmployerFilter(Employer employer) => time => (employer == null || time.Employer.Id == employer.Id);
        var employerPredicate = this.WhenAnyValue(x => x.Employer)
            .DistinctUntilChanged()
            .Select(EmployerFilter);
        
        Constants.Times
            .Connect()
            .Filter(startPredicate)
            .Filter(endPredicate)
            .Filter(employerPredicate)
            .Sort(SortExpressionComparer<Time>.Ascending(t => t.Start))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out Times)
            .Subscribe();

        LastInvoice = Constants.Settings.LastInvoice;
    }
    
    // Commands
    public ICommand CreateInvoiceCommand { get; }
    public ICommand SaveInvoiceCommand { get; }
    public ICommand ToggleCheckedCommand { get; }
    public ICommand ToggleVatCommand { get; }
    
    public ICommand ToggleContactNameCommand { get; }
    
    // Properties

    public ReadOnlyObservableCollection<Time> Times;
    
    private Settings _settings;
    public Settings Settings
    {
        get => _settings;
        set => this.RaiseAndSetIfChanged(ref _settings, value);
    }

    private Employer _employer;
    public Employer Employer
    {
        get => _employer;
        set => this.RaiseAndSetIfChanged(ref _employer, value);
    }
    
    private DateTime _start;
    public DateTime Start
    {
        get => _start;
        set => this.RaiseAndSetIfChanged(ref _start, value);
    }
    
    private DateTime _end;
    public DateTime End
    {
        get => _end;
        set => this.RaiseAndSetIfChanged(ref _end, value);
    }
    
    private string _itemDescription;
    public string ItemDescription
    {
        get => _itemDescription;
        set => this.RaiseAndSetIfChanged(ref _itemDescription, value);
    }
    
    private string _invoiceNumber;
    public string InvoiceNumber
    {
        get => _invoiceNumber;
        set => this.RaiseAndSetIfChanged(ref _invoiceNumber, value);
    }
    
    private bool _oneItem;
    public bool OneItem
    {
        get => _oneItem;
        set => this.RaiseAndSetIfChanged(ref _oneItem, value);
    }
    
    private bool _invoiceCreated;
    public bool InvoiceCreated
    {
        get => _invoiceCreated;
        set => this.RaiseAndSetIfChanged(ref _invoiceCreated, value);
    }
    
    private string _pdfPath;
    public string PdfPath
    {
        get => _pdfPath;
        set => this.RaiseAndSetIfChanged(ref _pdfPath, value);
    }
    
    private string _wordPath;
    public string WordPath
    {
        get => _wordPath;
        set => this.RaiseAndSetIfChanged(ref _wordPath, value);
    }
    
    private Stream _pdfStream;
    public Stream PdfStream
    {
        get => _pdfStream;
        set => this.RaiseAndSetIfChanged(ref _pdfStream, value);
    }
    
    private string _lastInvoice;
    public string LastInvoice
    {
        get => _lastInvoice;
        set => this.RaiseAndSetIfChanged(ref _lastInvoice, value);
    }
    
    private bool _includeVat;
    public bool IncludeVat
    {
        get => _includeVat;
        set => this.RaiseAndSetIfChanged(ref _includeVat, value);
    }
    
    private double _vatValue = 19;
    public double VatValue
    {
        get => _vatValue;
        set => this.RaiseAndSetIfChanged(ref _vatValue, value);
    }
    
    private bool _includeContactName = true;
    public bool IncludeContactName
    {
        get => _includeContactName;
        set => this.RaiseAndSetIfChanged(ref _includeContactName, value);
    }
    
    // Functions
    public void ToggleChecked()
    {
        OneItem = !OneItem; 
    }

    public void ToggleVat()
    {
        IncludeVat = !IncludeVat;
    }

    public void ToggleContactName()
    {
        IncludeContactName = !IncludeContactName;
    }
    
    public async void CreateInvoice()
    {
        object oMissing = System.Reflection.Missing.Value;

        Assembly assembly = typeof(App).GetTypeInfo().Assembly;
        WordDocument document = new WordDocument();
        
        WSection section = document.AddSection() as WSection;
        section.PageSetup.Margins.Top = (float)42.5;
        section.PageSetup.Margins.Right = (float)42.5;
        section.PageSetup.Margins.Bottom = (float)42.5;
        section.PageSetup.Margins.Left = 51;
        
        WParagraphStyle style = document.AddParagraphStyle("C6-5") as WParagraphStyle;
        style.CharacterFormat.FontSize = (float)6.5;
        style.CharacterFormat.FontName = "Arial";
        style.ParagraphFormat.BeforeSpacing = 0;
        style.ParagraphFormat.AfterSpacing = 0;
        
        style = document.AddParagraphStyle("C7") as WParagraphStyle;
        style.CharacterFormat.FontSize = 7;
        style.CharacterFormat.FontName = "Arial";
        style.ParagraphFormat.BeforeSpacing = 0;
        style.ParagraphFormat.AfterSpacing = 0;
        
        style = document.AddParagraphStyle("C9") as WParagraphStyle;
        style.CharacterFormat.FontSize = 9;
        style.CharacterFormat.FontName = "Arial";
        style.ParagraphFormat.BeforeSpacing = 0;
        style.ParagraphFormat.AfterSpacing = 0;
        
        style = document.AddParagraphStyle("C11") as WParagraphStyle;
        style.CharacterFormat.FontSize = 11;
        style.CharacterFormat.FontName = "Arial";
        style.ParagraphFormat.BeforeSpacing = 0;
        style.ParagraphFormat.AfterSpacing = 0;
        
        style = document.AddParagraphStyle("C11b") as WParagraphStyle;
        style.CharacterFormat.FontSize = 11;
        style.CharacterFormat.Bold = true;
        style.CharacterFormat.FontName = "Arial";
        style.ParagraphFormat.BeforeSpacing = 0;
        style.ParagraphFormat.AfterSpacing = 0;
        
        style = document.AddParagraphStyle("C12") as WParagraphStyle;
        style.CharacterFormat.FontSize = 12;
        style.CharacterFormat.FontName = "Arial";
        style.ParagraphFormat.BeforeSpacing = 0;
        style.ParagraphFormat.AfterSpacing = 0;
        
        style = document.AddParagraphStyle("C16b") as WParagraphStyle;
        style.CharacterFormat.FontSize = 16;
        style.CharacterFormat.Bold = true;
        style.CharacterFormat.FontName = "Arial";
        style.ParagraphFormat.BeforeSpacing = 0;
        style.ParagraphFormat.AfterSpacing = 0;
        
        style = document.AddParagraphStyle("C20") as WParagraphStyle;
        style.CharacterFormat.FontSize = 20;
        style.CharacterFormat.FontName = "Arial";
        style.ParagraphFormat.BeforeSpacing = 0;
        style.ParagraphFormat.AfterSpacing = 0;
        
        style = document.AddParagraphStyle("C28") as WParagraphStyle;
        style.CharacterFormat.FontSize = 28;
        style.CharacterFormat.FontName = "Arial";
        style.ParagraphFormat.BeforeSpacing = 0;
        style.ParagraphFormat.AfterSpacing = 0;
        
        
        
        

        IWTextRange textRange;
        
        IWTable table = section.AddTable();
        table.ResetCells(2,2);

        textRange = table[0, 1].AddParagraph().AppendText("");
        textRange.OwnerParagraph.ApplyStyle("C7");
        textRange = table[0, 1].AddParagraph().AppendText($"{Settings.CompanyName}");
        textRange.OwnerParagraph.ApplyStyle("C28");
        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
        textRange = table[1, 0].AddParagraph().AppendText("\n\n\n\n\n");
        textRange.OwnerParagraph.ApplyStyle("C7");
        textRange = table[1, 0].AddParagraph()
            .AppendText($"\n{(String.IsNullOrWhiteSpace(Settings.CompanyName) ? Settings.Name : Settings.CompanyName)} | {Settings.AddressLine1} | {Settings.AddressLine2}");
        textRange.OwnerParagraph.ApplyStyle("C7");
        textRange = table[1, 0].AddParagraph().AppendText("\n");
        textRange.OwnerParagraph.ApplyStyle("C7");
        textRange = table[1, 0].AddParagraph()
            .AppendText($"{Employer.Name}\n{Employer.AddressLine1}\n{Employer.AddressLine2}");
        textRange.OwnerParagraph.ApplyStyle("C11");
        IWTable innerTable = table[1, 1].AddTable();
        innerTable.ResetCells(7,3);
        textRange = innerTable[0, 0].AddParagraph().AppendText("\n\n\n");
        textRange.OwnerParagraph.ApplyStyle("C12");
        textRange = innerTable[1, 1].AddParagraph().AppendText("Rechnungs-Nr.");
        textRange.OwnerParagraph.ApplyStyle("C12");
        textRange = innerTable[1, 2].AddParagraph().AppendText($"{InvoiceNumber}");
        textRange.OwnerParagraph.ApplyStyle("C12");
        textRange = innerTable[2, 1].AddParagraph().AppendText("Rechnungsdatum");
        textRange.OwnerParagraph.ApplyStyle("C9");
        textRange = innerTable[2, 2].AddParagraph().AppendText($"{DateTime.Now:d}");
        textRange.OwnerParagraph.ApplyStyle("C9");
        textRange = innerTable[3, 1].AddParagraph().AppendText("Leistungszeitraum");
        textRange.OwnerParagraph.ApplyStyle("C9");
        textRange = innerTable[3, 2].AddParagraph().AppendText($"{Start:d}-{End:d}");
        textRange.OwnerParagraph.ApplyStyle("C9");
        textRange = innerTable[4, 1].AddParagraph().AppendText("");
        textRange.OwnerParagraph.ApplyStyle("C9");
        textRange = innerTable[5, 1].AddParagraph().AppendText("Ihre Kunden-Nr.");
        textRange.OwnerParagraph.ApplyStyle("C9");
        textRange = innerTable[5, 2].AddParagraph().AppendText($"{Employer.EmployerNb}");
        textRange.OwnerParagraph.ApplyStyle("C9");
        if (!String.IsNullOrWhiteSpace(Settings.Name) && IncludeContactName)
        {
            textRange = innerTable[6, 1].AddParagraph().AppendText("Ihr Ansprechpartner");
            textRange.OwnerParagraph.ApplyStyle("C9");
            textRange = innerTable[6, 2].AddParagraph().AppendText($"{Settings.Name}");
            textRange.OwnerParagraph.ApplyStyle("C9");
        }
        else
        {
            textRange = innerTable[6, 1].AddParagraph().AppendText("");
            textRange.OwnerParagraph.ApplyStyle("C9");
        }
        innerTable.AutoFit(AutoFitType.FitToContent);
        innerTable.TableFormat.Borders.BorderType = BorderStyle.Cleared;

        table.TableFormat.Borders.BorderType = BorderStyle.Cleared;

        textRange = section.AddParagraph().AppendText("");
        textRange.OwnerParagraph.ApplyStyle("C20");

        table = section.AddTable();
        table.ResetCells(4,1);

        textRange = table[0, 0].AddParagraph().AppendText($"Rechnung Nr. {InvoiceNumber}");
        textRange.OwnerParagraph.ApplyStyle("C16b");

        if (!String.IsNullOrWhiteSpace(Settings.IntroductionText))
        {
            textRange = table[1, 0].AddParagraph().AppendText($"\n{Settings.IntroductionText}\n\n");
            textRange.OwnerParagraph.ApplyStyle("C11");
        }
        else
        {
            textRange = table[1, 0].AddParagraph().AppendText("\n");
            textRange.OwnerParagraph.ApplyStyle("C11");
        }
        
        

        innerTable = table[2, 0].AddTable();
        
        innerTable.ResetCells((OneItem ? 1 : Times.Count()) + 1,5);
        textRange = innerTable[0, 0].AddParagraph().AppendText("Pos.");
        textRange.OwnerParagraph.ApplyStyle("C11b");
        textRange = innerTable[0, 1].AddParagraph().AppendText("Bezeichnung");
        textRange.OwnerParagraph.ApplyStyle("C11b");
        textRange = innerTable[0, 2].AddParagraph().AppendText("Dauer");
        textRange.OwnerParagraph.ApplyStyle("C11b");
        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
        textRange = innerTable[0, 3].AddParagraph().AppendText("Stundenpreis");
        textRange.OwnerParagraph.ApplyStyle("C11b");
        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
        textRange = innerTable[0, 4].AddParagraph().AppendText("Gesamt");
        textRange.OwnerParagraph.ApplyStyle("C11b");
        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;

        foreach (WTableCell cell in innerTable.Rows[0].Cells)
        {
            cell.CellFormat.Borders.Bottom.BorderType = BorderStyle.Thick;
        }
        
        TimeSpan duration = TimeSpan.Zero;
        double total = 0;

        foreach (var time in Times.WithIndex())
        {
            duration += time.item.Duration;
            total += time.item.Earned;
            if (OneItem) continue;
            textRange = innerTable[time.index+1, 0].AddParagraph().AppendText($"{time.index+1}");
            textRange.OwnerParagraph.ApplyStyle("C11");
            textRange = innerTable[time.index+1, 1].AddParagraph().AppendText($"{Utility.StringReplacerItem(ItemDescription, time.item)}");
            textRange.OwnerParagraph.ApplyStyle("C11");
            textRange = innerTable[time.index+1, 2].AddParagraph().AppendText($"{time.item.Duration.ToHourString()}");
            textRange.OwnerParagraph.ApplyStyle("C11");
            textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
            textRange = innerTable[time.index+1, 3].AddParagraph().AppendText($"{time.item.Salary:C}");
            textRange.OwnerParagraph.ApplyStyle("C11");
            textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
            textRange = innerTable[time.index+1, 4].AddParagraph().AppendText($"{time.item.Earned:C}");
            textRange.OwnerParagraph.ApplyStyle("C11");
            textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
        }

        if (Times.Count == 0) OneItem = true;

        if (OneItem)
        {
            total = Employer.Salary * duration.TotalHours;
            textRange = innerTable[1, 0].AddParagraph().AppendText($"1");
            textRange.OwnerParagraph.ApplyStyle("C11");
            textRange = innerTable[1, 1].AddParagraph().AppendText($"{ItemDescription}");
            textRange.OwnerParagraph.ApplyStyle("C11");
            textRange = innerTable[1, 2].AddParagraph().AppendText($"{duration.ToHourString()}");
            textRange.OwnerParagraph.ApplyStyle("C11");
            textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
            textRange = innerTable[1, 3].AddParagraph().AppendText($"{Employer.Salary}");
            textRange.OwnerParagraph.ApplyStyle("C11");
            textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
            textRange = innerTable[1, 4].AddParagraph().AppendText($"{total:C}");
            textRange.OwnerParagraph.ApplyStyle("C11");
            textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
        }

        
        innerTable.Rows[0].RowFormat.Paddings.Bottom = 8;
        innerTable.Rows[1].RowFormat.Paddings.Bottom = 0;
        innerTable.Rows[innerTable.Rows.Count - 1].RowFormat.Paddings.Bottom = 8;

        foreach (WTableCell cell in innerTable.Rows[innerTable.Rows.Count - 1].Cells)
        {
            cell.CellFormat.Borders.Bottom.BorderType = BorderStyle.Thick;
        }

        innerTable.TableFormat.Borders.BorderType = BorderStyle.Cleared;

        innerTable.Rows[innerTable.Rows.Count - 1].RowFormat.Borders.Bottom.BorderType = BorderStyle.Thick;
        innerTable.Rows[0].RowFormat.Borders.Bottom.BorderType = BorderStyle.Thick;
        if (Times.Count > 0 || OneItem) innerTable.Rows[1].RowFormat.Paddings.Top = 8;

        if (VatValue == 0) VatValue = 19;
        
        if (IncludeVat)
        {
            innerTable.AddRow();
            textRange = innerTable[innerTable.Rows.Count - 1, 3].AddParagraph().AppendText("Gesamtbetrag netto");
            textRange.OwnerParagraph.ApplyStyle("C11");
            textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
            textRange = innerTable[innerTable.Rows.Count - 1, 4].AddParagraph().AppendText($"{total:C}");
            textRange.OwnerParagraph.ApplyStyle("C11");
            innerTable.Rows[innerTable.Rows.Count - 1].RowFormat.Paddings.Top = 8;
            textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
            innerTable.ApplyHorizontalMerge(innerTable.Rows.Count - 1, 0, 3);

            innerTable.AddRow();
            innerTable.Rows[innerTable.Rows.Count - 1].RowFormat.Paddings.Top = 0;
            textRange = innerTable[innerTable.Rows.Count - 1, 3].AddParagraph().AppendText($"zzgl. Umsatzsteuer {(VatValue - (int)VatValue < Math.Pow(10, -5) ? VatValue.ToString("F0") : VatValue.ToString("F2"))}%");
            textRange.OwnerParagraph.ApplyStyle("C11");
            textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
            textRange = innerTable[innerTable.Rows.Count - 1, 4].AddParagraph().AppendText($"{(total*(VatValue/100)).ToString("C")}");
            textRange.OwnerParagraph.ApplyStyle("C11");
            textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
            innerTable.ApplyHorizontalMerge(innerTable.Rows.Count - 1, 0, 3);

            total *= 1 + (VatValue / 100);
        }
        
        innerTable.AddRow();
        textRange = innerTable[innerTable.Rows.Count - 1, 3].AddParagraph().AppendText($"Gesamtbetrag{(IncludeVat ? " brutto" : "")}");
        textRange.OwnerParagraph.ApplyStyle("C11b");
        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
        textRange = innerTable[innerTable.Rows.Count - 1, 4].AddParagraph().AppendText($"{total:C}");
        textRange.OwnerParagraph.ApplyStyle("C11b");
        innerTable.Rows[innerTable.Rows.Count - 1].RowFormat.Paddings.Top = 8;
        textRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
        innerTable.ApplyHorizontalMerge(innerTable.Rows.Count - 1, 0, 3);

        for (int i = innerTable.Rows.Count - (IncludeVat ? 3 : 1); i < innerTable.Rows.Count; i++)
        {
            foreach (WTableCell cell in innerTable.Rows[i].Cells)
            {
                cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
            }
        }

        var width = innerTable.Width;
        List<double> widths = new List<double>{width * .1, width * .4, width * .1, width * .20, width * .20}; 

        foreach (WTableRow row in innerTable.Rows)
        {
            foreach (WTableCell cell in row.Cells)
            {
                cell.Width = (float)widths[cell.GetCellIndex()]; 
            }
        }
        
        //foreach (WTableRow wTableRow in table.Rows)
            //{
            //    BorderStyle bottomBorder =
            //        wTableRow.GetRowIndex() == table.Rows.Count-1 ? BorderStyle.Thick : BorderStyle.Cleared;
            //    foreach (WTableCell wTableCell in wTableRow.Cells)
            //    {
            //        wTableCell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
            //        wTableCell.CellFormat.Borders.Bottom.BorderType = bottomBorder;
            //        if (wTableCell.GetCellIndex() == 0) wTableCell.Width = 50;
            //        if (wTableCell.GetCellIndex() == 1) wTableCell.Width = (float)(612 - 110 - 72*2) / 3 + 55 -15;
            //        if (wTableCell.GetCellIndex() == 2) wTableCell.Width = 60;
            //        if (wTableCell.GetCellIndex() == 3) wTableCell.Width = (float)(612 - 110 - 72*2) / 3;
            //        if (wTableCell.GetCellIndex() == 4) wTableCell.Width = 70 + 15;
            //    }
            //}
        
        
        string[] informationText =
            Utility.StringReplacerProfile(Constants.Settings.InformationText).Split("%".ToCharArray());

        var paragraph = table[3,0].AddParagraph();
        paragraph.ApplyStyle("C11");
        textRange = paragraph.AppendText("\n\n\n");
        foreach (var s in informationText.WithIndex())
        {
            textRange = paragraph.AppendText($"{s.item}") as WTextRange;
            textRange.CharacterFormat.Bold = s.index % 2 == 1;
        }

        table.TableFormat.Borders.BorderType = BorderStyle.Cleared;

        table = section.HeadersFooters.Footer.AddTable();
        table.ResetCells(1,4);
        textRange = table[0, 0].AddParagraph()
            .AppendText(
                $"{(!String.IsNullOrWhiteSpace(Settings.CompanyName) ? Settings.CompanyName : Settings.Name)}\n{Settings.AddressLine1}\n{Settings.AddressLine2}");
        textRange.OwnerParagraph.ApplyStyle("C6-5");
        textRange = table[0, 1].AddParagraph().AppendText(
            $"{(!String.IsNullOrWhiteSpace(Settings.PhoneNumber) ? "Tel.: " + Settings.PhoneNumber + "\n" : String.Empty)}{(!String.IsNullOrWhiteSpace(Settings.EmailAddress) ? "E-Mail: " + Settings.EmailAddress : String.Empty)}");
        textRange.OwnerParagraph.ApplyStyle("C6-5");
        textRange = table[0, 2].AddParagraph()
            .AppendText(
                $"Steuer-Nr.: {Settings.TaxId}\n{(!String.IsNullOrWhiteSpace(Settings.ManagingDirector) ? "Geschäftsführung: " + Settings.ManagingDirector : String.Empty)}");
        textRange.OwnerParagraph.ApplyStyle("C6-5");
        textRange = table[0, 3].AddParagraph()
            .AppendText($"{Settings.BankName}\nIBAN: {Settings.BankIban}\nBIC: {Settings.BankBic}");
        textRange.OwnerParagraph.ApplyStyle("C6-5");

        table.TableFormat.Borders.BorderType = BorderStyle.Cleared;
        
        
        


        //WSection section = document.AddSection() as WSection;
        //section.PageSetup.Margins.All = 72;
        //section.PageSetup.PageSize = new Syncfusion.Drawing.SizeF(612, 792);
        //
        //
        //WParagraphStyle style = document.AddParagraphStyle("Normal") as WParagraphStyle;
        //style.CharacterFormat.FontName = "Arial";
        //style.CharacterFormat.FontSize = 11f;
        //style.ParagraphFormat.BeforeSpacing = 0;
        //style.ParagraphFormat.AfterSpacing = 0;
//
        //IWTableStyle tableStyle = document.AddTableStyle("NoBorder");
        //tableStyle.ParagraphFormat.Borders.Color = Syncfusion.Drawing.Color.Transparent;
        //
        //
        //style = document.AddParagraphStyle("SenderData") as WParagraphStyle;
        //style.CharacterFormat.FontName = "Arial";
        //style.CharacterFormat.FontSize = 8f;
        //style.CharacterFormat.TextColor = Syncfusion.Drawing.Color.DarkOrange;
        //style.ParagraphFormat.BeforeSpacing = 0;
        //style.ParagraphFormat.AfterSpacing = 8;
        //
//
//
        //IWParagraph paragraph = section.AddParagraph();
        //paragraph.ApplyStyle("Normal");
        //WTextRange textRange = paragraph.AppendText("\n\n\n\n\n") as WTextRange;
        //paragraph = section.AddParagraph();
        //paragraph.ApplyStyle("SenderData");
        //textRange = paragraph.AppendText($"{Constants.Settings.Name} | {Constants.Settings.AddressLine1} | {Constants.Settings.AddressLine2}") as WTextRange;
        //paragraph = section.AddParagraph();
        //paragraph.ApplyStyle("Normal");
        //textRange = paragraph.AppendText($"{Employer.Name}\n{Employer.AddressLine1}\n{Employer.AddressLine2}") as WTextRange;
        //paragraph = section.AddParagraph();
        //paragraph.ApplyStyle("Normal");
        //textRange = paragraph.AppendText("\n\n") as WTextRange;
        //IWTable table = section.AddTable();
        //table.ResetCells(2,3);
        //IWTextRange iwTextRange = table[0, 0].AddParagraph().AppendText("Kundennummer");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 11f;
        //iwTextRange.CharacterFormat.Bold = true;
        //iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //iwTextRange = table[1, 0].AddParagraph().AppendText($"{Employer.EmployerNb}");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 11f;
        //iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //iwTextRange = table[0, 1].AddParagraph().AppendText("Leistungszeitraum");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 11f;
        //iwTextRange.CharacterFormat.Bold = true;
        //iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //iwTextRange = table[1, 1].AddParagraph().AppendText($"{Start:d} - {End:d}");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 11f;
        //iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //iwTextRange = table[0, 2].AddParagraph().AppendText($"Rechnungsdatum");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 11f;
        //iwTextRange.CharacterFormat.Bold = true;
        //iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //iwTextRange = table[1, 2].AddParagraph().AppendText($"{DateTime.Now:d}");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 11f;
        //iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //foreach (WTableRow wTableRow in table.Rows)
        //{
        //    foreach (WTableCell wTableCell in wTableRow.Cells)
        //    {
        //        wTableCell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
        //    }
        //}
        //paragraph = section.AddParagraph();
        //paragraph.ApplyStyle("Normal");
        //textRange = paragraph.AppendText($"\n\n") as WTextRange;
        //
        //paragraph = section.AddParagraph();
        //paragraph.ApplyStyle("Normal");
        //textRange = paragraph.AppendText($"Rechnungsnummer: {InvoiceNumber}") as WTextRange;
        //textRange.CharacterFormat.FontSize = 18f;
        //
        //paragraph = section.AddParagraph();
        //paragraph.ApplyStyle("Normal");
        //textRange = paragraph.AppendText($"\n\n") as WTextRange;
        //
        //table = section.AddTable();
        ////table.ResetCells(!OneItem ? Times.Count()+1 : 2,5);
        //table.ResetCells(Times.Count() == 0 ? 1 : (OneItem ? 2 : Times.Count() + 1), 5);
        //iwTextRange = table[0, 0].AddParagraph().AppendText("Pos.\n");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 11f;
        //iwTextRange.CharacterFormat.Bold = true;
        //iwTextRange = table[0, 1].AddParagraph().AppendText("Bezeichnung\n");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 11f;
        //iwTextRange.CharacterFormat.Bold = true;
        //iwTextRange = table[0, 2].AddParagraph().AppendText("Dauer\n");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 11f;
        //iwTextRange.CharacterFormat.Bold = true;
        //iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //iwTextRange = table[0, 3].AddParagraph().AppendText($"Stundenpreis\n");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 11f;
        //iwTextRange.CharacterFormat.Bold = true;
        //iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //iwTextRange = table[0, 4].AddParagraph().AppendText($"Gesamt\n");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 11f;
        //iwTextRange.CharacterFormat.Bold = true;
        //iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //double total = 0; 
        //TimeSpan time = TimeSpan.Zero;
        //int spacingBottom = 4;
//
//
        //TimeSpan duration = TimeSpan.Zero;
        //for (var i = 0; i < Times.Count(); i++)
        //{
        //    time += Times[i].Duration;
        //    if (OneItem) continue;
        //    iwTextRange = table[i+1, 0].AddParagraph().AppendText($"{i+1}");
        //    iwTextRange.CharacterFormat.FontName = "Arial";
        //    iwTextRange.CharacterFormat.FontSize = 11f;
        //    iwTextRange.OwnerParagraph.ParagraphFormat.AfterSpacing = i + 1 == Times.Count() ? spacingBottom : 0;
        //    iwTextRange = table[i+1, 1].AddParagraph().AppendText($"{Utility.StringReplacerItem(ItemDescription, Times[i])}");
        //    iwTextRange.CharacterFormat.FontName = "Arial";
        //    iwTextRange.CharacterFormat.FontSize = 11f;
        //    iwTextRange = table[i+1, 2].AddParagraph().AppendText($"{Extensions.ToHourString(Times[i].Duration)}");
        //    iwTextRange.CharacterFormat.FontName = "Arial";
        //    iwTextRange.CharacterFormat.FontSize = 11f;
        //    iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //    iwTextRange = table[i+1, 3].AddParagraph().AppendText($"{Times[i].Salary:C}");
        //    iwTextRange.CharacterFormat.FontName = "Arial";
        //    iwTextRange.CharacterFormat.FontSize = 11f;
        //    iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //    iwTextRange = table[i+1, 4].AddParagraph().AppendText($"{Times[i].Earned:C}");
        //    iwTextRange.CharacterFormat.FontName = "Arial";
        //    iwTextRange.CharacterFormat.FontSize = 11f;
        //    iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //    total += Times[i].Earned;
        //}
//
        //if (OneItem && Times.Count() > 0)
        //{
        //    iwTextRange = table[1, 0].AddParagraph().AppendText($"1");
        //    iwTextRange.CharacterFormat.FontName = "Arial";
        //    iwTextRange.CharacterFormat.FontSize = 11f;
        //    iwTextRange.OwnerParagraph.ParagraphFormat.AfterSpacing = spacingBottom;
        //    iwTextRange = table[1, 1].AddParagraph().AppendText($"{ItemDescription}");
        //    iwTextRange.CharacterFormat.FontName = "Arial";
        //    iwTextRange.CharacterFormat.FontSize = 11f;
        //    iwTextRange = table[1, 2].AddParagraph().AppendText($"{time.ToHourString()}");
        //    iwTextRange.CharacterFormat.FontName = "Arial";
        //    iwTextRange.CharacterFormat.FontSize = 11f;
        //    iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //    iwTextRange = table[1, 3].AddParagraph().AppendText($"{Employer.Salary:C}");
        //    iwTextRange.CharacterFormat.FontName = "Arial";
        //    iwTextRange.CharacterFormat.FontSize = 11f;
        //    iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //    iwTextRange = table[1, 4].AddParagraph().AppendText($"{(Employer.Salary*time.TotalHours):C}");
        //    iwTextRange.CharacterFormat.FontName = "Arial";
        //    iwTextRange.CharacterFormat.FontSize = 11f;
        //    iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //    total = Employer.Salary * time.TotalHours;
        //}
        //foreach (WTableRow wTableRow in table.Rows)
        //{
        //    BorderStyle bottomBorder =
        //        wTableRow.GetRowIndex() == table.Rows.Count-1 ? BorderStyle.Thick : BorderStyle.Cleared;
        //    foreach (WTableCell wTableCell in wTableRow.Cells)
        //    {
        //        wTableCell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
        //        wTableCell.CellFormat.Borders.Bottom.BorderType = bottomBorder;
        //        if (wTableCell.GetCellIndex() == 0) wTableCell.Width = 50;
        //        if (wTableCell.GetCellIndex() == 1) wTableCell.Width = (float)(612 - 110 - 72*2) / 3 + 55 -15;
        //        if (wTableCell.GetCellIndex() == 2) wTableCell.Width = 60;
        //        if (wTableCell.GetCellIndex() == 3) wTableCell.Width = (float)(612 - 110 - 72*2) / 3;
        //        if (wTableCell.GetCellIndex() == 4) wTableCell.Width = 70 + 15;
        //    }
        //}
//
        //if (VatValue == 0) VatValue = 19;
        //if (IncludeVat)
        //{
        //    table.AddRow();
        //    iwTextRange = table[table.Rows.Count - 1, 3].AddParagraph().AppendText("Gesamtbetrag netto");
        //    iwTextRange.CharacterFormat.FontName = "Arial";
        //    iwTextRange.CharacterFormat.FontSize = 11f;
        //    iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment =
        //        Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //    iwTextRange.OwnerParagraph.ParagraphFormat.BeforeSpacing = 8;
        //    iwTextRange = table[table.Rows.Count - 1, 4].AddParagraph()
        //        .AppendText($"{total:C}");
        //    iwTextRange.CharacterFormat.FontName = "Arial";
        //    iwTextRange.CharacterFormat.FontSize = 11f;
        //    iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment =
        //        Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //    iwTextRange.OwnerParagraph.ParagraphFormat.BeforeSpacing = 8;
        //    table.AddRow();
        //    iwTextRange = table[table.Rows.Count - 1, 3].AddParagraph().AppendText($"zzgl. Umsatzsteuer {(VatValue - (int)VatValue < Math.Pow(10, -5) ? VatValue.ToString("F0") : VatValue.ToString("F2"))}%");
        //    iwTextRange.CharacterFormat.FontName = "Arial";
        //    iwTextRange.CharacterFormat.FontSize = 11f;
        //    iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment =
        //        Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //    iwTextRange = table[table.Rows.Count - 1, 4].AddParagraph().AppendText($"{(total*(VatValue/100)):C}");
        //    iwTextRange.CharacterFormat.FontName = "Arial";
        //    iwTextRange.CharacterFormat.FontSize = 11f;
        //    iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment =
        //        Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //    total *= (1 + (VatValue / 100));
        //}
//
        //table.AddRow();
        //iwTextRange = table[table.Rows.Count - 1, 3].AddParagraph().AppendText($"Rechnungsbetrag");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 11f;
        //iwTextRange.CharacterFormat.Bold = true;
        //iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment =
        //    Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //iwTextRange.OwnerParagraph.ParagraphFormat.BeforeSpacing = 8;
        //iwTextRange = table[table.Rows.Count - 1, 4].AddParagraph().AppendText($"{total:C}");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 11f;
        //iwTextRange.CharacterFormat.Bold = true;
        //iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment =
        //    Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //iwTextRange.OwnerParagraph.ParagraphFormat.BeforeSpacing = 8;
        //for (int i = table.Rows.Count - 1 - (IncludeVat ? 2 : 0); i < table.Rows.Count; i++)
        //{
        //    WTableRow row = table.Rows[i];
        //    foreach (WTableCell cell in row.Cells)
        //    {
        //        cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
        //        //cell.CellFormat.Borders.Bottom.BorderType = BorderStyle.Thick;
        //    }
        //    table.ApplyHorizontalMerge(i,0,3);
        //}
        //
        //
//
        ////paragraph = section.AddParagraph();
        ////paragraph.ApplyStyle("Normal");
        ////paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        ////textRange = paragraph.AppendText($"Rechnungsbetrag:    {}") as WTextRange;
        ////textRange.CharacterFormat.Bold = true;
        ////textRange.OwnerParagraph.ParagraphFormat.BeforeSpacing = 8;
        //
        //
//
        //paragraph = section.AddParagraph();
        //paragraph.ApplyStyle("Normal");
        //textRange = paragraph.AppendText($"\n\n") as WTextRange;
        //
        //string[] informationText = Utility.StringReplacerProfile(Constants.Settings.InformationText).Split("%".ToCharArray());
        ////string[] InformationText = Profile.InformationText
        ////    .Replace("%timespan%", Profile.DefaultInvoiceDays.ToString()).Split("%".ToCharArray());
        //var counter = 0;
        //paragraph = section.AddParagraph();
        //paragraph.ApplyStyle("Normal");
        //foreach (string s in informationText)
        //{
        //    textRange = paragraph.AppendText($"{s}") as WTextRange;
        //    textRange.CharacterFormat.Bold = counter%2==1;
        //    counter++;
        //}
//
        //table = section.HeadersFooters.Footer.AddTable();
        //table.ResetCells(2,2);
        //iwTextRange = table[0, 0].AddParagraph().AppendText("Bankverbindung");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 9f;
        //iwTextRange.CharacterFormat.Bold = true;
        //iwTextRange.OwnerParagraph.ParagraphFormat.BeforeSpacing = 5;
        //iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Left;
        //iwTextRange = table[0, 1].AddParagraph().AppendText("Steuernummer");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 9f;
        //iwTextRange.CharacterFormat.Bold = true;
        //iwTextRange.OwnerParagraph.ParagraphFormat.BeforeSpacing = 5;
        //iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //iwTextRange = table[1, 0].AddParagraph()
        //    .AppendText($"{Constants.Settings.BankName}\nKontoinhaber: {Constants.Settings.Name}\nIBAN: {Constants.Settings.BankIban}\nBIC: {Constants.Settings.BankBic}");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 9f;
        //iwTextRange.CharacterFormat.Bold = false;
        //iwTextRange.OwnerParagraph.ParagraphFormat.BeforeSpacing = 0;
        //iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Left;
        //iwTextRange = table[1, 1].AddParagraph().AppendText($"{Constants.Settings.TaxId}");
        //iwTextRange.CharacterFormat.FontName = "Arial";
        //iwTextRange.CharacterFormat.FontSize = 9f;
        //iwTextRange.CharacterFormat.Bold = false;
        //iwTextRange.OwnerParagraph.ParagraphFormat.BeforeSpacing = 0;
        //iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        //foreach (WTableRow wTableRow in table.Rows)
        //{
        //    BorderStyle topBorder = wTableRow.GetRowIndex() == 0 ? BorderStyle.Hairline : BorderStyle.Cleared;
        //    foreach (WTableCell wTableCell in wTableRow.Cells)
        //    {
        //        wTableCell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
        //        wTableCell.CellFormat.Borders.Top.BorderType = topBorder;
        //    }
        //}
//
//
        ////IWParagraph address = section.HeadersFooters.Header.AddParagraph();
        ////address = section.AddParagraph();
        ////address.ApplyStyle("EmployerAddress");
        ////address.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
        ////WTextRange textRange = address.AppendText($"{Employer.Name}\n{Employer.Address}") as WTextRange;

        MemoryStream docxStream = new MemoryStream();
        document.Save(docxStream, FormatType.Docx);
        
        Constants.Settings.LastInvoice = InvoiceNumber;
        await Constants.Database.SaveProfileAsync(Constants.Settings);
        DocIORenderer render = new DocIORenderer();
        PdfDocument pdfInvoice = render.ConvertToPDF(document);
        render.Dispose();
        document.Dispose();
        document.Close();
        MemoryStream pdfStream = new MemoryStream();
        pdfInvoice.Save(pdfStream);
        pdfInvoice.Close();
        docxStream.Position = 0;
        pdfStream.Position = 0;
        PdfPath = await Utility.Save($"Invoices/{DateTime.Now:s}_{Employer.EmployerNb}_{InvoiceNumber}.pdf", pdfStream);
        pdfStream.Position = 0;
        PdfStream = pdfStream;
        WordPath = await Utility.Save($"Invoices/{DateTime.Now:s}_{Employer.EmployerNb}_{InvoiceNumber}.docx", docxStream);
        InvoiceCreated = true;
        Constants.Settings.LastInvoice = InvoiceNumber;
        await Constants.Database.SaveProfileAsync(Constants.Settings);
    }

    public async void SaveInvoice()
    {
        await Utility.Share(PdfPath);
        await Utility.Share(WordPath);
    }

    double Func(double k)
    {
        if (k == 1) return .5 * Math.Sqrt(2);
        else return .5 * Math.Sqrt(2 + 2 * Func(k - 1));
    }
}