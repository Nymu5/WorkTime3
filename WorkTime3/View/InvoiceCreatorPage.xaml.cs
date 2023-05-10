using System.Reactive.Disposables;
using ReactiveUI;

namespace MyTime.View;

public partial class InvoiceCreatorPage
{
    public InvoiceCreatorPage()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, x => x.Employer.EmployerDetailString, x => x.EmployerDetailString.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.Settings.PersonalDetailString, x => x.PersonalDetailString.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.Settings.BankDetailString, x => x.BankDetailString.Text)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.Start, x => x.StartDate.Date)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.End, x => x.EndDate.Date)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.OneItem, x => x.OneElementCheckbox.IsChecked)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.InvoiceNumber, x => x.InvoiceNumber.Text)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.ItemDescription, x => x.ItemDescription.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.InvoiceCreated, x => x.PdfViewer.IsVisible)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.InvoiceCreated, x => x.PdfGrid.IsVisible, CreatedToCreate, CreateToCreated)
                .DisposeWith(disposable);
            //this.Bind(ViewModel, x => x.InvoiceCreated, x => x.SaveToolbarItem.IsVisible)
            //    .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.InvoiceCreated, x => x.ToolbarHelper.IsToggled)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.PdfStream, x => x.PdfViewer.DocumentSource)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.LastInvoice, x => x.LastInvoiceLabel.Text)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.IncludeVat, x => x.VatCheckbox.IsChecked)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.VatValue, x => x.VatEntry.Text)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.IncludeContactName, x => x.ContactNameCheckbox.IsChecked)
                .DisposeWith(disposable);
        });
    }

    private bool CreatedToCreate(bool created) => !created;
    private bool CreateToCreated(bool created) => !created;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshToolbarItems();
    }

    private void RefreshToolbarItems()
    {
        SetToolbarItemVisibility(CreateToolbarItemR, !ViewModel!.InvoiceCreated);
        SetToolbarItemVisibility(SaveToolbarItemR, ViewModel!.InvoiceCreated);
    }
    
    private void ToolbarHelper_OnToggled(object sender, ToggledEventArgs e)
    {
        RefreshToolbarItems();
    }

    private void SetToolbarItemVisibility(ToolbarItem toolbarItem, bool value)
    {
        if (value && !ToolbarItems.Contains(toolbarItem)) ToolbarItems.Add(toolbarItem);
        if (!value) ToolbarItems.Remove(toolbarItem);
    }
}