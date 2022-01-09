using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using astator.Core;
using astator.Core.Graphics;
using astator.Core.UI.Floaty;
using Microsoft.Maui.Platform;
using static astator.Core.Globals.Permission;

namespace astator
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : MauiAppCompatActivity
    {
        public static MainActivity Instance { get; private set; }

        public Func<Keycode, KeyEvent, bool> OnKeyDownCallback { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            Globals.AppContext = this;

            try
            {
                this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                this.Window.SetStatusBarColor(((Color)Microsoft.Maui.Controls.Application.Current.Resources["PrimaryColor"]).ToNative());

                this.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;

                if (this.PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, this.PackageName) != Permission.Granted && this.PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, this.PackageName) != Permission.Granted)
                {
                    var permissions = new string[]
                    {
                        Manifest.Permission.ReadExternalStorage,
                        Manifest.Permission.WriteExternalStorage
                    };
                    RequestPermissions(permissions, 1002);
                }

                if (OperatingSystem.IsAndroidVersionAtLeast(30))
                {
                    if (!Android.OS.Environment.IsExternalStorageManager)
                    {
                        StartActivityForResult(new Intent(Android.Provider.Settings.ActionManageAllFilesAccessPermission), 3);
                    }
                }
            }
            catch { }

            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                if (requestCode == (int)RequestFlags.MediaProjection)
                {
                    Intent intent = new(this, typeof(ScreenCapturer));
                    intent.PutExtra("data", data);
                    if (OperatingSystem.IsAndroidVersionAtLeast(26))
                    {
                        StartService(intent);
                    }
                    else
                    {
                        StartService(intent);
                    }
                }
                else if (requestCode == (int)RequestFlags.FloatyWindow)
                {
                    StartService(new(this, typeof(FloatyService)));
                }
            }
        }


        private DateTime latestTime;

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            var result = this.OnKeyDownCallback?.Invoke(keyCode, e) ?? false;

            if (!result && keyCode == Keycode.Back)
            {
                var time = DateTime.Now;
                if (time.Subtract(this.latestTime).TotalMilliseconds < 1000)
                {
                    Process.KillProcess(Process.MyPid());
                }
                else
                {
                    this.latestTime = time;
                    Globals.Toast("再按一次返回退出astator");
                }
                return true;
            }

            return result ? result : base.OnKeyDown(keyCode, e);
        }
    }
}