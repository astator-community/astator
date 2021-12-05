
using Android.App;
using Android.OS;
using Android.Util;
using System.Collections.Generic;
using Debug = System.Diagnostics.Debug;

namespace astator.Core.Script
{
    public static class Devices
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
                Debug.Assert(manager is not null);
                Debug.Assert(manager.DefaultDisplay is not null);
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
                Debug.Assert(manager is not null);
                Debug.Assert(manager.DefaultDisplay is not null);
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
        public static string AndroidId => Android.Provider.Settings.Secure.GetString(MainActivity?.ApplicationContext?.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
    }
}
