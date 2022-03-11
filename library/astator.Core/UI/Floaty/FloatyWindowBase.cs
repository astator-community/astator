using Android.Graphics;
using Android.Views;
using astator.Core.UI.Base;

namespace astator.Core.UI.Floaty;

public enum FloatyState
{
    Initialize,
    Show,
    Hide,
    Remove
}

public class FloatyWindowBase
{
    internal readonly View view;
    internal FloatyState state = FloatyState.Initialize;

    public FloatyWindowBase(View view)
    {
        this.view = view;
    }

    /// <summary>
    /// 设置悬浮窗位置
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetPosition(int x, int y)
    {
        var layoutParams = this.view.LayoutParameters as WindowManagerLayoutParams;
        layoutParams.X = Util.Dp2Px(x);
        layoutParams.Y = Util.Dp2Px(y);

        if (this is AppFloatyWindow appFloaty) appFloaty.windowManager.UpdateViewLayout(view, layoutParams);
        else FloatyService.Instance?.UpdateViewLayout(this.view, layoutParams);
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
    /// 显示悬浮窗
    /// </summary>
    public bool Show()
    {
        if (this.state == FloatyState.Initialize || this.state == FloatyState.Hide)
        {
            this.view.Visibility = ViewStates.Visible;
            this.state = FloatyState.Show;
            return true;
        }
        return false;

    }

    /// <summary>
    /// 隐藏悬浮窗
    /// </summary>
    public bool Hide()
    {
        if (this.state == FloatyState.Show)
        {
            this.view.Visibility = ViewStates.Invisible;
            this.state = FloatyState.Hide;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 移除悬浮窗
    /// </summary>
    /// <returns></returns>
    public bool Remove()
    {
        if (this.state == FloatyState.Show || this.state == FloatyState.Hide)
        {
            if (this is AppFloatyWindow appFloaty) appFloaty.windowManager.RemoveView(view);
            else FloatyService.Instance?.RemoveView(this.view);

            this.state = FloatyState.Remove;
            return true;
        }
        else
        {
            return false;
        }
    }
}
