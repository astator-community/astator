using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using System;
using System.Collections.Generic;

namespace astator.Core.UI.Base;

[Activity(Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
public class TemplateActivity : AppCompatActivity
{
    public static Dictionary<string, TemplateActivity> ScriptActivityList { get; set; } = new();
    public Action OnFinished { get; set; }
    public Action OnResumed { get; set; }

    private string scriptId = string.Empty;

    protected override void OnCreate(Bundle savedInstanceState)
    {
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
        this.OnResumed?.Invoke();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void Finish()
    {
        base.Finish();
        ScriptActivityList.Remove(this.scriptId);
        this.OnFinished?.Invoke();
    }
}
