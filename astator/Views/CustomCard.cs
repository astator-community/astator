using Android.Views;
using astator.Core.UI.Base;
using Microsoft.Maui.Platform;

namespace astator.Views;

public class CustomCard : Grid
{
    public static readonly BindableProperty TagBindableProperty = BindableProperty.Create(nameof(Tag), typeof(object), typeof(CustomCard));
    public object Tag
    {
        get => GetValue(TagBindableProperty);
        set => SetValue(TagBindableProperty, value);
    }

    public event EventHandler Clicked;
    public event EventHandler LongClicked;

    protected override void OnHandlerChanged()
    {
        var view = this.Handler.PlatformView as LayoutViewGroup;

        view.SetOnTouchListener(new OnTouchListener((v, e) =>
        {
            if (e.Action == MotionEventActions.Down)
            {
                this.BackgroundColor = (Color)Application.Current.Resources["FocusColor"];
            }
            else
            {
                this.BackgroundColor = (Color)Application.Current.Resources["PrimaryColor"];
            }
            return false;
        }));


        view.SetOnClickListener(new OnClickListener((v) =>
        {
            this.Clicked?.Invoke(this, null);
        }));

        view.SetOnLongClickListener(new OnLongClickListener((v) =>
        {
            this.LongClicked?.Invoke(this, null);
            return true;
        }));
    }
}

