using System.IO.Compression;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using astator.Core.Script;
using astator.Core.UI.Base;
using astator.Modules;
using astator.Pages;
using Microsoft.Maui.Platform;

namespace astator;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
public class MainActivity : MauiAppCompatActivity, IActivity
{
    public static MainActivity Instance { get; private set; }

    public LifecycleObserver LifecycleObserver { get; set; }


    protected override void OnCreate(Bundle savedInstanceState)
    {
        Instance = this;
        Globals.AppContext = this;
        TipsView.TipsViewImpl.AppContext = this;

        this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
        this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
        this.Window.SetStatusBarColor(((Color)Microsoft.Maui.Controls.Application.Current.Resources["PrimaryColor"]).ToPlatform());
        this.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;

        this.LifecycleObserver = new LifecycleObserver(this);
        this.Lifecycle.AddObserver(this.LifecycleObserver);

        var permissionHelper = new PermissionHelper(this);
        PermissionHelperer.Instance = permissionHelper;

        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
    }

    protected override void OnResume()
    {
        base.OnResume();
    }

    private DateTime latestTime;

    public override void OnBackPressed()
    {
        var mainPage = Microsoft.Maui.Controls.Application.Current.MainPage as TabbedPage;

        if (mainPage.Navigation.ModalStack.Count > 0)
        {
            Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PopModalAsync();
        }
        else
        {
            if (mainPage.CurrentPage is HomePage homePage)
            {
                if (homePage.OnBackPressed()) return;
            }
            else if (mainPage.CurrentPage is DocPage docPage)
            {
                if (docPage.OnBackPressed()) return;
            }

            var time = DateTime.Now;
            if (time.Subtract(this.latestTime).TotalMilliseconds < 1000)
            {
                this.Finish();
                Java.Lang.JavaSystem.Exit(0);
            }
            else
            {
                this.latestTime = time;
                Globals.Toast("再按一次返回退出应用");
            }
        }
    }
}
