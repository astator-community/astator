using Android.App;
using Android.Content.Res;
using Android.Runtime;
using astator.Core.Graphics;

namespace astator
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }

        protected override MauiApp CreateMauiApp()
        {
            return MauiProgram.CreateMauiApp();
        }
    }
}