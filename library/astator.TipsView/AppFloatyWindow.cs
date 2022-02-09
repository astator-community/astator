using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using static Android.Views.ViewGroup;
using Point = Android.Graphics.Point;
using View = Android.Views.View;

namespace astator.TipsView;

/// <summary>
/// 应用悬浮窗, 无需悬浮窗权限
/// </summary>
internal class AppFloatyWindow
{
    private readonly IWindowManager windowManager;
    private readonly View view;
    private bool showed = false;


    public AppFloatyWindow(Context context, View view,
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

        this.windowManager = context.GetSystemService("window").JavaCast<IWindowManager>();
        this.windowManager?.AddView(view, layoutParams);

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
        this.windowManager?.UpdateViewLayout(this.view, layoutParams);
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
            this.windowManager?.RemoveView(this.view);
            return true;
        }
        else
        {
            return false;
        }
    }
}
