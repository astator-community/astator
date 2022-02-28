using Android.Graphics;
using Android.Views;
using astator.Core.UI.Base;
using System;
using static Android.Views.ViewGroup;
namespace astator.Core.UI.Floaty
{
    public enum FloatyState
    {
        Initialize,
        Show,
        Hide,
        Remove
    }

    /// <summary>
    /// 全局悬浮窗, 需要悬浮窗权限
    /// </summary>
    public class FloatyWindow
    {

        private readonly View view;
        private FloatyState state = FloatyState.Initialize;

        /// <summary>
        /// 构造函数, 创建一个悬浮窗
        /// </summary>
        /// <param name="view"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public FloatyWindow(View view,
            int x = 0,
            int y = 0,
            GravityFlags gravity = GravityFlags.Left | GravityFlags.Top,
            WindowManagerFlags flags = WindowManagerFlags.NotFocusable | WindowManagerFlags.LayoutNoLimits)
        {
            var layoutParams = new WindowManagerLayoutParams();
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                layoutParams.Type = WindowManagerTypes.ApplicationOverlay;
            }
            else
            {
                layoutParams.Type = WindowManagerTypes.Phone;
            }
            layoutParams.Format = Format.Transparent;
            layoutParams.Gravity = gravity;
            layoutParams.Flags = flags;

            if (OperatingSystem.IsAndroidVersionAtLeast(28))
            {
                layoutParams.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
            }

            layoutParams.Width = LayoutParams.WrapContent;
            layoutParams.Height = LayoutParams.WrapContent;
            layoutParams.X = Util.Dp2Px(x);
            layoutParams.Y = Util.Dp2Px(y);

            FloatyService.Instance?.AddView(view, layoutParams);
            this.view = view;
            this.state = FloatyState.Show;
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
            FloatyService.Instance?.UpdateViewLayout(this.view, layoutParams);
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
        public void Show()
        {
            if (this.state == FloatyState.Initialize || this.state == FloatyState.Hide)
            {
                this.view.Visibility = ViewStates.Visible;
                this.state = FloatyState.Show;
            }

        }

        /// <summary>
        /// 隐藏悬浮窗
        /// </summary>
        public void Hide()
        {
            if (this.state == FloatyState.Show)
            {
                this.view.Visibility = ViewStates.Invisible;
                this.state = FloatyState.Hide;
            }
        }

        /// <summary>
        /// 移除悬浮窗
        /// </summary>
        /// <returns></returns>
        public bool Remove()
        {
            if (this.state == FloatyState.Show || this.state == FloatyState.Hide)
            {
                FloatyService.Instance?.RemoveView(this.view);
                this.state = FloatyState.Remove;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
