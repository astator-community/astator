using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace astator.Core.Script;
public static class Devices
{
    public static float Dp => Application.Context.Resources.DisplayMetrics.Density;
    public static int Dpi => (int)Application.Context.Resources.DisplayMetrics.DensityDpi;
    ///<summary>宽</summary>
    public static int Width
    {
        get
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                return Application.Context.GetSystemService("window").JavaCast<IWindowManager>().MaximumWindowMetrics.Bounds.Width();
            }
            else
            {
                var dm = new DisplayMetrics();
                Application.Context.GetSystemService("window").JavaCast<IWindowManager>().DefaultDisplay.GetRealMetrics(dm);
                return dm.WidthPixels;
            }
        }
    }
    ///<summary>高</summary>
    public static int Height
    {
        get
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                return Application.Context.GetSystemService("window").JavaCast<IWindowManager>().MaximumWindowMetrics.Bounds.Height();
            }
            else
            {
                var dm = new DisplayMetrics();
                Application.Context.GetSystemService("window").JavaCast<IWindowManager>().DefaultDisplay.GetRealMetrics(dm);
                return dm.HeightPixels;
            }
        }
    }
    ///<summary>与旋转方向无关的宽</summary>
    public static int RealWidth
    {
        get
        {
            var manager = Application.Context.GetSystemService("window").JavaCast<IWindowManager>();
            if (manager.DefaultDisplay.Rotation == SurfaceOrientation.Rotation0 || manager.DefaultDisplay.Rotation == SurfaceOrientation.Rotation180)
            {
                return Width;
            }
            return Height;
        }
    }
    ///<summary>与旋转方向无关的高</summary>
    public static int RealHeight
    {
        get
        {
            var manager = Application.Context.GetSystemService("window").JavaCast<IWindowManager>();
            if (manager.DefaultDisplay.Rotation == SurfaceOrientation.Rotation0 || manager.DefaultDisplay.Rotation == SurfaceOrientation.Rotation180)
            {
                return Height;
            }
            return Width;
        }
    }
    ///<summary>修订版本号</summary>
    public static string Id => Build.Id;
    ///<summary>主板</summary>
    public static string Board => Build.Board;
    ///<summary>系统定制商</summary>
    public static string Brand => Build.Brand;
    ///<summary>CPU指令集</summary>
    public static IList<string> SupportedAbis => Build.SupportedAbis;
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
    ///<summary>一个64位的数字(以十六进制字符串表示)，对应用程序签署密钥、用户和设备的每个组合来说都是独一无二的</summary>
    public static string AndroidId => Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
}
