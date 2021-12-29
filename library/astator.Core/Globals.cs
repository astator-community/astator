using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media.Projection;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using astator.Core.Graphics;
using astator.Core.UI.Floaty;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application = Android.App.Application;

namespace astator.Core
{
    public class Globals
    {
        public static Activity MainActivity { get; set; }

        /// <summary>
        /// 给用户展示简短消息的视图
        /// </summary>
        /// <param name="text">内容</param>
        /// <param name="duration">持续时间, 默认ToastLength.Short</param>
        public static void Toast(string text, ToastLength duration = ToastLength.Short)
        {
            RunOnUiThread(() =>
            {
                Android.Widget.Toast.MakeText(Application.Context, text, duration).Show();
            });
        }

        /// <summary>
        /// 在ui线程执行action
        /// </summary>
        /// <param name="action"></param>
        public static void RunOnUiThread(Action action)
        {
            MainActivity?.RunOnUiThread(() =>
            {
                action.Invoke();
            });
        }

        /// <summary>
        /// 设置状态栏颜色
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="color"></param>
        public static void SetStatusBarColor(Activity activity, string color)
        {
            activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            activity.Window.SetStatusBarColor(Color.ParseColor(color));
        }

        /// <summary>
        /// 设置状态栏字体颜色为白色
        /// </summary>
        /// <param name="activity"></param>
        public static void SetLightStatusBar(Activity activity)
        {
            activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
        }

        /// <summary>
        /// 启动一个activity
        /// </summary>
        /// <param name="intent">意图</param>
        public static void StartActivity(Intent intent)
        {
            MainActivity.StartActivity(intent);
        }


        /// <summary>
        /// 权限类
        /// </summary>
        public class Permission
        {
            /// <summary>
            /// 请求标志
            /// </summary>
            public enum RequestFlags
            {
                MediaProjection = 1000,
                FloatyWindow = 1001,
                ExternalStorage = 1002
            }

            /// <summary>
            /// 申请截图权限
            /// </summary>
            public static void ReqScreenCap()
            {
                if (CheckScreenCap())
                {
                    return;
                }

                Task.Run(() =>
                {
                    var manager = (MediaProjectionManager)MainActivity.GetSystemService("media_projection");
                    if (manager is not null)
                    {
                        var intent = manager.CreateScreenCaptureIntent();
                        MainActivity.StartActivityForResult(intent, (int)RequestFlags.MediaProjection);
                    }
                });
            }

            /// <summary>
            /// 检查截图权限
            /// </summary>
            /// <returns></returns>
            public static bool CheckScreenCap()
            {
                return ScreenCapturer.Instance is not null;
            }

            /// <summary>
            /// 申请悬浮窗权限
            /// </summary>
            public static void ReqFloaty()
            {
                if (!Android.Provider.Settings.CanDrawOverlays(MainActivity))
                {
                    MainActivity.StartActivityForResult(new Intent(Android.Provider.Settings.ActionManageOverlayPermission, Android.Net.Uri.Parse("package:" + MainActivity.PackageName)), (int)RequestFlags.FloatyWindow);
                }
            }

            /// <summary>
            /// 检查悬浮窗权限
            /// </summary>
            /// <returns></returns>
            public static bool CheckFloaty()
            {
                return Android.Provider.Settings.CanDrawOverlays(MainActivity);
            }

            /// <summary>
            /// 启动悬浮窗服务
            /// </summary>
            public static void StartFloatyService()
            {
                if (FloatyService.Instance is null)
                {
                    MainActivity.StartService(new(MainActivity, typeof(FloatyService)));
                }
            }
        }


        public class Devices
        {
            public static Activity MainActivity { get => Globals.MainActivity; }
            public static float Dp => Dm.Density;
            public static int Dpi => (int)Dm.DensityDpi;


            public static DisplayMetrics Dm
            {
                get
                {
                    var dm = new DisplayMetrics();
                    MainActivity.WindowManager.DefaultDisplay.GetRealMetrics(dm);
                    return dm;
                }
            }

            ///<summary>与旋转方向无关的宽</summary>
            public static int Width
            {
                get
                {
                    var manager = MainActivity?.WindowManager;
                    if (manager.DefaultDisplay.Rotation == Android.Views.SurfaceOrientation.Rotation0 || manager.DefaultDisplay.Rotation == Android.Views.SurfaceOrientation.Rotation180)
                    {
                        return Dm.WidthPixels;
                    }
                    return Dm.HeightPixels;
                }
            }
            ///<summary>与旋转方向无关的高</summary>
            public static int Height
            {
                get
                {
                    var manager = MainActivity?.WindowManager;
                    if (manager.DefaultDisplay.Rotation == Android.Views.SurfaceOrientation.Rotation0)
                    {
                        return Dm.HeightPixels;
                    }
                    return Dm.WidthPixels;
                }
            }
            ///<summary>修订版本号</summary>
            public static string Id => Build.Id;
            ///<summary>主板</summary>
            public static string Board => Build.Board;
            ///<summary>系统定制商</summary>
            public static string Brand => Build.Brand;
            ///<summary>CPU指令集</summary>
            public static List<string> SupportedAbis => (List<string>)Build.SupportedAbis;
            ///<summary>设备参数</summary>
            public static string Device => Build.Device;
            ///<summary>显示屏参数</summary>
            public static string Display => Build.Display;
            ///<summary>唯一编号</summary>
            public static string Fingerprint => Build.Fingerprint;
            ///<summary>硬件序列号</summary>
            public static string Serial => Build.GetSerial();
            ///<summary>硬件制造商</summary>
            public static string Manufacturer => Build.Manufacturer;
            ///<summary>版本</summary>
            public static string Model => Build.Model;
            ///<summary>硬件名</summary>
            public static string Hardware => Build.Hardware;
            ///<summary>手机产品名</summary>
            public static string Product => Build.Product;
            ///<summary>描述Build的标签</summary>
            public static string Tags => Build.Tags;
            ///<summary>Builder类型</summary>
            public static string Type => Build.Type;
            ///<summary>当前开发代码</summary>
            public static string Codename => Build.VERSION.Codename;
            ///<summary>源码控制版本号</summary>
            public static string Incremental => Build.VERSION.Incremental;
            ///<summary>版本字符串</summary>
            public static string Release => Build.VERSION.Release;
            ///<summary>SDK版本号</summary>
            public static int SdkInt => (int)Build.VERSION.SdkInt;
            ///<summary>Host值</summary>
            public static string Host => Build.Host;
            ///<summary>User名</summary>
            public static string User => Build.User;
            ///<summary>编译时间</summary>
            public static long Time => Build.Time;

            ///<summary>设备首次启动时产生和存储的64位数，当设备被wipe后该数重置。</summary>
            public static string AndroidId => Android.Provider.Settings.Secure.GetString(MainActivity.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
        }

    }
}
