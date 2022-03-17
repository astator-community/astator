using Android.Views;
using astator.Core.UI.Base;
using Util = astator.Core.UI.Base.Util;

namespace astator.Views;

internal class CustomImageButton : ImageButton
{
    public static readonly BindableProperty TagBindableProperty = BindableProperty.Create(nameof(Tag), typeof(object), typeof(CustomImageButton));
    public object Tag
    {
        get => GetValue(TagBindableProperty);
        set => SetValue(TagBindableProperty, value);
    }

    public static readonly BindableProperty IsCircleBindableProperty = BindableProperty.Create(nameof(IsCircle), typeof(bool), typeof(CustomImageButton));
    public bool IsCircle
    {
        get => (bool)GetValue(IsCircleBindableProperty);
        set => SetValue(IsCircleBindableProperty, value);
    }



    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        var view = this.Handler.PlatformView as AndroidX.AppCompat.Widget.AppCompatImageView;


        view.SetPadding(Util.Dp2Px(this.Padding.Left), Util.Dp2Px(this.Padding.Top), Util.Dp2Px(this.Padding.Right), Util.Dp2Px(this.Padding.Bottom));


        if (this.IsCircle)
        {
            view.OutlineProvider = new CircleOutlineProvider();
            view.ClipToOutline = true;
        }

        view.SetOnTouchListener(new OnTouchListener((v, e) =>
        {
            if (e.Action == MotionEventActions.Down)
            {
                this.BackgroundColor = (Color)Application.Current.Resources["FocusColor"];
            }
            else
            {
                this.BackgroundColor = Colors.Transparent;
            }
            return false;
        }));
    }
}


