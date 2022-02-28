using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Core.App;
using AndroidX.Core.Graphics.Drawable;
using System;
using static Android.Views.ViewGroup;
namespace astator.Core.UI.Floaty
{
    [Service(Label = ".floaty", ForegroundServiceType = ForegroundService.TypeNone, Enabled = true)]
    public class FloatyService : Service
    {
        public static FloatyService Instance { get; set; }
        private IWindowManager windowManager;
        public void AddView(View view, LayoutParams layoutParams)
        {
            this.windowManager?.AddView(view, layoutParams);
        }
        public void UpdateViewLayout(View view, LayoutParams layoutParams)
        {
            this.windowManager?.UpdateViewLayout(view, layoutParams);
        }
        public void RemoveView(View view)
        {
            this.windowManager?.RemoveView(view);
        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Instance = this;
            this.windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
            StartNotification();
            return base.OnStartCommand(intent, flags, startId);
        }

        private void StartNotification()
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                NotificationChannel channel = new("1003", "悬浮窗服务", NotificationImportance.Default);
                notificationManager.CreateNotificationChannel(channel);
            }

            var notification = new NotificationCompat.Builder(this, "1003")
              .SetContentTitle("悬浮窗服务正在运行中")
              .SetSmallIcon(IconCompat.CreateWithResource(this, Android.Resource.Drawable.SymDefAppIcon))
              .Build();

            StartForeground(1003, notification);
        }

    }
}
