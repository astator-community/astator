using Android.Content;
using astator.Core.UI.Base;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;

namespace astator.Views;

internal class CustomLabelButton : Button
{
    public static readonly BindableProperty TagBindableProperty = BindableProperty.Create(nameof(Tag), typeof(object), typeof(CustomLabelButton));
    public object Tag
    {
        get => GetValue(TagBindableProperty);
        set => SetValue(TagBindableProperty, value);
    }
}

internal class CustomLabelButtonRenderer : ButtonRenderer
{
    public CustomLabelButtonRenderer(Context context) : base(context)
    {
    }

    protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
    {
        base.OnElementChanged(e);
        if (this.Control is not null)
        {
            this.Control.SetMinWidth(0);
            this.Control.SetMinHeight(0);
            this.Control.SetMinimumWidth(0);
            this.Control.SetMinimumHeight(0);
            //this.Control.SetWidth(Util.Dp2Px(e.NewElement.Width));
        }
    }
}
