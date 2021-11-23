using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using astator.Core.Graphics;
using astator.Core.UI.Floaty;
using System;
using System.Collections.Generic;
using static astator.Core.Globals.Permission;

namespace astator.Core.UI
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    public class TemplateActivity : AppCompatActivity
    {
        public static Dictionary<string, TemplateActivity> ScriptActivityList { get; set; } = new();
        public Action OnFinished { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ScriptActivityList.Add(this.Intent.GetStringExtra("id"), this);
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void Finish()
        {
            base.Finish();
            this.OnFinished?.Invoke();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                if (requestCode == (int)RequestFlags.MediaProjection)
                {
                    Intent intent = new(this, typeof(ScreenCapturer));
                    intent.PutExtra("data", data);
                    StartForegroundService(intent);
                }
                else if (requestCode == (int)RequestFlags.FloatyWindow)
                {
                    StartService(new(this, typeof(FloatyService)));
                }
            }
        }
        public override void StartActivityForResult(Intent intent, int requestCode)
        {
            base.StartActivityForResult(intent, requestCode);
        }
    }
}
