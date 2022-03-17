using astator.Core.UI.Base;

namespace astator.Views;

internal class CustomImage : Image
{
    public static readonly BindableProperty IsCircleBindableProperty = BindableProperty.Create(nameof(IsCircle), typeof(bool), typeof(CustomImage));
    public bool IsCircle
    {
        get => (bool)GetValue(IsCircleBindableProperty);
        set => SetValue(IsCircleBindableProperty, value);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        var view = this.Handler.PlatformView as AndroidX.AppCompat.Widget.AppCompatImageView;
        if (this.IsCircle)
        {
            view.ClipToOutline = true;
            view.OutlineProvider = new CircleOutlineProvider();
        }
    }
}
