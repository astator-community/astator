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
            new ActivityResultContracts.RequestMultiplePermissions(),
            new ActivityResultCallback(results =>
            {
                dynamic grantResults = null;
                switch (results.Class.Name)
                {
                    case "androidx.collection.SimpleArrayMap":
                        grantResults = results.JavaCast<AndroidX.Collection.SimpleArrayMap>();
                        break;
                    case "androidx.collection.ArrayMap":
                        grantResults = results.JavaCast<AndroidX.Collection.ArrayMap>();
                        break;
                    case "java.util.HashMap":
                        grantResults = results.JavaCast<Java.Util.HashMap>();
                        break;
                    default:
                        ScriptLogger.Error($"ReqMultiplePermissions错误, 未对返回类型: {results.Class.Name} 进行处理!");
                        return;
                }
                this.permissionCallback?.Invoke(!grantResults.ContainsValue(false));
            }));

        this.startActivityLauncher = this.registry.Register("startActivity",
            context,
            new ActivityResultContracts.StartActivityForResult(),
            new ActivityResultCallback(result =>
            {
                var _result = result.JavaCast<ActivityResult>();
                this.startActivityCallback?.Invoke(_result);
            }));
    }

    public void ReqPermissions(string[] permissions, Action<bool> callback)
    {
        this.permissionCallback = callback;
        this.permissionLauncher.Launch(permissions);
    }

    public void StartActivityForResult(Intent intent, Action<ActivityResult> callback)
    {
        this.startActivityCallback = callback;
        this.startActivityLauncher.Launch(intent);
    }
}
