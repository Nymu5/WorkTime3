using Maui.BindableProperty.Generator.Core;

namespace WorkTime3.Resources.Templates;

public partial class BorderedEntry : ContentView
{
    private static readonly BindableProperty EntryLabelProperty = BindableProperty.Create(nameof(EntryLabel), typeof(string), typeof(BorderedEntry), string.Empty);
    
    public string EntryLabel
    {
        get => (string)GetValue(EntryLabelProperty);
        set => SetValue(EntryLabelProperty, value);
    }
    
    [AutoBindable(DefaultValue = "Colors.LightGray")]
    private readonly Color _borderColor;

    [AutoBindable(PropertyName = "EntryText", DefaultBindingMode = nameof(BindingMode.TwoWay))]
    private readonly string _entryText; 
}