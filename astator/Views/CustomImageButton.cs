using Android.Views;
using astator.Core.UI.Base;
using Util = astator.Core.UI.Base.Util;

namespace astator.Views;

internal class CustomImageButton : ImageButton
{
    public static readonly BindableProperty TagBindableProperty = BindableProperty.Create(nameof(Tag), typeof(object), typeof(CustomLabelButton));
    public object Tag
    {
        get => GetValue(TagBindableProperty);
        set => SetValue(TagBindableProperty, value);
    }

    public static readonly BindableProperty IsCircleBindableProperty = BindableProperty.Create(nameof(IsCircle), typeof(bool), typeof(CustomLabelButton));
    public bool IsCircle
    {
        get => (bool)GetValue(IsCircleBindableProperty);
        set => SetValue(IsCircleBindableProperty, value);
    }



    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        var view = this.Handler.NativeView as AndroidX.AppCompat.Widget.AppCompatImageButton;


        view.SetPadding(Util.DpParse(this.Padding.Left), Util.DpParse(this.Padding.Top), Util.DpParse(this.Padding.Right), Util.DpParse(this.Padding.Bottom));


        if (this.IsCircle)
        {
            view.OutlineProvider = new CircleButtonOutlineProvider();
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

    private class CircleButtonOutlineProvider : ViewOutlineProvider
    {
        public override void GetOutline(Android.Views.View view, Android.Graphics.Outline outline)
        {
            outline.SetRoundRect(0, 0, view.Width, view.Height, view.Width < view.Height ? view.Width : view.Height);
        }
    }
}


