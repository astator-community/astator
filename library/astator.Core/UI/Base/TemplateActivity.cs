using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using System;
using System.Collections.Generic;

namespace astator.Core.UI.Base;

[Activity(Theme = "@style/AppTheme.NoActionBar")]
public class TemplateActivity : AppCompatActivity, IActivity
{
    public static Dictionary<string, TemplateActivity> ScriptActivityList { get; set; } = new();

    public LifecycleObserver LifecycleObserver { get; set; }
    public Action OnFinishedCallback { get; set; }
    public Action OnResumeCallback { get; set; }
    public Func<Keycode, KeyEvent, bool> OnKeyDownCallback { get; set; }

    private string scriptId = string.Empty;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        this.LifecycleObserver = new LifecycleObserver(this);
        this.Lifecycle.AddObserver(this.LifecycleObserver);

        base.OnCreate(savedInstanceState);
        this.scriptId = this.Intent.GetStringExtra("id");
        if (ScriptActivityList.ContainsKey(this.scriptId))
        {
            ScriptActivityList.Remove(this.scriptId);
        }
        ScriptActivityList.Add(this.scriptId, this);
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnResume()
    {
        base.OnResume();
        this.OnResumeCallback?.Invoke();
    }

    public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
    {
        if (this.OnKeyDownCallback?.Invoke(keyCode, e) == true) return true;
        return base.OnKeyDown(keyCode, e);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void Finish()
    {
        base.Finish();
        ScriptActivityList.Remove(this.scriptId);
        this.OnFinishedCallback?.Invoke();
    }
}
