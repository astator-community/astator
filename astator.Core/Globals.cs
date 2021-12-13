using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media.Projection;
using Android.Views;
using Android.Widget;
using astator.Core.UI.Floaty;
using System;
using System.Threading.Tasks;
using Application = Android.App.Application;

namespace astator.Core
{
    public static class Globals
    {
        public static Activity MainActivity { get; set; }

        public static void Toast(string text, ToastLength duration = 0)
        {
            RunOnUiThread(() =>
            {
                Android.Widget.Toast.MakeText(Application.Context, text, duration).Show();
            });
        }

        public static void RunOnUiThread(Action action)
        {
            MainActivity?.RunOnUiThread(() =>
            {
                action.Invoke();
            });
        }

        public static void SetStatusBarColor(Activity activity, string color)
        {
            activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            activity.Window.SetStatusBarColor(Color.ParseColor(color));
        }

        public static void SetLightStatusBar(Activity activity)
        {
            activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
        }

        public static void StartActivity(Intent intent)
        {
            MainActivity.StartActivity(intent);
        }


        public static class Permission
        {
            public enum RequestFlags
            {
                MediaProjection = 1000,
                FloatyWindow = 1001,
                ExternalStorage = 1002
            }

            public enum CaptureOrientation
            {
                None = 0,
                Vertical = 1,
                Horizontal = 2,
            }

            public static Task ReqScreenCapture(ScriptRuntime runtime, CaptureOrientation orientation)
            {
                return runtime.Tasks.Run((token) =>
                {
                    runtime.CaptureOrientation = orientation;
                    var manager = (MediaProjectionManager)MainActivity.GetSystemService("media_projection");
                    if (manager is not null)
                    {
                        var intent = manager.CreateScreenCaptureIntent();
                        intent.PutExtra("id", runtime.ScriptId);
                        intent.PutExtra("orientation", (int)orientation);
                        MainActivity.StartActivityForResult(intent, (int)RequestFlags.MediaProjection);
                    }
                });
            }

            public static void ReqFloaty()
            {
                if (!Android.Provider.Settings.CanDrawOverlays(MainActivity))
                {
                    MainActivity.StartActivityForResult(new Intent(Android.Provider.Settings.ActionManageOverlayPermission, Android.Net.Uri.Parse("package:" + MainActivity.PackageName)), (int)RequestFlags.FloatyWindow);
                }
            }

            public static bool CheckFloaty()
            {
                return Android.Provider.Settings.CanDrawOverlays(MainActivity);
            }

            public static void StartFloatyService()
            {
                if (FloatyService.Instance is null)
                {
                    MainActivity.StartService(new(MainActivity, typeof(FloatyService)));
                }
            }
        }

    }
}
