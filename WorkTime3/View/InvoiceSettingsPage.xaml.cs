using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTime.Controller;
using ReactiveUI;
using ReactiveUI.Maui;

namespace MyTime.View;

public partial class InvoiceSettingsPage : ReactiveContentPage<InvoiceSettingsController>
{
    public InvoiceSettingsPage()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {

        });
    }
}