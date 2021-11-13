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

    public class ScriptRuntime
    {
        public UIManager UiManager { get; set; }
        public FloatyManager FloatyManager { get; set; }
        public ScriptThreads Threads { get; set; }
        public bool CaptureOrientation { get; set; } = false;
        public bool IsUIMode { get; set; } = false;
        public ScriptState State { get; private set; }

        public Action ExitCallback { get; set; }

        public string directory { get; private set; } = string.Empty;

        private readonly string scriptId;

        private readonly TemplateActivity activity;

        private readonly Activity baseActivity;

        private readonly ScriptEngine engine;



        public ScriptRuntime(string id, ScriptEngine engine, TemplateActivity activity, Activity baseActivity, string directory) : this(id, engine, baseActivity, directory)
        {
            this.IsUIMode = true;
            this.activity = activity;
            this.activity.OnFinished = () => { Exit(); };
            this.FloatyManager = new FloatyManager(activity, directory);
            this.UiManager = new UIManager(activity, directory);
        }
        public ScriptRuntime(string id, ScriptEngine engine, Activity baseActivity, string directory)
        {
            this.State = ScriptState.Unstarted;
            this.scriptId = id;
            this.engine = engine;
            this.baseActivity = baseActivity;
            this.directory = directory;
            this.Threads = new ScriptThreads(Exit);

        }
        public void ReqScreenCapture(bool orientation)
        {
            this.CaptureOrientation = orientation;
            var manager = (MediaProjectionManager)this.activity?.GetSystemService("media_projection");
            if (manager is not null)
            {
                var intent = manager.CreateScreenCaptureIntent();
                intent?.PutExtra("id", this.scriptId);
                intent?.PutExtra("orientation", orientation);
                (this.activity ?? this.baseActivity).StartActivityForResult(intent, (int)RequestFlags.media_projection);
            }
        }

        public void ReqFloaty()
        {
            if (!Android.Provider.Settings.CanDrawOverlays(this.activity))
            {
                (this.activity ?? this.baseActivity).StartActivityForResult(new Intent(Android.Provider.Settings.ActionManageOverlayPermission, Android.Net.Uri.Parse("package:" + (this.activity ?? this.baseActivity).PackageName)), (int)RequestFlags.floaty_window);
            }
        }
        public bool CheckFloaty()
        {
            return Android.Provider.Settings.CanDrawOverlays(this.activity);
        }
        public void StartFloatyService()
        {
            if (FloatyService.Instance is null && this.activity is not null)
            {
                (this.activity ?? this.baseActivity).StartService(new((this.activity ?? this.baseActivity), typeof(FloatyService)));
            }
        }


        public void SetExit()
        {
            this.State = ScriptState.WaitExit;
            this.Threads.Interrupt();
        }

        public void Exit()
        {
            if (this.State == ScriptState.WaitExit)
            {
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
            }
        }
    }
}
