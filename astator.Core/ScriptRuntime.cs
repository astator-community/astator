using Android.App;
using Android.Content;
using Android.Media.Projection;
using astator.Core.Graphics;
using astator.Core.Threading;
using astator.Core.UI;
using astator.Core.UI.Floaty;
using astator.Engine;
using System;

namespace astator.Core
{

    public enum ScriptState
    {
        Unstarted = 0,
        Running = 1,
        WaitExit = 2,
        Exiting = 3,
        Exited = 4
    }

    public enum CaptureOrientation
    {
        None = -1,
        Vertical = 0,
        Horizontal = 1,
    }

    public class ScriptRuntime
    {
        public UiManager UiManager { get; private set; }
        public FloatyManager FloatyManager { get; private set; }
        public ScriptThreadManager Threads { get; private set; }
        public ScriptTaskManager Tasks { get; private set; }
        public CaptureOrientation CaptureOrientation { get; set; } = CaptureOrientation.None;
        public bool IsUIMode { get; private set; } = false;
        public ScriptState State { get; private set; }
        public Action ExitCallback { get; set; }
        public string Directory { get; private set; } = string.Empty;
        public string ScriptId { get; }
        public  TemplateActivity Activity { get; private set; }

        private readonly ScriptEngine engine;



        public ScriptRuntime(string id, ScriptEngine engine, TemplateActivity activity, string directory) : this(id, engine, directory)
        {
            this.IsUIMode = true;
            this.Activity = activity;
            this.Activity.OnFinished = () => { SetExit(); };
            this.FloatyManager = new FloatyManager(activity, directory);
            this.UiManager = new UiManager(activity, directory);
        }
        public ScriptRuntime(string id, ScriptEngine engine, string directory)
        {
            this.State = ScriptState.Unstarted;
            this.ScriptId = id;
            this.engine = engine;
            this.Directory = directory;
            this.Threads = new ScriptThreadManager();
            this.Tasks = new ScriptTaskManager();

        }

        public void SetExit()
        {
            this.State = ScriptState.WaitExit;

            if (this.Threads.IsAlive() || this.Tasks.IsAlive())
            {
                this.Threads.ScriptExitCallback = Exit;
                this.Threads.ScriptExitSignal = true;
                this.Tasks.ScriptExitCallback = Exit;
                this.Tasks.ScriptExitSignal = true;
            }
            else
            {
                Exit(0);
            }

            this.Threads.Interrupt();
            this.Tasks.Cancel();
        }

        public void Exit(int type)
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

            this.State = ScriptState.Exiting;

            try
            {
                this.ExitCallback?.Invoke();
                this.FloatyManager?.HideAll();
                ScreenCapturer.Instance?.Dispose();
                this.engine.UnExecute();
                ScriptLogger.Instance.Log("脚本停止运行");
            }
            catch { }
            finally
            {
                this.State = ScriptState.Exited;
            }
        }
    }
}
