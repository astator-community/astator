using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Core.Content;
using AndroidX.Core.View;
using astator.Core;
using astator.Core.Graphics;
using astator.Core.Script;
using astator.Core.UI;
using astator.Core.UI.Floaty;
using Microsoft.Maui;
using System;
using System.Threading.Tasks;
using static AndroidX.CoordinatorLayout.Widget.CoordinatorLayout;
using static astator.Core.Globals.Permission;

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
            Globals.MainActivity = this;

            try
            {
                Globals.SetStatusBarColor(this, "#f0f3f6");
                Globals.SetLightStatusBar(this);

                if (this.PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, this.PackageName) != Permission.Granted && this.PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, this.PackageName) != Permission.Granted)
                {
                    var permissions = new string[]
                    {
                        Manifest.Permission.ReadExternalStorage,
                        Manifest.Permission.WriteExternalStorage
                    };
                    RequestPermissions(permissions, (int)RequestFlags.ExternalStorage);
                }

                if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                {
                    if (!Android.OS.Environment.IsExternalStorageManager)
                    {
                        StartActivityForResult(new Intent(Android.Provider.Settings.ActionManageAllFilesAccessPermission), 3);
                    }
                }

            }
            catch (System.Exception) { }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (resultCode == Result.Ok)
                {
                    if (requestCode == (int)RequestFlags.MediaProjection)
                    {
                        Intent intent = new(this, typeof(ScreenCapturer));
                        intent.PutExtra("data", data);
                        StartForegroundService(intent);
                    }
                    else if (requestCode == (int)RequestFlags.FloatyWindow)
                    {
                        StartService(new(this, typeof(FloatyService)));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public override void StartActivityForResult(Intent intent, int requestCode)
        {
            base.StartActivityForResult(intent, requestCode);
        }

    }
}