using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media.Projection;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Activity.Result;
using astator.Core.Accessibility;
using astator.Core.Graphics;
using astator.Core.UI;
using astator.Core.UI.Floaty;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Application = Android.App.Application;

namespace astator.Core.Script;

public class Globals
{
    public static Context AppContext { get; set; }

    /// <summary>
    /// 给用户展示简短消息的视图
    /// </summary>
    /// <param name="text">内容</param>
    /// <param name="duration">持续时间, 默认ToastLength.Short</param>
    public static void Toast(object text, ToastLength duration = ToastLength.Short)
    {
        RunOnUiThread(() => Android.Widget.Toast.MakeText(Application.Context, text.ToString(), duration).Show());
    }

    /// <summary>
    /// 在ui线程执行action
    /// </summary>
    /// <param name="action"></param>
    public static void RunOnUiThread(Action action)
    {
        Device.BeginInvokeOnMainThread(action);
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
    /// 权限类
    /// </summary>
    public class Permission
    {
        public static LifecycleObserver LifecycleObserver { get; set; }

        /// <summary>
        /// 启动一个activity
        /// </summary>
        /// <param name="intent">意图</param>
        public static void StartActivity(Intent intent)
        {
            AppContext.StartActivity(intent);
        }

        /// <summary>
        /// 启动一个activity并获取回传数据
        /// </summary>
        /// <param name="intent">意图</param>
        public static void StartActivityForResult(Intent intent, Action<ActivityResult> callback)
        {
            LifecycleObserver.StartActivityForResult(intent, callback);
        }

        /// <summary>
        /// 申请截图权限
        /// </summary>
        public static void ReqScreenCap(Action<bool> callback)
        {
            if (CheckScreenCap())
            {
                return;
            }

            var manager = (MediaProjectionManager)AppContext.GetSystemService("media_projection");
            var intent = manager.CreateScreenCaptureIntent();
            LifecycleObserver.StartActivityForResult(intent,
            result =>
            {
                var isOk = result.ResultCode == (int)Result.Ok;
                if (isOk)
                {
                    var intent = new Intent(AppContext, typeof(ScreenCapturer));
                    intent.PutExtra("data", result.Data);
                    if (OperatingSystem.IsAndroidVersionAtLeast(26))
                    {
                        AppContext.StartForegroundService(intent);
                    }
                    else
                    {
                        AppContext.StartService(intent);
                    }
                }
                callback.Invoke(isOk);
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
            if (!Android.Provider.Settings.CanDrawOverlays(AppContext))
            {
                var intent = new Intent(Android.Provider.Settings.ActionManageOverlayPermission, Android.Net.Uri.Parse("package:" + AppContext.PackageName));
                LifecycleObserver.StartActivityForResult(intent,
                    result =>
                    {
                        if (Android.Provider.Settings.CanDrawOverlays(AppContext))
                        {
                            var intent = new Intent(AppContext, typeof(FloatyService));
                            if (OperatingSystem.IsAndroidVersionAtLeast(26))
                            {
                                AppContext.StartForegroundService(intent);
                            }
                            else
                            {
                                AppContext.StartService(intent);
                            }
                        }
                    });
            }
        }

        /// <summary>
        /// 检查悬浮窗权限
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckFloaty()
        {
            if (Android.Provider.Settings.CanDrawOverlays(AppContext))
            {
                await StartFloatyService();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 启动悬浮窗服务
        /// </summary>
        public static async Task StartFloatyService()
        {
            if (FloatyService.Instance is null)
            {
                var intent = new Intent(AppContext, typeof(FloatyService));
                if (OperatingSystem.IsAndroidVersionAtLeast(26))
                {
                    AppContext.StartForegroundService(intent);
                }
                else
                {
                    AppContext.StartService(intent);
                }

                await Task.Run(() =>
                {
                    while (FloatyService.Instance is null)
                    {
                        System.Threading.Thread.Sleep(50);
                    }
                });
            }
        }

        /// <summary>
        /// 检查无障碍服务是否开启
        /// </summary>
        /// <returns></returns>
        public static bool CheckAccessibilityService()
        {
            return ScriptAccessibilityService.Instance is not null;
        }

        /// <summary>
        /// 跳转到系统无障碍服务界面
        /// </summary>
        public static void ReqAccessibilityService()
        {
            var intent = new Intent(Android.Provider.Settings.ActionAccessibilitySettings);
            intent.SetFlags(ActivityFlags.NewTask);
            AppContext.StartActivity(intent);
        }

        /// <summary>
        /// 权限动态申请
        /// </summary>
        /// <param name="permission">权限值</param>
        /// <param name="callback">申请结果回调</param>
        public static void ReqPermission(string permission, Action<bool> callback)
        {
            LifecycleObserver?.ReqPermission(permission, callback);
        }

        /// <summary>
        /// 获取是否已忽略电池优化
        /// </summary>
        /// <returns></returns>
        public static bool IsIgnoringBatteryOptimizations()
        {
            var powerManager = (PowerManager)AppContext.GetSystemService("power");
            return powerManager.IsIgnoringBatteryOptimizations(AppContext.PackageName);
        }

        /// <summary>
        /// 忽略电池优化
        /// </summary>
        public static void IgnoringBatteryOptimizations()
        {
            var powerManager = (PowerManager)AppContext.GetSystemService("power");
            var isIgnored = powerManager.IsIgnoringBatteryOptimizations(AppContext.PackageName);
            if (!isIgnored)
            {
                var intent = new Intent("android.settings.REQUEST_IGNORE_BATTERY_OPTIMIZATIONS");
                intent.SetData(Android.Net.Uri.Parse($"package:{AppContext.PackageName}"));
                AppContext.StartActivity(intent);
            }
            //else
            //{
            //    var intent = new Intent("android.settings.IGNORE_BATTERY_OPTIMIZATION_SETTINGS");
            //    var resolveInfo = AppContext.PackageManager.ResolveActivity(intent, 0);
            //    if (resolveInfo != null)
            //    {
            //        AppContext.StartActivity(intent);
            //    }
            //}
        }
    }


    public class Devices
    {
        public static float Dp => Dm.Density;
        public static int Dpi => (int)Dm.DensityDpi;


        public static DisplayMetrics Dm
        {
            get
            {
                var dm = new DisplayMetrics();
                (AppContext as Activity).WindowManager.DefaultDisplay.GetRealMetrics(dm);
                return dm;
            }
        }
        ///<summary>宽</summary>
        public static int Width => Dm.WidthPixels;
        ///<summary>高</summary>
        public static int Height => Dm.HeightPixels;
        ///<summary>与旋转方向无关的宽</summary>
        public static int RealWidth
        {
            get
            {
                var manager = (AppContext as Activity)?.WindowManager;
                if (manager.DefaultDisplay.Rotation == SurfaceOrientation.Rotation0 || manager.DefaultDisplay.Rotation == SurfaceOrientation.Rotation180)
                {
                    return Dm.WidthPixels;
                }
                return Dm.HeightPixels;
            }
        }
        ///<summary>与旋转方向无关的高</summary>
        public static int RealHeight
        {
            get
            {
                var manager = (AppContext as Activity)?.WindowManager;
                if (manager.DefaultDisplay.Rotation == SurfaceOrientation.Rotation0 || manager.DefaultDisplay.Rotation == SurfaceOrientation.Rotation180)
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

        [SupportedOSPlatform("android26.0")]
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
        public static string AndroidId => Android.Provider.Settings.Secure.GetString((AppContext as Activity).ContentResolver, Android.Provider.Settings.Secure.AndroidId);
    }

}
