using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTime.Controller;
using ReactiveUI.Maui;

namespace MyTime.View;

public partial class InvoiceCreatorPage : ReactiveContentPage<InvoiceCreatorController>
{
    public InvoiceCreatorPage()
    {
        InitializeComponent();
    }
}