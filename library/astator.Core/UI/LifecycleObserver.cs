using Android.Content;
using Android.Runtime;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using System;

namespace astator.Core.UI;

internal class ActivityResultCallback : Java.Lang.Object, IActivityResultCallback
{
    private readonly Action<Java.Lang.Object> callback;
    public ActivityResultCallback(Action<Java.Lang.Object> callback)
    {
        this.callback = callback;
    }
    public void OnActivityResult(Java.Lang.Object result)
    {
        this.callback?.Invoke(result);
    }
}

public class LifecycleObserver : Java.Lang.Object, ILifecycleObserver
{
    private readonly ActivityResultRegistry registry;

    private readonly ActivityResultLauncher permissionLauncher;
    private Action<bool> permissionCallback;

    private readonly ActivityResultLauncher startActivityLauncher;
    private Action<ActivityResult> startActivityCallback;

    public LifecycleObserver(AppCompatActivity context)
    {
        this.registry = context.ActivityResultRegistry;

        this.permissionLauncher = this.registry.Register("permissions",
            context,
            new ActivityResultContracts.RequestPermission(),
            new ActivityResultCallback(result =>
            {
                this.permissionCallback?.Invoke((bool)result);
            }));

        this.startActivityLauncher = this.registry.Register("startActivity",
            context,
            new ActivityResultContracts.StartActivityForResult(),
            new ActivityResultCallback(result =>
            {
                this.startActivityCallback?.Invoke(result.JavaCast<ActivityResult>());
            }));
    }

    public void ReqPermission(string permission, Action<bool> callback)
    {
        this.permissionCallback = callback;
        this.permissionLauncher.Launch(permission);
    }

    public void StartActivityForResult(Intent intent, Action<ActivityResult> callback)
    {
        this.startActivityCallback = callback;
        this.startActivityLauncher.Launch(intent);
    }
}
