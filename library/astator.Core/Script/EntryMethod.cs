using System;

namespace astator.Core.Script;

/// <summary>
/// 入口方法特性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class EntryMethod : Attribute
{
    /// <summary>
    /// 是否使用ui
    /// </summary>
    public bool IsUIMode { get; set; }
}
