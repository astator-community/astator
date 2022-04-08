using System;
using System.Collections.Generic;
using Android.App;
using astator.Core.Engine;
using astator.Core.Threading;
using astator.Core.UI;
using astator.Core.UI.Base;
using astator.Core.UI.Floaty;
using NLog;

namespace astator.Core.Script;

/// <summary>
/// 脚本运行状态
/// </summary>
public enum ScriptState
{
    /// <summary>
    /// 未启动
    /// </summary>
    Unstarted = 0,

    /// <summary>
    /// 正在运行
    /// </summary>
    Running = 1,

    /// <summary>
    /// 等待停止
    /// </summary>
    WaitStop = 2,

    /// <summary>
    /// 正在停止
    /// </summary>
    Stopping = 3,

    /// <summary>
    /// 已经停止
    /// </summary>
    Exited = 4
}

public class ScriptRuntime
{

    /// <summary>
    /// 脚本id
    /// </summary>
    public string ScriptId { get; private set; }

    /// <summary>
    /// ui管理类
    /// </summary>
    public UiManager Ui { get; private set; }

    /// <summary>
    /// 悬浮窗相关
    /// </summary>
    public FloatyHelper FloatyHelper { get; private set; }

    /// <summary>
    /// 线程管理类
    /// </summary>
    public ThreadManager Threads { get; private set; }

    /// <summary>
    /// Task管理类
    /// </summary>
    public TaskManager Tasks { get; private set; }

    /// <summary>
    /// 权限相关
    /// </summary>
    public PermissionHelper PermissionHelper { get; private set; }

    /// <summary>
    /// 是否为ui模式
    /// </summary>
    public bool IsUiMode { get; private set; } = false;

    /// <summary>
    /// 脚本运行状态
    /// </summary>
    public ScriptState State { get; private set; }

    /// <summary>
    /// 脚本工作路径
    /// </summary>
    public string WorkDir { get; private set; } = string.Empty;

    /// <summary>
    /// 脚本自身的activity, 非ui模式时为null
    /// </summary>
    public TemplateActivity Activity { get; private set; }

    /// <summary>
    /// 在脚本停止时退出应用, 只在打包apk有效
    /// </summary>
    public bool IsExitAppOnStoped { get; set; }

    /// <summary>
    /// 脚本执行引擎
    /// </summary>
    private readonly ScriptEngine engine;

    /// <summary>
    /// 脚本停止回调集合
    /// </summary>
    private readonly List<Action> exitCallbacks = new();

    public ScriptRuntime(string id, ScriptEngine engine, TemplateActivity activity, string dir) : this(id, engine, dir, activity)
    {
        this.IsUiMode = true;
        this.Activity = activity;
        this.Activity.OnFinishedCallback = SetStop;
        this.Ui = new UiManager(activity, dir);
    }

    public ScriptRuntime(string id, ScriptEngine engine, string directory, Activity activity = null)
    {
        this.State = ScriptState.Unstarted;
        this.ScriptId = id;
        this.engine = engine;
        this.WorkDir = directory;
        this.Threads = new ThreadManager();
        this.Tasks = new TaskManager();

        if (activity is null)
        {
            this.Threads.ScriptExitCallback = Stop;
            this.Threads.ScriptExitSignal = true;
            this.Tasks.ScriptExitCallback = Stop;
            this.Tasks.ScriptExitSignal = true;
        }

        this.FloatyHelper = new FloatyHelper(activity ?? Globals.AppContext, directory);
        this.PermissionHelper = new PermissionHelper(activity ?? Globals.AppContext as Activity);
    }

    /// <summary>
    /// 停止脚本
    /// </summary>
    public void SetStop()
    {
        if (this.State == ScriptState.Running || this.State == ScriptState.Unstarted)
        {
            this.State = ScriptState.WaitStop;

            if (this.Threads.IsAlive() || this.Tasks.IsAlive())
            {
                this.Threads.ScriptExitCallback = Stop;
                this.Threads.ScriptExitSignal = true;
                this.Tasks.ScriptExitCallback = Stop;
                this.Tasks.ScriptExitSignal = true;
                this.Threads.Interrupt();
                this.Tasks.Cancel();
            }
            else
            {
                Stop(0);
            }
        }
    }

    /// <summary>
    /// 添加一个脚本停止时的回调
    /// </summary>
    /// <param name="callback"></param>
    public void AddExitCallback(Action callback)
    {
        this.exitCallbacks.Add(callback);
    }

    /// <summary>
    /// 添加一个logger的回调
    /// </summary>
    /// <param name="callback"></param>
    /// <returns>回调的key</returns>
    public string AddLoggerCallback(Action<LogLevel, DateTime, string> callback)
    {
        return ScriptLogger.AddCallback(this.ScriptId, callback);
    }

    /// <summary>
    /// 移除logger的回调
    /// </summary>
    /// <param name="key">回调的key, 当key为空时移除当前runtime添加的所有回调</param>
    public void RemoveLoggerCallback(string key = null)
    {
        ScriptLogger.RemoveCallback(key ?? this.ScriptId);
    }


    /// <summary>
    /// 停止脚本
    /// </summary>
    /// <param name="type">调用方标志, 1为threads, 2为tasks</param>
    internal void Stop(int type)
    {
        Globals.InvokeOnMainThreadAsync(() =>
        {
            if (type == 1)
            {
                if (this.Tasks.IsAlive()) return;
            }
            else if (type == 2)
            {
                if (this.Threads.IsAlive()) return;
            }

            this.State = ScriptState.Stopping;

            try
            {
                RemoveLoggerCallback();

                foreach (var callback in this.exitCallbacks)
                {
                    callback.Invoke();
                }
                this.FloatyHelper?.RemoveAll();


                if (this.IsUiMode)
                {
                    if (!this.Activity.IsFinishing && !this.Activity.IsDestroyed)
                    {
                        this.Activity.Finish();
                    }
                }
                if (this.IsExitAppOnStoped && Application.Context.PackageName != Globals.AstatorPackageName)
                {
                    (Globals.AppContext as Activity).Finish();
                    Java.Lang.JavaSystem.Exit(0);
                }

                this.engine.UnExecute();
                ScriptLogger.Log("脚本停止运行: " + this.ScriptId);
            }
            catch { }
            finally
            {
                this.State = ScriptState.Exited;
            }
        });
    }
}
