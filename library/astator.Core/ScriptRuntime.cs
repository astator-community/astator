using astator.Core.Engine;
using astator.Core.Graphics;
using astator.Core.Threading;
using astator.Core.UI;
using astator.Core.UI.Floaty;
using System;
using System.Collections.Generic;

namespace astator.Core
{
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
        /// ui管理类
        /// </summary>
        public UiManager Ui { get; private set; }

        /// <summary>
        /// 悬浮窗管理类
        /// </summary>
        public FloatyManager Floatys { get; private set; }

        /// <summary>
        /// 线程管理类
        /// </summary>
        public ScriptThreadManager Threads { get; private set; }

        /// <summary>
        /// 任务管理
        /// </summary>
        public ScriptTaskManager Tasks { get; private set; }

        /// <summary>
        /// 是否为ui脚本
        /// </summary>
        public bool IsUiMode { get; private set; } = false;

        /// <summary>
        /// 脚本id
        /// </summary>
        public string ScriptId { get; }

        /// <summary>
        /// 脚本运行状态
        /// </summary>
        public ScriptState State { get; private set; }

        /// <summary>
        /// 脚本停止回调
        /// </summary>
        public List<Action> ExitCallbacks { get; set; } = new();

        /// <summary>
        /// 脚本所在路径
        /// </summary>
        public string Directory { get; private set; } = string.Empty;

        /// <summary>
        /// 脚本自身的activity
        /// </summary>
        public TemplateActivity Context { get; private set; }

        /// <summary>
        /// 在脚本停止时退出应用, 只在打包apk有效
        /// </summary>
        public bool IsExitAppOnStoped { get; set; }

        /// <summary>
        /// 脚本执行引擎
        /// </summary>
        private readonly ScriptEngine engine;

        public ScriptRuntime(string id, ScriptEngine engine, TemplateActivity activity, string directory) : this(id, engine, directory)
        {
            this.IsUiMode = true;
            this.Context = activity;
            this.Context.OnFinished = SetStop;
            this.Ui = new UiManager(activity, directory);
        }
        public ScriptRuntime(string id, ScriptEngine engine, string directory)
        {
            this.State = ScriptState.Unstarted;
            this.ScriptId = id;
            this.engine = engine;
            this.Directory = directory;
            this.Threads = new ScriptThreadManager();
            this.Tasks = new ScriptTaskManager();

            if (!this.IsUiMode)
            {
                this.Threads.ScriptExitCallback = Stop;
                this.Threads.ScriptExitSignal = true;

                this.Tasks.ScriptExitCallback = Stop;
                this.Tasks.ScriptExitSignal = true;
            }

            this.Floatys = new FloatyManager(this.Context ?? Globals.AppContext, directory);
        }

        /// <summary>
        /// 设置停止标志
        /// </summary>
        public void SetStop()
        {
            this.State = ScriptState.WaitStop;

            if (this.Threads.IsAlive() || this.Tasks.IsAlive())
            {
                this.Threads.ScriptExitCallback = Stop;
                this.Threads.ScriptExitSignal = true;
                this.Tasks.ScriptExitCallback = Stop;
                this.Tasks.ScriptExitSignal = true;
            }
            else
            {
                Stop(0);
            }

            this.Threads.Interrupt();
            this.Tasks.Cancel();
        }

        /// <summary>
        /// 停止脚本
        /// </summary>
        /// <param name="type">调用方标志, 1为threads, 2为tasks</param>
        internal void Stop(int type)
        {
            if (type == 1)
            {
                if (this.Tasks.IsAlive())
                {
                    return;
                }
            }
            else if (type == 2)
            {
                if (this.Threads.IsAlive())
                {
                    return;
                }
            }

            this.State = ScriptState.Stopping;

            try
            {
                foreach (var callback in this.ExitCallbacks)
                {
                    callback.Invoke();
                }
                this.Floatys?.RemoveAll();
                ScreenCapturer.Instance?.Dispose();
                this.engine.UnExecute();
                ScriptLogger.Log("脚本停止运行: " + this.ScriptId);

                if (this.IsExitAppOnStoped && Android.App.Application.Context.PackageName != "com.astator.astator")
                {
                    Globals.RunOnUiThread(() =>
                    {
                        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                    });
                }
            }
            catch { }
            finally
            {
                this.State = ScriptState.Exited;
            }
        }
    }
}
