namespace MyTime.Controls;

public class BindableToolbarItem : ToolbarItem
{
    public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(BindableToolbarItem), true, BindingMode.TwoWay, propertyChanged: OnIsVisibleChanged);

    public bool IsVisible
    {
        get => (bool)GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    protected override void OnParentChanged()
    {
        base.OnParentChanged();
        RefreshVisibility();
    }

    private static void OnIsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var item = bindable as BindableToolbarItem;
        item.RefreshVisibility();
    }

    private void RefreshVisibility()
    {
        if (Parent == null) return;

        bool value = IsVisible;

        var toolbarItems = ((ContentPage)Parent).ToolbarItems;

        if (value && !toolbarItems.Contains(this))
        {
            Application.Current.Dispatcher.Dispatch(() => toolbarItems.Add(this));
        } 
        else if (!value && toolbarItems.Contains(this))
        {
            Application.Current.Dispatcher.Dispatch(() => toolbarItems.Remove(this));
        }
    }
}