using Android.OS;
using Android.Views.Accessibility;
using astator.Core.Graphics;
using System.Linq;
using Action = Android.Views.Accessibility.Action;

namespace astator.Core.Accessibility;

public static partial class NodeInfoExtension
{
    /// <summary>
    /// 获取Id
    /// </summary>
    /// <param name="nodeInfo"></param>
    /// <returns></returns>
    public static string GetId(this AccessibilityNodeInfo nodeInfo)
    {
        var ids = nodeInfo.ViewIdResourceName.Split('/');
        if (ids.Length >= 2)
        {
            return ids.Last();
        }
        return null;
    }

    /// <summary>
    /// 获取控件所在范围
    /// </summary>
    /// <param name="nodeInfo"></param>
    /// <returns></returns>
    public static Rect GetBounds(this AccessibilityNodeInfo nodeInfo)
    {
        var bounds = new Android.Graphics.Rect();
        nodeInfo.GetBoundsInScreen(bounds);

        var result = new Rect(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
        return result;
    }

    /// <summary>
    /// 获取控件相对于根节点的深度
    /// </summary>
    /// <param name="nodeInfo"></param>
    /// <returns></returns>
    public static int GetDepth(this AccessibilityNodeInfo nodeInfo)
    {
        var depth = 0;
        var parent = nodeInfo.Parent;
        while (parent is not null)
        {
            depth++;
            parent = parent.Parent;
        }
        return depth;
    }

    /// <summary>
    /// 点击控件
    /// </summary>
    /// <param name="nodeInfo"></param>
    /// <returns></returns>
    public static bool Click(this AccessibilityNodeInfo nodeInfo)
    {
        return nodeInfo.PerformAction(Action.Click);
    }

    /// <summary>
    /// 长按控件
    /// </summary>
    /// <param name="nodeInfo"></param>
    /// <returns></returns>
    public static bool LongClick(this AccessibilityNodeInfo nodeInfo)
    {
        return nodeInfo.PerformAction(Action.LongClick);
    }

    /// <summary>
    /// 设置编辑框文本
    /// </summary>
    /// <param name="nodeInfo"></param>
    /// <returns></returns>
    public static bool SetText(this AccessibilityNodeInfo nodeInfo, string text)
    {
        var args = new Bundle();
        args.PutCharSequence(AccessibilityNodeInfo.ActionArgumentSetTextCharsequence, text);
        return nodeInfo.PerformAction(Action.SetText, args);
    }

    /// <summary>
    /// 选中编辑框内容
    /// </summary>
    /// <param name="nodeInfo"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static bool SetSelectionText(this AccessibilityNodeInfo nodeInfo, int start, int end)
    {
        try
        {
            nodeInfo.SetTextSelection(start, end);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 向前滚动控件内容
    /// </summary>
    /// <param name="nodeInfo"></param>
    /// <returns></returns>
    public static bool ScrollForward(this AccessibilityNodeInfo nodeInfo)
    {
        return nodeInfo.PerformAction(Action.ScrollForward);
    }

    /// <summary>
    /// 向后滚动控件内容
    /// </summary>
    /// <param name="nodeInfo"></param>
    /// <returns></returns>
    public static bool ScrollBackward(this AccessibilityNodeInfo nodeInfo)
    {
        return nodeInfo.PerformAction(Action.ScrollBackward);
    }

}
