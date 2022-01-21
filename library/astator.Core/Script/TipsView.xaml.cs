using astator.Core.UI.Layout;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;

namespace astator.Core.Script;

public partial class TipsView : GridLayout
{
    public static readonly BindableProperty RadiusBindableProperty = BindableProperty.Create(nameof(Radius), typeof(int), typeof(TipsView), 0);
    public int Radius
    {
        get => (int)GetValue(RadiusBindableProperty);
        set => SetValue(RadiusBindableProperty, value);
    }

    public TipsView()
    {
        InitializeComponent();
    }

    public void ChangeTipsText(string text)
    {
        this.Tips.Text = text;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        var view = this.Handler.NativeView as LayoutViewGroup;

        view.ClipToOutline = true;
        view.OutlineProvider = new RadiusOutlineProvider(this.Radius);
    }
}