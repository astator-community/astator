using Android.Views;

namespace astator.Views;

internal class CustomImage : Image
{
    public static readonly BindableProperty IsCircleBindableProperty = BindableProperty.Create(nameof(IsCircle), typeof(bool), typeof(CustomLabelButton));
    public bool IsCircle
    {
        get => (bool)GetValue(IsCircleBindableProperty);
        set => SetValue(IsCircleBindableProperty, value);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        var view = this.Handler.NativeView as AndroidX.AppCompat.Widget.AppCompatImageView;


        if (this.IsCircle)
        {
            view.ClipToOutline = true;
            view.OutlineProvider = new CircleImageOutlineProvider();
        }
    }

    private class CircleImageOutlineProvider : ViewOutlineProvider
    {
        public override void GetOutline(Android.Views.View view, Android.Graphics.Outline outline)
        {
            outline.SetRoundRect(0, 0, view.Width, view.Height, (view.Width < view.Height ? view.Width : view.Height) / 2);
        }
    }
}
