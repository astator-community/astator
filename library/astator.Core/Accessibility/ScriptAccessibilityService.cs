using Android.AccessibilityServices;
using Android.App;
using Android.Content;
using Android.Views.Accessibility;
using AndroidX.Core.App;
using AndroidX.Core.Graphics.Drawable;
using System;

namespace astator.Core.Accessibility;

[Service(Label = "astator", Enabled = true, Exported = true, Permission = "android.permission.BIND_ACCESSIBILITY_SERVICE")]
[IntentFilter(new string[] { "android.accessibilityservice.AccessibilityService" })]
[MetaData("android.accessibilityservice", Resource = "@xml/accessibilityservice")]
public class ScriptAccessibilityService : AccessibilityService, IDisposable
{
    public static ScriptAccessibilityService Instance { get; set; }

    protected override void OnServiceConnected()
    {
        Instance = this;
        StartNotification();
        base.OnServiceConnected();
    }

    private void StartNotification()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            NotificationChannel channel = new("1001", "无障碍服务", NotificationImportance.Default);
            notificationManager.CreateNotificationChannel(channel);
        }

        var notification = new NotificationCompat.Builder(this, "1001")
          .SetContentTitle("无障碍服务正在运行中")
          .SetSmallIcon(IconCompat.CreateWithResource(this, Android.Resource.Drawable.SymDefAppIcon))
          .Build();

        StartForeground(1001, notification);
    }

    public override void OnAccessibilityEvent(AccessibilityEvent e)
    {
    }

    public override void OnInterrupt()
    {
        Dispose();
    }

    public override void OnDestroy()
    {
        Dispose();
        base.OnDestroy();
    }


    private bool disposedValue;
    protected new virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                DisableSelf();
                Instance = null; 
                disposedValue = true;
            }
        }
    }

    ~ScriptAccessibilityService()
    {
        Dispose(disposing: false);
    }

    public new void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
