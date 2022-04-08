using Android.Content;
using Google.Android.Material.Snackbar;
using Microsoft.Maui.Platform;

namespace astator.TipsView;

public partial class TipsViewImpl : Grid
{
#if DEBUG
    public const string AstatorPackageName = "com.debug.astator";
#elif RELEASE
    public const string AstatorPackageName = "com.astator.astator";
#endif

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
            if (instance == null) instance = new TipsViewImpl();
            return instance;
        }
    }

    public static void ChangeTipsText(string text)
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            if (Android.App.Application.Context.PackageName == AstatorPackageName) Instance.Tips.Text = text;
        });
    }
    public static void Hide()
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            if (Android.App.Application.Context.PackageName == AstatorPackageName) Instance.IsVisible = false;
        });
    }

    public static void Show()
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            if (Android.App.Application.Context.PackageName == AstatorPackageName) Instance.IsVisible = true;
        });
    }

    public TipsViewImpl()
    {
        InitializeComponent();
        if (Android.App.Application.Context.PackageName == AstatorPackageName)
        {
            var nativeView = this.ToPlatform(Application.Current.MainPage.Handler.MauiContext);
            _ = new FloatyWindow(AppContext, nativeView, gravity: Android.Views.GravityFlags.Center);
        }
    }


    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        var view = this.Handler.PlatformView as LayoutViewGroup;
        view.ClipToOutline = true;
        view.OutlineProvider = new RadiusOutlineProvider(this.Radius);
    }
}