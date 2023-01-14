using Maui.BindableProperty.Generator.Core;

namespace MyTime.Resources.Templates;

public partial class DetailLabel : ContentView
{
    [AutoBindable(PropertyName = "Label", DefaultBindingMode = nameof(BindingMode.TwoWay))]
    private readonly string _label;

    [AutoBindable(PropertyName = "Content", DefaultBindingMode = nameof(BindingMode.TwoWay))]
    private readonly string _content;
}