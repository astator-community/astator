using System;

namespace astator.Core.Script;

/// <summary>
/// 项目入口方法特性
/// </summary>
public class ProjectEntryMethod : EntryMethod
{
}

/// <summary>
/// 脚本入口方法特性
/// </summary>
public class ScriptEntryMethod : EntryMethod
{
    public string FileName { get; set; } = string.Empty;
}

/// <summary>
/// 入口方法特性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class EntryMethod : Attribute
{
    /// <summary>
    /// 是否使用ui
    /// </summary>
    public bool IsUIMode { get; set; } = false;
}