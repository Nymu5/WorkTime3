using Maui.BindableProperty.Generator.Core;

namespace MyTime.Resources.Templates;

public partial class ButtonRightIcon : ContentView
{
    [AutoBindable(PropertyName = "Command")]
    private readonly Command _command;
}