using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using MyTime.Controller;
using ReactiveUI;
using ReactiveUI.Maui;

namespace MyTime.View;

public partial class InvoiceCreatorPage : ReactiveContentPage<InvoiceCreatorController>
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
        });
    }
}