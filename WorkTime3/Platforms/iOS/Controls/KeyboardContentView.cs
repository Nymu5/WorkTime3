using System.Diagnostics;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MyTime.Controls;

public partial class KeyboardContentView : ContentView
{
    NSObject _keyboardShowObserver;
    NSObject _keyboardHeightChangeObserver;
    NSObject _keyboardHideObserver;

    ~KeyboardContentView()
    {
        UnregisterForKeyboardNotifications();
    }

    private double? originalTranslationY;
    private bool origTranslationSaved = false;

    private bool IsUpCompleted = false;
    private bool IsDownCompleted = false;

    private void StoreTranslation()
    {
        if (!origTranslationSaved )
        {
            origTranslationSaved = true;
            originalTranslationY = this.Content.TranslationY;
        }
    }
    
    private async Task SetHeight(UIKeyboardEventArgs args)
    {
        StoreTranslation();

        nfloat bottom;
        try
        {
            UIWindow window = UIApplication.SharedApplication.Delegate.GetWindow();
            bottom = window.SafeAreaInsets.Bottom;
        }
        catch
        {
            bottom = 0;
        }


        NSValue result = (NSValue)args.Notification.UserInfo.ObjectForKey(new NSString(UIKeyboard.FrameEndUserInfoKey));
        CGSize keyboardSize = result.RectangleFValue.Size;

        Easing anim = Easing.SpringIn;

        var heightChange = (keyboardSize.Height - bottom);

        var duration = (uint)(args.AnimationDuration * 1000);
        
        if (MoveForKeyboard) await this.Content.TranslateTo(0, originalTranslationY.Value - heightChange, duration, anim);
    }

    

    async void OnKeyboardHeightChanged(object sender, UIKeyboardEventArgs args)
    {
        if (IsUpCompleted)
        {
            if (!IsDownCompleted)
            {
                try
                {
                    await SetHeight(args);
                }
                catch
                {
                    Debug.WriteLine("Could not resize page");
                }
            }
        }
    }

    async void OnKeyboardShow(object sender, UIKeyboardEventArgs args)
    {
        if (IsUpCompleted)
        {
            return;
        }
        try
        {
            await SetHeight(args);
            IsDownCompleted = false;
            IsUpCompleted = true;
        }
        catch
        {
            Debug.WriteLine("Could not resize page");
        }
    }

    async void OnKeyboardHide(object sender, UIKeyboardEventArgs args)
    {
        try
        {
            //SetOrigPadding();

            IsDownCompleted = true;
            IsUpCompleted = false;

            Easing anim = Easing.CubicIn;
            if (this != null && originalTranslationY != null)
            {
                var duration = (uint)(args.AnimationDuration * 1000);
                await this.Content.TranslateTo(0, originalTranslationY.Value, duration, anim);
            }
        }
        catch
        {
            Debug.WriteLine("Could not resize page");
        }            
    }


    partial void RegisterForKeyboardNotifications()
    {
        if (_keyboardShowObserver == null)
            _keyboardShowObserver = UIKeyboard.Notifications.ObserveWillShow(OnKeyboardShow); 

        if (_keyboardHeightChangeObserver == null)
            _keyboardHeightChangeObserver = UIKeyboard.Notifications.ObserveWillChangeFrame(OnKeyboardHeightChanged);

        if (_keyboardHideObserver == null)
            _keyboardHideObserver = UIKeyboard.Notifications.ObserveWillHide(OnKeyboardHide);
    }
    void UnregisterForKeyboardNotifications()
    {
        if (_keyboardShowObserver != null)
        {
            _keyboardShowObserver.Dispose();
            _keyboardShowObserver = null;
        }

        if (_keyboardHeightChangeObserver != null)
        {
            _keyboardHeightChangeObserver.Dispose();
            _keyboardHeightChangeObserver = null;
        }

        if (_keyboardHideObserver != null)
        {
            _keyboardHideObserver.Dispose();
            _keyboardHideObserver = null;
        }
    }
}