using Android.Views.Accessibility;
using astator.Core.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace astator.Core.Accessibility;

public struct SearcherArgs
{
    public string Id { get; set; } = null;
    public string PackageName { get; set; } = null;
    public string PackageNameStartsWith { get; set; } = null;
    public string PackageNameEndsWith { get; set; } = null;
    public string ClassName { get; set; } = null;
    public string Text { get; set; } = null;
    public string Description { get; set; } = null;
    public Rect? Bounds { get; set; } = null;
    public bool? Checkable { get; set; } = null;
    public bool? Clickable { get; set; } = null;
    public bool? Editable { get; set; } = null;
    public bool? Enabled { get; set; } = null;
    public bool? Focusable { get; set; } = null;
    public int? Depth { get; set; } = null;
    public int? DrawingOrder { get; set; } = null;

    public SearcherArgs()
    {

    }

    public Dictionary<string, dynamic> GetEnabledArgs()
    {
        var result = new Dictionary<string, dynamic>();

        if (this.Id is not null)
        {
            result.Add("Id", this.Id);
        }

        if (this.PackageName is not null)
        {
            result.Add("PackageName", this.PackageName);
        }

        if (this.PackageNameStartsWith is not null)
        {
            result.Add("PackageNameStartsWith", this.PackageNameStartsWith);
        }

        if (this.PackageNameEndsWith is not null)
        {
            result.Add("PackageNameEndsWith", this.PackageNameEndsWith);
        }

        if (this.Text is not null)
        {
            result.Add("Text", this.Text);
        }

        if (this.Description is not null)
        {
            result.Add("Description", this.Description);
        }

        if (this.Bounds is not null)
        {
            result.Add("Bounds", this.Bounds);
        }

        if (this.Checkable is not null)
        {
            result.Add("Checkable", this.Checkable);
        }

        if (this.Clickable is not null)
        {
            result.Add("Clickable", this.Clickable);
        }

        if (this.Editable is not null)
        {
            result.Add("Editable", this.Editable);
        }

        if (this.Enabled is not null)
        {
            result.Add("Enabled", this.Enabled);
        }

        if (this.Focusable is not null)
        {
            result.Add("Focusable", this.Focusable);
        }

        if (this.Depth is not null)
        {
            result.Add("Depth", this.Depth);
        }

        if (this.DrawingOrder is not null)
        {
            result.Add("DrawingOrder", this.DrawingOrder);
        }

        return result;
    }

}


public static partial class NodeInfoExtension
{
    /// <summary>
    /// 在给定的节点查找符合条件的所有节点
    /// </summary>
    /// <param name="nodeInfo"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static List<AccessibilityNodeInfo> Find(this AccessibilityNodeInfo nodeInfo, SearcherArgs args)
    {
        var enabledArgs = args.GetEnabledArgs();
        return Find(new List<AccessibilityNodeInfo>() { nodeInfo }, enabledArgs);
    }

    /// <summary>
    /// 在给定的节点查找符合条件的第一个节点
    /// </summary>
    /// <param name="nodeInfo"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static AccessibilityNodeInfo FindOne(this AccessibilityNodeInfo nodeInfo, SearcherArgs args)
    {
        var enabledArgs = args.GetEnabledArgs();

        return FindOne(new List<AccessibilityNodeInfo>() { nodeInfo }, enabledArgs);
    }

    private static List<AccessibilityNodeInfo> Find(List<AccessibilityNodeInfo> nodeInfos, Dictionary<string, dynamic> args)
    {
        var result = new List<AccessibilityNodeInfo>();

        var childs = new List<AccessibilityNodeInfo>();

        foreach (var nodeInfo in nodeInfos)
        {
            if (nodeInfo.Match(args))
            {
                result.Add(nodeInfo);
            }

            for (var i = 0; i < nodeInfo.ChildCount; i++)
            {
                childs.Add(nodeInfo.GetChild(i));
            }
        }

        if (childs.Any())
        {
            result = result.Concat(Find(childs, args)).ToList();
        }
        return result;
    }

    private static AccessibilityNodeInfo FindOne(List<AccessibilityNodeInfo> nodeInfos, Dictionary<string, dynamic> args)
    {
        var childs = new List<AccessibilityNodeInfo>();

        foreach (var nodeInfo in nodeInfos)
        {
            if (nodeInfo.Match(args))
            {
                return nodeInfo;
            }

            for (var i = 0; i < nodeInfo.ChildCount; i++)
            {
                childs.Add(nodeInfo.GetChild(i));
            }
        }

        if (childs.Any())
        {
            return FindOne(childs, args);
        }
        return null;
    }


    private static bool Match(this AccessibilityNodeInfo nodeInfo, Dictionary<string, dynamic> args)
    {
        foreach (var arg in args)
        {
            var attr = nodeInfo.GetAttr(arg.Key);
            if (arg.Key == "Bounds")
            {
                var left = (Rect)attr;
                var right = (Rect)arg.Value;
                if (left.Left > right.Left
                    || left.Top > right.Top
                    || left.Right > right.Right
                    || left.Bottom > right.Bottom)
                {
                    return false;
                }
            }
            else if (arg.Key == "PackageNameStartsWith")
            {
                var name = attr as string;
                if (!name.StartsWith(arg.Value))
                {
                    return false;
                }
            }
            else if (arg.Key == "PackageNameEndsWith")
            {
                var name = attr as string;
                if (!name.EndsWith(arg.Value))
                {
                    return false;
                }
            }
            else
            {
                if (attr is null || attr != arg.Value)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static dynamic GetAttr(this AccessibilityNodeInfo nodeInfo, string key)
    {
        return key switch
        {
            "Id" => nodeInfo.GetId(),
            "PackageName" => nodeInfo.PackageName,
            "PackageNameStartsWith" => nodeInfo.PackageName,
            "PackageNameEndsWith" => nodeInfo.PackageName,
            "Text" => nodeInfo.Text?.Trim(),
            "Description" => nodeInfo.ContentDescription,
            "Bounds" => nodeInfo.GetBounds(),
            "Checkable" => nodeInfo.Checkable,
            "Clickable" => nodeInfo.Clickable,
            "Editable" => nodeInfo.Editable,
            "Enabled" => nodeInfo.Enabled,
            "Focusable" => nodeInfo.Focusable,
            "Depth" => nodeInfo.GetDepth(),
            "DrawingOrder" => nodeInfo.DrawingOrder,

            _ => null
        };
    }

}
