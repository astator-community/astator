using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using static Android.Views.ViewGroup;
namespace astator.Core.UI.Floaty
{
    [Service(Label = ".floatys")]
    public class FloatyService : Service
    {
        public static FloatyService Instance { get; set; }
        private IWindowManager windowManager;
        public void AddView(View view, LayoutParams layoutParams)
        {
            this.windowManager.AddView(view, layoutParams);
        }
        public void UpdateViewLayout(View view, LayoutParams layoutParams)
        {
            this.windowManager.UpdateViewLayout(view, layoutParams);
        }
        public void RemoveView(View view)
        {
            this.windowManager?.RemoveView(view);
        }

        public override IBinder OnBind(Intent intent)
        {
            throw new System.NotImplementedException();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            this.windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
            Instance = this;
            return base.OnStartCommand(intent, flags, startId);
        }

    }
}
