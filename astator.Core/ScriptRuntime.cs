using astator.Core.Engine;
using astator.Core.Graphics;
using astator.Core.Threading;
using astator.Core.UI;
using astator.Core.UI.Floaty;
using System;
using static astator.Core.Globals.Permission;

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

    public class ScriptRuntime
    {
        public UiManager Ui { get; private set; }
        public FloatyManager Floatys { get; private set; }
        public ScriptThreadManager Threads { get; private set; }
        public ScriptTaskManager Tasks { get; private set; }
        public bool IsUiMode { get; private set; } = false;
        public string ScriptId { get; }
        public ScriptState State { get; private set; }
        public Action ExitCallback { get; set; }
        public string Directory { get; private set; } = string.Empty;
        public CaptureOrientation CaptureOrientation { get; set; } = CaptureOrientation.None;
        public TemplateActivity Activity { get; private set; }

        private readonly ScriptEngine engine;

        public ScriptRuntime(string id, ScriptEngine engine, TemplateActivity activity, string directory) : this(id, engine, directory)
        {
            this.IsUiMode = true;
            this.Activity = activity;
            this.Activity.OnFinished = () => { SetExit(); };
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
            this.Floatys = new FloatyManager(Activity ?? Globals.MainActivity, directory);
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
                this.Floatys?.HideAll();
                ScreenCapturer.Instance?.Dispose();
                this.engine.UnExecute();
                ScriptLogger.Instance.Log("脚本停止运行" + this.ScriptId);
            }
            catch { }
            finally
            {
                this.State = ScriptState.Exited;
            }
        }
    }
}
