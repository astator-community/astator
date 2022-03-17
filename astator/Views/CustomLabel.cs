using astator.Core.UI.Base;

namespace astator.Views;

internal class CustomLabel : Label
{
    public static readonly BindableProperty TagBindableProperty = BindableProperty.Create(nameof(Tag), typeof(object), typeof(CustomLabel));
    public object Tag
    {
        get => GetValue(TagBindableProperty);
        set => SetValue(TagBindableProperty, value);
    }

    public event EventHandler Clicked;

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        var view = this.Handler.PlatformView as AndroidX.AppCompat.Widget.AppCompatTextView;

        view.SetOnClickListener(new OnClickListener((v) =>
        {
            Clicked?.Invoke(this, null);
        }));
    }
}
