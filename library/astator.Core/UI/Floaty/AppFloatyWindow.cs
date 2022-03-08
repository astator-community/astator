using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using System;
using static Android.Views.ViewGroup;

namespace astator.Core.UI.Floaty;

/// <summary>
/// 应用悬浮窗, 无需悬浮窗权限
/// </summary>
public class AppFloatyWindow : FloatyWindowBase
{
    private readonly IWindowManager windowManager;

    public AppFloatyWindow(Context context, View view,
           int x = 0,
           int y = 0,
           GravityFlags gravity = GravityFlags.Left | GravityFlags.Top,
           WindowManagerFlags flags = WindowManagerFlags.NotFocusable | WindowManagerFlags.LayoutNoLimits) : base(view)
    {
        this.windowManager = context.GetSystemService("window").JavaCast<IWindowManager>();
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

        this.windowManager?.AddView(view, layoutParams);
    }
}
