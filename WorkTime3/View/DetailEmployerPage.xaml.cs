using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTime.View;

public partial class DetailEmployerPage : ContentPage
{
    public DetailEmployerPage()
    {
        InitializeComponent();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        //Title = "Details";
        Title = String.Empty;
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Title = String.Empty;
    }
}