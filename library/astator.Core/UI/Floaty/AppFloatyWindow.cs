using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using static Android.Views.ViewGroup;

namespace astator.Core.UI.Floaty;

/// <summary>
/// 应用悬浮窗, 无需悬浮窗权限
/// </summary>
public class AppFloatyWindow : FloatyWindowBase
{
    public AppFloatyWindow(Context context, View view,
           int x = 0,
           int y = 0,
           GravityFlags gravity = GravityFlags.Left | GravityFlags.Top,
           WindowManagerFlags flags = WindowManagerFlags.NotFocusable | WindowManagerFlags.LayoutNoLimits) : base(view)
    {
        var layoutParams = new WindowManagerLayoutParams
        {
            Type = WindowManagerTypes.ApplicationPanel,
            Format = Format.Transparent,
            Gravity = gravity,
            Flags = flags
        };

        if (OperatingSystem.IsAndroidVersionAtLeast(28))
        {
            layoutParams.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
        }

        layoutParams.Width = LayoutParams.WrapContent;
        layoutParams.Height = LayoutParams.WrapContent;
        layoutParams.X = x;
        layoutParams.Y = y;

        this.WindowManager = context.GetSystemService("window").JavaCast<IWindowManager>();
        this.WindowManager?.AddView(view, layoutParams);
        this.state = FloatyState.Show;
    }
}
