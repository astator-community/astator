using Android.Graphics;
using Android.Views;
using System;
using static Android.Views.ViewGroup;
namespace astator.Core.UI.Floaty
{
    /// <summary>
    /// 悬浮窗
    /// </summary>
    public class FloatWindow
    {
        private readonly View view;
        private bool showed = false;

        /// <summary>
        /// 构造函数, 创建一个悬浮窗
        /// </summary>
        /// <param name="view"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public FloatWindow(View view, int x = 0, int y = 0)
        {
            this.view = view;
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
            layoutParams.Gravity = GravityFlags.Left | GravityFlags.Top;
            layoutParams.Flags = WindowManagerFlags.NotFocusable | WindowManagerFlags.LayoutNoLimits;

            if (OperatingSystem.IsAndroidVersionAtLeast(30))
            {
                layoutParams.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
            }

            layoutParams.Width = LayoutParams.WrapContent;
            layoutParams.Height = LayoutParams.WrapContent;
            layoutParams.X = x;
            layoutParams.Y = y;
            FloatyService.Instance?.AddView(view, layoutParams);
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
        internal bool Remove()
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
