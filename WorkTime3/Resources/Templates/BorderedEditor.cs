using Maui.BindableProperty.Generator.Core;

namespace MyTime.Resources.Templates;

public partial class BorderedEditor : ContentView
{
    private static readonly BindableProperty EditorLabelProperty = BindableProperty.Create(nameof(EditorLabel), typeof(string), typeof(BorderedEditor), string.Empty);
    
    public string EditorLabel
    {
        get => (string)GetValue(EditorLabelProperty);
        set => SetValue(EditorLabelProperty, value);
    }
    
    [AutoBindable(DefaultValue = "Colors.LightGray")]
    private readonly Color _borderColor;
    
    [AutoBindable(PropertyName = "EntryText", DefaultBindingMode = nameof(BindingMode.TwoWay))]
    private readonly string _entryText; 
}