using Android.Content;
using Microsoft.Maui.Platform;

namespace astator.TipsView;

public partial class TipsViewImpl : Grid
{
    public static readonly BindableProperty RadiusBindableProperty = BindableProperty.Create(nameof(Radius), typeof(int), typeof(TipsViewImpl), 30);
    public int Radius
    {
        get => (int)GetValue(RadiusBindableProperty);
        set => SetValue(RadiusBindableProperty, value);
    }

    public static Context AppContext { get; set; }

    private static TipsViewImpl instance;
    public static TipsViewImpl Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TipsViewImpl();
            }
            return instance;
        }
    }

    public static void ChangeTipsText(string text)
    {
        Device.BeginInvokeOnMainThread(() => Instance.Tips.Text = text);
    }
    public static void Hide()
    {
        Device.BeginInvokeOnMainThread(() => Instance.IsVisible = false);
    }

    public static void Show()
    {
        Device.BeginInvokeOnMainThread(() => Instance.IsVisible = true);
    }


    public TipsViewImpl()
    {
        InitializeComponent();

        var nativeView = this.ToPlatform(Application.Current.MainPage.Handler.MauiContext);
        _ = new FloatyWindow(AppContext, nativeView, gravity: Android.Views.GravityFlags.Center);
    }


    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        var view = this.Handler.PlatformView as LayoutViewGroup;
        view.ClipToOutline = true;
        view.OutlineProvider = new RadiusOutlineProvider(this.Radius);
    }
}