using AndroidX.Activity.Result;

namespace astator.Platforms.Android;

public class ActivityResultCallback : Java.Lang.Object, IActivityResultCallback
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
