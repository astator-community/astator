using System.Collections.Generic;
using System.Linq;
using Android.AccessibilityServices;
using Android.Graphics;
using Android.Views.Accessibility;
using astator.Core.Script;

namespace astator.Core.Accessibility;

/// <summary>
/// 自动化操作类
/// </summary>
public static class Automator
{
    private static ScriptAccessibilityService Service => ScriptAccessibilityService.Instance;

    /// <summary>
    /// 点击坐标
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
    /// 点击坐标(给定范围的中心点)
    /// </summary>
    /// <param name="bounds">范围</param>
    /// <param name="duration">持续时间, 默认1ms</param>
    public static void Click(Graphics.Rect bounds, int duration = 1)
    {
        var path = new Path();
        var x = bounds.GetCenterX();
        var y = bounds.GetCenterY();
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
    /// 获取所有窗口根节点
    /// </summary>
    /// <returns></returns>
    public static List<AccessibilityNodeInfo> GetWindowRoots()
    {
        var result = new List<AccessibilityNodeInfo>();
        foreach (var window in Service.Windows)
        {
            result.Add(window.Root);
        }
        return result;
    }

    /// <summary>
    /// 获取指定包名的窗口根节点
    /// </summary>
    /// <param name="pkgName"></param>
    /// <returns></returns>
    public static AccessibilityNodeInfo GetWindowRoot(string pkgName)
    {
        var nodes = GetWindowRoots();

        var filterNodes = from node in nodes
                          where node is not null
                          where node.PackageName == pkgName
                          select node;

        AccessibilityNodeInfo result = null;
        foreach (var node in filterNodes)
        {
            if (result is not null)
            {
                if (node.GetBounds().GetHeight() > result.GetBounds().GetHeight())
                {
                    result = node;
                }
            }
            else
            {
                result = node;
            }
        }

        return result;
    }

    /// <summary>
    /// 获取当前活动应用的窗口根节点
    /// </summary>
    /// <returns></returns>
    public static AccessibilityNodeInfo GetCurrentWindowRoot()
    {
        var pkgName = Globals.GetCurrentPackageName();
        if (!string.IsNullOrEmpty(pkgName))
        {
            return GetWindowRoot(pkgName);
        }
        else
        {
            return Service.RootInActiveWindow;
        }
    }

    /// <summary>
    /// 获取当前活动应用包名
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentPackageName()
    {
        return GetCurrentWindowRoot().PackageName;
    }
}
