using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using astator.Core.UI.Base;
using System;
using static Android.Views.ViewGroup;

namespace astator.Core.UI.Floaty;

/// <summary>
/// 系统悬浮窗, 需要悬浮窗权限
/// </summary>
public class SystemFloatyWindow : FloatyWindowBase
{

    /// <summary>
    /// 构造函数, 创建一个悬浮窗
    /// </summary>
    /// <param name="view"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public SystemFloatyWindow(Context context, View view,
        int x = 0,
        int y = 0,
        GravityFlags gravity = GravityFlags.Left | GravityFlags.Top,
        WindowManagerFlags flags = WindowManagerFlags.NotFocusable | WindowManagerFlags.LayoutNoLimits) : base( view)
    {
        var layoutParams = new WindowManagerLayoutParams
        {
            Format = Format.Transparent,
            Gravity = gravity,
            Flags = flags
        };

        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            layoutParams.Type = WindowManagerTypes.ApplicationOverlay;
        }
        else
        {
            layoutParams.Type = WindowManagerTypes.Phone;
        }

        if (OperatingSystem.IsAndroidVersionAtLeast(28))
        {
            layoutParams.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
        }

        layoutParams.Width = LayoutParams.WrapContent;
        layoutParams.Height = LayoutParams.WrapContent;
        layoutParams.X = Util.Dp2Px(x);
        layoutParams.Y = Util.Dp2Px(y);

        this.WindowManager = context.GetSystemService("window").JavaCast<IWindowManager>();
        this.WindowManager.AddView(view, layoutParams);
        this.state = FloatyState.Show;
    }


}
