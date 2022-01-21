using Android.Graphics;
using Android.Runtime;
using Android.Views;
using System;
using static Android.Views.ViewGroup;

namespace astator.Core.UI.Floaty;

/// <summary>
/// 应用悬浮窗, 无需悬浮窗权限
/// </summary>
public class AppFloatyWindow
{
    private static readonly IWindowManager windowManager;

    private readonly View view;
    private bool showed = false;

    static AppFloatyWindow()
    {
        windowManager = Globals.AppContext.GetSystemService("window").JavaCast<IWindowManager>();
    }

    public AppFloatyWindow(View view,
           int x = 0,
           int y = 0,
           GravityFlags gravity = GravityFlags.Left | GravityFlags.Top,
           WindowManagerFlags flags = WindowManagerFlags.NotFocusable | WindowManagerFlags.LayoutNoLimits)
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

        windowManager?.AddView(view, layoutParams);

        this.view = view;
        this.showed = true;
    }

    /// <summary>
    /// 设置悬浮窗位置
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetPosition(int x, int y)
    {
        var layoutParams = this.view.LayoutParameters as WindowManagerLayoutParams;
        layoutParams.X = x;
        layoutParams.Y = y;
        windowManager?.UpdateViewLayout(this.view, layoutParams);
    }

    /// <summary>
    /// 获取悬浮窗位置
    /// </summary>
    /// <returns></returns>
    public Point GetPosition()
    {
        var layoutParams = this.view.LayoutParameters as WindowManagerLayoutParams;
        return new Point(layoutParams.X, layoutParams.Y);
    }

    /// <summary>
    /// 移除悬浮窗
    /// </summary>
    /// <returns></returns>
    public bool Remove()
    {
        if (this.showed)
        {
            this.showed = false;
            windowManager?.RemoveView(this.view);
            return true;
        }
        else
        {
            return false;
        }
    }
}
