using Foundation;
using UIKit;

namespace MyTime;

public class KeyboardAvoidingScrollViewHandler : Microsoft.Maui.Handlers.ScrollViewHandler
{
    NSObject keyboardShowObserver;
    NSObject keyboardHideObserver;

    protected override void ConnectHandler(UIScrollView platformView)
    {
        base.ConnectHandler(platformView);

        keyboardShowObserver = UIKeyboard.Notifications.ObserveWillShow(OnKeyboardShow);
        keyboardHideObserver = UIKeyboard.Notifications.ObserveWillHide(OnKeyboardHide);
    }

    private void OnKeyboardShow(object sender, UIKeyboardEventArgs args)
    {
        var scrollView = PlatformView;
        if (scrollView == null)
        {
            return;
        }

        var keyboardFrame = UIKeyboard.FrameEndFromNotification(args.Notification);

        scrollView.ContentInset = new UIEdgeInsets(
            scrollView.ContentInset.Top,
            scrollView.ContentInset.Left,
            keyboardFrame.Size.Height,
            scrollView.ContentInset.Right
        );
    }

    private void OnKeyboardHide(object sender, UIKeyboardEventArgs e)
    {
        var scrollView = PlatformView;
        if (scrollView == null)
        {
            return;
        }

        scrollView.ContentInset = UIEdgeInsets.Zero;
    }

    protected override void DisconnectHandler(UIScrollView platformView)
    {
        if (keyboardShowObserver != null)
        {
            keyboardShowObserver.Dispose();
            keyboardShowObserver = null;
        }

        if (keyboardHideObserver != null)
        {
            keyboardHideObserver.Dispose();
            keyboardHideObserver = null;
        }

        base.DisconnectHandler(platformView);
    }
}