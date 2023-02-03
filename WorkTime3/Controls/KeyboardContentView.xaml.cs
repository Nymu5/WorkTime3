using Maui.BindableProperty.Generator.Core;

namespace MyTime.Controls;

public partial class KeyboardContentView : ContentView
{
    public static readonly BindableProperty MoveForKeyboardProperty =
        BindableProperty.Create(nameof(MoveForKeyboard), typeof(bool), typeof(KeyboardContentView), default(bool));
    public bool MoveForKeyboard
    {
        get => (bool)GetValue(MoveForKeyboardProperty);
        set => SetValue(MoveForKeyboardProperty, value);
    }

    public KeyboardContentView()
    {
        InitializeComponent();
#if __IOS__
        RegisterForKeyboardNotifications();
#endif
    }

    partial void RegisterForKeyboardNotifications();
}