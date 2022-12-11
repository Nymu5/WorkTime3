using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTime3.View;

public partial class AddTimePage : ContentPage
{
    public AddTimePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Title = "Add Time";
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Title = String.Empty;
    }
}