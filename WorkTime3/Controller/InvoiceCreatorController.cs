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
using Extensions = MyTime.Core.Extensions;
using Settings = MyTime.Model.Settings;
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
        ToggleCheckedCommand = new Command(execute: ToggleChecked);

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
        
    }
    
    // Commands
    public ICommand CreateInvoiceCommand { get; }
    public ICommand ToggleCheckedCommand { get; }
    
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
    
    private long _invoiceNumber;
    public long InvoiceNumber
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
    
    // Functions
    public void ToggleChecked()
    {
        OneItem = !OneItem; 
    }
    
    public async void CreateInvoice()
    {
        object oMissing = System.Reflection.Missing.Value;

        Assembly assembly = typeof(App).GetTypeInfo().Assembly;
        WordDocument document = new WordDocument();
        WSection section = document.AddSection() as WSection;
        section.PageSetup.Margins.All = 72;
        section.PageSetup.PageSize = new Syncfusion.Drawing.SizeF(612, 792);
        
        
        WParagraphStyle style = document.AddParagraphStyle("Normal") as WParagraphStyle;
        style.CharacterFormat.FontName = "Arial";
        style.CharacterFormat.FontSize = 11f;
        style.ParagraphFormat.BeforeSpacing = 0;
        style.ParagraphFormat.AfterSpacing = 0;

        IWTableStyle tableStyle = document.AddTableStyle("NoBorder");
        tableStyle.ParagraphFormat.Borders.Color = Syncfusion.Drawing.Color.Transparent;
        
        
        style = document.AddParagraphStyle("SenderData") as WParagraphStyle;
        style.CharacterFormat.FontName = "Arial";
        style.CharacterFormat.FontSize = 8f;
        style.CharacterFormat.TextColor = Syncfusion.Drawing.Color.DarkOrange;
        style.ParagraphFormat.BeforeSpacing = 0;
        style.ParagraphFormat.AfterSpacing = 8;
        


        IWParagraph paragraph = section.AddParagraph();
        paragraph.ApplyStyle("Normal");
        WTextRange textRange = paragraph.AppendText("\n\n\n\n\n") as WTextRange;
        paragraph = section.AddParagraph();
        paragraph.ApplyStyle("SenderData");
        textRange = paragraph.AppendText($"{Constants.Settings.Name} | {Constants.Settings.AddressLine1} | {Constants.Settings.AddressLine2}") as WTextRange;
        paragraph = section.AddParagraph();
        paragraph.ApplyStyle("Normal");
        textRange = paragraph.AppendText($"{Employer.Name}\n{Employer.AddressLine1}\n{Employer.AddressLine2}") as WTextRange;
        paragraph = section.AddParagraph();
        paragraph.ApplyStyle("Normal");
        textRange = paragraph.AppendText("\n\n") as WTextRange;
        IWTable table = section.AddTable();
        table.ResetCells(2,3);
        IWTextRange iwTextRange = table[0, 0].AddParagraph().AppendText("Kundennummer");
        iwTextRange.CharacterFormat.FontName = "Arial";
        iwTextRange.CharacterFormat.FontSize = 11f;
        iwTextRange.CharacterFormat.Bold = true;
        iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        iwTextRange = table[1, 0].AddParagraph().AppendText($"{Employer.EmployerNb}");
        iwTextRange.CharacterFormat.FontName = "Arial";
        iwTextRange.CharacterFormat.FontSize = 11f;
        iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        iwTextRange = table[0, 1].AddParagraph().AppendText("Leistungszeitraum");
        iwTextRange.CharacterFormat.FontName = "Arial";
        iwTextRange.CharacterFormat.FontSize = 11f;
        iwTextRange.CharacterFormat.Bold = true;
        iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        iwTextRange = table[1, 1].AddParagraph().AppendText($"{Start:dd.MM.yyyy} - {End:dd.MM.yyyy}");
        iwTextRange.CharacterFormat.FontName = "Arial";
        iwTextRange.CharacterFormat.FontSize = 11f;
        iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        iwTextRange = table[0, 2].AddParagraph().AppendText($"Rechnungsdatum");
        iwTextRange.CharacterFormat.FontName = "Arial";
        iwTextRange.CharacterFormat.FontSize = 11f;
        iwTextRange.CharacterFormat.Bold = true;
        iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        iwTextRange = table[1, 2].AddParagraph().AppendText($"{DateTime.Now:dd.MM.yyyy}");
        iwTextRange.CharacterFormat.FontName = "Arial";
        iwTextRange.CharacterFormat.FontSize = 11f;
        iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        foreach (WTableRow wTableRow in table.Rows)
        {
            foreach (WTableCell wTableCell in wTableRow.Cells)
            {
                wTableCell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
            }
        }
        paragraph = section.AddParagraph();
        paragraph.ApplyStyle("Normal");
        textRange = paragraph.AppendText($"\n\n") as WTextRange;
        
        paragraph = section.AddParagraph();
        paragraph.ApplyStyle("Normal");
        textRange = paragraph.AppendText($"Rechnungsnummer: {InvoiceNumber}") as WTextRange;
        textRange.CharacterFormat.FontSize = 18f;
        
        paragraph = section.AddParagraph();
        paragraph.ApplyStyle("Normal");
        textRange = paragraph.AppendText($"\n\n") as WTextRange;
        
        table = section.AddTable();
        //table.ResetCells(!OneItem ? Times.Count()+1 : 2,5);
        table.ResetCells(Times.Count() == 0 ? 1 : (OneItem ? 2 : Times.Count() + 1), 5);
        iwTextRange = table[0, 0].AddParagraph().AppendText("Pos.\n");
        iwTextRange.CharacterFormat.FontName = "Arial";
        iwTextRange.CharacterFormat.FontSize = 11f;
        iwTextRange.CharacterFormat.Bold = true;
        iwTextRange = table[0, 1].AddParagraph().AppendText("Bezeichnung\n");
        iwTextRange.CharacterFormat.FontName = "Arial";
        iwTextRange.CharacterFormat.FontSize = 11f;
        iwTextRange.CharacterFormat.Bold = true;
        iwTextRange = table[0, 2].AddParagraph().AppendText("Dauer (h)\n");
        iwTextRange.CharacterFormat.FontName = "Arial";
        iwTextRange.CharacterFormat.FontSize = 11f;
        iwTextRange.CharacterFormat.Bold = true;
        iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        iwTextRange = table[0, 3].AddParagraph().AppendText($"Stundenpreis\n");
        iwTextRange.CharacterFormat.FontName = "Arial";
        iwTextRange.CharacterFormat.FontSize = 11f;
        iwTextRange.CharacterFormat.Bold = true;
        iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        iwTextRange = table[0, 4].AddParagraph().AppendText($"Gesamt\n");
        iwTextRange.CharacterFormat.FontName = "Arial";
        iwTextRange.CharacterFormat.FontSize = 11f;
        iwTextRange.CharacterFormat.Bold = true;
        iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        double total = 0; 
        TimeSpan time = TimeSpan.Zero;
        int spacingBottom = 4;


        TimeSpan duration = TimeSpan.Zero;
        for (var i = 0; i < Times.Count(); i++)
        {
            time += Times[i].Duration;
            if (OneItem) continue;
            iwTextRange = table[i+1, 0].AddParagraph().AppendText($"{i+1}");
            iwTextRange.CharacterFormat.FontName = "Arial";
            iwTextRange.CharacterFormat.FontSize = 11f;
            iwTextRange.OwnerParagraph.ParagraphFormat.AfterSpacing = i + 1 == Times.Count() ? spacingBottom : 0;
            iwTextRange = table[i+1, 1].AddParagraph().AppendText($"{Utility.StringReplacerItem(ItemDescription, Times[i])}");
            iwTextRange.CharacterFormat.FontName = "Arial";
            iwTextRange.CharacterFormat.FontSize = 11f;
            iwTextRange = table[i+1, 2].AddParagraph().AppendText($"{Extensions.ToHourString(Times[i].Duration)}");
            iwTextRange.CharacterFormat.FontName = "Arial";
            iwTextRange.CharacterFormat.FontSize = 11f;
            iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
            iwTextRange = table[i+1, 3].AddParagraph().AppendText($"{Times[i].Salary:C}");
            iwTextRange.CharacterFormat.FontName = "Arial";
            iwTextRange.CharacterFormat.FontSize = 11f;
            iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
            iwTextRange = table[i+1, 4].AddParagraph().AppendText($"{Times[i].Earned:C}");
            iwTextRange.CharacterFormat.FontName = "Arial";
            iwTextRange.CharacterFormat.FontSize = 11f;
            iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
            total += Times[i].Earned;
        }

        if (OneItem && Times.Count() > 0)
        {
            iwTextRange = table[1, 0].AddParagraph().AppendText($"1");
            iwTextRange.CharacterFormat.FontName = "Arial";
            iwTextRange.CharacterFormat.FontSize = 11f;
            iwTextRange.OwnerParagraph.ParagraphFormat.AfterSpacing = spacingBottom;
            iwTextRange = table[1, 1].AddParagraph().AppendText($"{ItemDescription}");
            iwTextRange.CharacterFormat.FontName = "Arial";
            iwTextRange.CharacterFormat.FontSize = 11f;
            iwTextRange = table[1, 2].AddParagraph().AppendText($"{time.ToHourString()}");
            iwTextRange.CharacterFormat.FontName = "Arial";
            iwTextRange.CharacterFormat.FontSize = 11f;
            iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
            iwTextRange = table[1, 3].AddParagraph().AppendText($"{Employer.Salary:C}");
            iwTextRange.CharacterFormat.FontName = "Arial";
            iwTextRange.CharacterFormat.FontSize = 11f;
            iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
            iwTextRange = table[1, 4].AddParagraph().AppendText($"{(Employer.Salary*time.TotalHours):C}");
            iwTextRange.CharacterFormat.FontName = "Arial";
            iwTextRange.CharacterFormat.FontSize = 11f;
            iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        }
        foreach (WTableRow wTableRow in table.Rows)
        {
            BorderStyle bottomBorder =
                wTableRow.GetRowIndex() == table.Rows.Count-1 ? BorderStyle.Thick : BorderStyle.Cleared;
            foreach (WTableCell wTableCell in wTableRow.Cells)
            {
                wTableCell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                wTableCell.CellFormat.Borders.Bottom.BorderType = bottomBorder;
                if (wTableCell.GetCellIndex() == 0) wTableCell.Width = 50;
                if (wTableCell.GetCellIndex() == 1) wTableCell.Width = (float)(612 - 110 - 72*2) / 3 + 55;
                if (wTableCell.GetCellIndex() == 2) wTableCell.Width = 60;
                if (wTableCell.GetCellIndex() == 3) wTableCell.Width = (float)(612 - 110 - 72*2) / 3;
                if (wTableCell.GetCellIndex() == 4) wTableCell.Width = 70;
            }
        }

        paragraph = section.AddParagraph();
        paragraph.ApplyStyle("Normal");
        paragraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        textRange = paragraph.AppendText($"Rechnungsbetrag:    {(OneItem ? ((Employer.Salary*time.TotalHours).ToString("C")) : total.ToString("C"))}") as WTextRange;
        textRange.CharacterFormat.Bold = true;
        textRange.OwnerParagraph.ParagraphFormat.BeforeSpacing = 8;

        paragraph = section.AddParagraph();
        paragraph.ApplyStyle("Normal");
        textRange = paragraph.AppendText($"\n\n") as WTextRange;
        
        string[] informationText = Utility.StringReplacerProfile(Constants.Settings.InformationText).Split("%".ToCharArray());
        //string[] InformationText = Profile.InformationText
        //    .Replace("%timespan%", Profile.DefaultInvoiceDays.ToString()).Split("%".ToCharArray());
        var counter = 0;
        paragraph = section.AddParagraph();
        paragraph.ApplyStyle("Normal");
        foreach (string s in informationText)
        {
            textRange = paragraph.AppendText($"{s}") as WTextRange;
            textRange.CharacterFormat.Bold = counter%2==1;
            counter++;
        }

        table = section.HeadersFooters.Footer.AddTable();
        table.ResetCells(2,2);
        iwTextRange = table[0, 0].AddParagraph().AppendText("Bankverbindung");
        iwTextRange.CharacterFormat.FontName = "Arial";
        iwTextRange.CharacterFormat.FontSize = 9f;
        iwTextRange.CharacterFormat.Bold = true;
        iwTextRange.OwnerParagraph.ParagraphFormat.BeforeSpacing = 5;
        iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Left;
        iwTextRange = table[0, 1].AddParagraph().AppendText("Steuernummer");
        iwTextRange.CharacterFormat.FontName = "Arial";
        iwTextRange.CharacterFormat.FontSize = 9f;
        iwTextRange.CharacterFormat.Bold = true;
        iwTextRange.OwnerParagraph.ParagraphFormat.BeforeSpacing = 5;
        iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        iwTextRange = table[1, 0].AddParagraph()
            .AppendText($"{Constants.Settings.BankName}\nKontoinhaber: {Constants.Settings.Name}\nIBAN: {Constants.Settings.BankIban}\nBIC: {Constants.Settings.BankBic}");
        iwTextRange.CharacterFormat.FontName = "Arial";
        iwTextRange.CharacterFormat.FontSize = 9f;
        iwTextRange.CharacterFormat.Bold = false;
        iwTextRange.OwnerParagraph.ParagraphFormat.BeforeSpacing = 0;
        iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Left;
        iwTextRange = table[1, 1].AddParagraph().AppendText($"{Constants.Settings.TaxId}");
        iwTextRange.CharacterFormat.FontName = "Arial";
        iwTextRange.CharacterFormat.FontSize = 9f;
        iwTextRange.CharacterFormat.Bold = false;
        iwTextRange.OwnerParagraph.ParagraphFormat.BeforeSpacing = 0;
        iwTextRange.OwnerParagraph.ParagraphFormat.HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment.Right;
        foreach (WTableRow wTableRow in table.Rows)
        {
            BorderStyle topBorder = wTableRow.GetRowIndex() == 0 ? BorderStyle.Hairline : BorderStyle.Cleared;
            foreach (WTableCell wTableCell in wTableRow.Cells)
            {
                wTableCell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                wTableCell.CellFormat.Borders.Top.BorderType = topBorder;
            }
        }


        //IWParagraph address = section.HeadersFooters.Header.AddParagraph();
        //address = section.AddParagraph();
        //address.ApplyStyle("EmployerAddress");
        //address.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
        //WTextRange textRange = address.AppendText($"{Employer.Name}\n{Employer.Address}") as WTextRange;

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
        await Utility.SaveAndShare("Invoice.pdf", pdfStream);
        await Utility.SaveAndShare("Invoice.docx", docxStream);
            
        
        await Shell.Current.GoToAsync("..");
    }
}