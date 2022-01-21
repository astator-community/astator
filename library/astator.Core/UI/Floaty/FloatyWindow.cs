using Android.Graphics;
using Android.Views;
using System;
using static Android.Views.ViewGroup;
namespace astator.Core.UI.Floaty
{
    /// <summary>
    /// 全局悬浮窗, 需要悬浮窗权限
    /// </summary>
    public class FloatyWindow
    {

        private readonly View view;
        private bool showed = false;

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
            layoutParams.X = x;
            layoutParams.Y = y;

            FloatyService.Instance?.AddView(view, layoutParams);
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
        /// 移除悬浮窗
        /// </summary>
        /// <returns></returns>
        public bool Remove()
        {
            if (this.showed)
            {
                this.showed = false;
                FloatyService.Instance?.RemoveView(this.view);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
