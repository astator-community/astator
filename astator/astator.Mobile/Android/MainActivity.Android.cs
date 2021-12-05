using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using astator.Core;
using astator.Core.Graphics;
using astator.Core.UI.Floaty;
using System;
using static astator.Core.Globals.Permission;


namespace astator
{
    [Activity(
            MainLauncher = true,
            ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
            WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
        )]
    public class MainActivity : Windows.UI.Xaml.ApplicationActivity
    {
        public static MainActivity Instance { get; private set; }

        public Func<Keycode, KeyEvent, bool> KeyDownCallback;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Instance = this;
            Globals.MainActivity = this;
            try
            {
                this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                this.Window.SetStatusBarColor(Color.ParseColor("#f0f3f6"));

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

                if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                {
                    if (!Android.OS.Environment.IsExternalStorageManager)
                    {
                        StartActivityForResult(new Intent(Android.Provider.Settings.ActionManageAllFilesAccessPermission), 3);
                    }
                }
            }
            catch { }
        }
        public override bool OnKeyDown(Keycode keycode, KeyEvent e)
        {
            var result = this.KeyDownCallback?.Invoke(keycode, e) ?? false;
            return result ? result : base.OnKeyDown(keycode, e);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
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
        public override void StartActivityForResult(Intent intent, int requestCode)
        {
            base.StartActivityForResult(intent, requestCode);
        }
    }
}

