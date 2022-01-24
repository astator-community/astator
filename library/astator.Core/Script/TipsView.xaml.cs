using astator.Core.UI.Floaty;
using astator.Core.UI.Layout;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;

namespace astator.Core.Script;

public partial class TipsView : GridLayout
{
    public static readonly BindableProperty RadiusBindableProperty = BindableProperty.Create(nameof(Radius), typeof(int), typeof(TipsView), 30);
    public int Radius
    {
        get => (int)GetValue(RadiusBindableProperty);
        set => SetValue(RadiusBindableProperty, value);
    }

    private readonly Android.Views.View nativeView;
    private readonly AppFloatyWindow floaty;
    private readonly string loggerCallbackKey;

    public TipsView()
    {
        InitializeComponent();

        this.nativeView = this.ToNative(Application.Current.MainPage.Handler.MauiContext);
        this.floaty = new AppFloatyWindow(this.nativeView, gravity: Android.Views.GravityFlags.Center);

        this.loggerCallbackKey = ScriptLogger.AddCallback("TipsView", (args) =>
        {
            ChangeTipsText(args.Message);
        });
    }

    public void Close()
    {
        ScriptLogger.RemoveCallback(this.loggerCallbackKey);
        Globals.RunOnUiThread(() => this.floaty.Remove());
    }


    public void ChangeTipsText(string text)
    {
        if (text.EndsWith("..."))
        {
            Globals.RunOnUiThread(() => this.Tips.Text = text);
        }
    }




    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        var view = this.Handler.NativeView as LayoutViewGroup;

        view.ClipToOutline = true;
        view.OutlineProvider = new RadiusOutlineProvider(this.Radius);
    }
}