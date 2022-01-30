using Android.AccessibilityServices;
using Android.Graphics;
using Android.Views.Accessibility;
using System.Collections.Generic;
using System.Linq;

namespace astator.Core.Accessibility
{
    /// <summary>
    /// 自动化操作类
    /// </summary>
    public static class Automator
    {
        private static ScriptAccessibilityService Service => ScriptAccessibilityService.Instance;

        /// <summary>
        /// 坐标点击
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="duration">持续时间, 默认1ms</param>
        public static void Click(int x, int y, int duration = 1)
        {
            var path = new Path();
            path.MoveTo(x, y);
            var gesture = new GestureDescription.Builder().AddStroke(new GestureDescription.StrokeDescription(path, 0L, duration)).Build();
            if (gesture != null)
            {
                Service?.DispatchGesture(gesture, null, null);
            }
        }

        /// <summary>
        /// 滑动
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <param name="duration">持续时间</param>
        public static void Swipe(int startX, int startY, int endX, int endY, int duration)
        {
            var path = new Path();
            path.MoveTo(startX, startY);
            path.LineTo(endX, endY);
            var gesture = new GestureDescription.Builder().AddStroke(new GestureDescription.StrokeDescription(path, 0L, duration)).Build();
            if (gesture is not null)
            {
                Service?.DispatchGesture(gesture, null, null);
            }
        }

        /// <summary>
        /// 获取所有窗口
        /// </summary>
        /// <returns></returns>
        public static List<AccessibilityWindowInfo> GetWindows()
        {
            return Service.Windows.ToList();
        }

        /// <summary>
        /// 获取当前活动应用的窗口
        /// </summary>
        /// <returns></returns>
        public static AccessibilityWindowInfo GetCurrentWindow()
        {
            var windows = GetWindows();

            var result = from window in windows
                         where window.Type == AccessibilityWindowType.Application
                         where window.Root is not null
                         select window;

            return result.First();
        }

        /// <summary>
        /// 获取当前活动应用包名
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentPackageName()
        {
            return GetCurrentWindow().Root.PackageName;
        }

        /// <summary>
        /// 获取当前活动应用的窗口节点
        /// </summary>
        /// <returns></returns>
        public static AccessibilityNodeInfo GetCurrentWindowInfo()
        {
            return GetCurrentWindow().Root;
        }
    }

}
