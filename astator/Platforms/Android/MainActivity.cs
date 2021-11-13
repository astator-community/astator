using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using astator.Core.Script;
using astator.Core.UI;
using Microsoft.Maui;

namespace astator
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : MauiAppCompatActivity
    {
        public static Activity Instance { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Instance = this;
            Devices.Activity = this;
            try
            {

                this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                this.Window.SetStatusBarColor(Color.ParseColor("#f0f3f6"));


                if (this.PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, this.PackageName) != Permission.Granted && this.PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, this.PackageName) != Permission.Granted)
                {
                    var permissions = new string[]
                    {
                        Manifest.Permission.ReadExternalStorage,
                        Manifest.Permission.WriteExternalStorage
                    };
                    RequestPermissions(permissions, (int)RequestFlags.ExternalStorage);
                }

                if (!Environment.IsExternalStorageManager)
                {
                    StartActivityForResult(new Intent(Android.Provider.Settings.ActionManageAllFilesAccessPermission), 3);
                }
            }
            catch (System.Exception) { }
        }
    }
}